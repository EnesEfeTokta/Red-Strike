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
            float dt = Runner.DeltaTime;

            // YÖN HESAPLA
            if (TargetId.IsValid)
            {
                var targetObj = Runner.FindObject(TargetId);
                if (targetObj != null)
                {
                    Vector3 direction = (targetObj.transform.position - transform.position).normalized;
                    if (direction != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        // Rigidbody yerine Transform döndür
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, dt * rotationSpeed);
                    }
                }
            }

            // İLERİ GİT (Rigidbody yerine Transform kullan)
            transform.position += transform.forward * currentSpeed * dt;
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("BasicRocket: OnCollisionEnter triggered");
            if (!Object.HasStateAuthority) return;
            if (hasExploded) return;
            Debug.Log($"BasicRocket collided with: {collision.gameObject.name}");

            if (collision.gameObject.TryGetComponent<NetworkObject>(out var netObj))
            {
                if (netObj.Id == OwnerVehicleId) return;
            }
            Debug.Log("Processing collision...");

            var unit = collision.gameObject.GetComponent<Unit.Unit>();
            if (unit == null)
                return;
                Debug.Log("Hit a unit!");

            if (unit.teamId == OwnerTeamId)
            {
                DespawnBullet();
                Debug.Log("Hit friendly unit, no damage applied.");
                return;
            }

            Debug.Log("Applying damage to the unit...");

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
                Debug.Log("Despawning BasicRocket...");
                Runner.Despawn(Object);
            }
        }
    }
}
