using UnityEngine;
using Fusion;
using NetworkingSystem;

namespace AmmunitionSystem.Ammunitions.BasicRocket
{
    [RequireComponent(typeof(Rigidbody))]
    public class BasicRocket : Ammunition
    {
        private Rigidbody rb;
        public float speed = 20f;
        public float rotationSpeed = 5f;
        public float accelerationRate = 1.5f;

        private bool hasExploded = false;
        private float currentSpeed;
        [Networked] public NetworkId TargetId { get; set; }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            currentSpeed = speed * 0.5f;
        }

        public override void SetRocketTarget(NetworkId targetId)
        {
            TargetId = targetId;
        }

        public override void FixedUpdateNetwork()
        {
            if (!Object.HasStateAuthority) return;
            if (hasExploded) return;

            currentSpeed = Mathf.Lerp(currentSpeed, speed, Runner.DeltaTime * accelerationRate);

            if (TargetId.IsValid)
            {
                var targetObj = Runner.FindObject(TargetId);

                if (targetObj != null)
                {
                    Vector3 direction = (targetObj.transform.position - transform.position).normalized;
                    if (direction != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Runner.DeltaTime * rotationSpeed));
                    }
                }
            }

            rb.linearVelocity = transform.forward * currentSpeed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!Object.HasStateAuthority) return;
            if (hasExploded) return;

            if (collision.gameObject.TryGetComponent<NetworkObject>(out var netObj))
            {
                if (netObj.Id == OwnerVehicleId) return;
            }

            var unit = collision.gameObject.GetComponent<Unit.Unit>();
            if (unit == null)
                return;

            if (unit.teamId == OwnerTeamId)
            {
                DespawnBullet();
                return;
            }

            Debug.Log($"Hit unit: {collision.gameObject.name}, Damage: {ammunitionData.damage}");
            unit.TakeDamage(ammunitionData.damage);

            hasExploded = true;
            CommanderData.LocalCommander.RPC_SpawnExplosionEffect(transform.position);

            DespawnBullet();
        }

        private void DespawnBullet()
        {
            if (Object != null && Object.IsValid)
            {
                Runner.Despawn(Object);
            }
        }
    }
}
