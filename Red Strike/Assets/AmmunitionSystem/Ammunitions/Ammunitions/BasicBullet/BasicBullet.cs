using UnityEngine;
using Fusion;

namespace AmmunitionSystem.Ammunitions.BasicBullet
{
    [RequireComponent(typeof(Rigidbody))]
    public class BasicBullet : Ammunition
    {
        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        public override void Spawned()
        {
            rb.linearVelocity = transform.forward * ammunitionData.speed;

            if (Object.HasStateAuthority)
            {
                Invoke(nameof(DespawnBullet), ammunitionData.lifetime);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!Object.HasStateAuthority) return;

            var hitNetObj = collision.gameObject.GetComponentInParent<NetworkObject>();
            
            if (hitNetObj != null)
            {
                if (hitNetObj.Id == OwnerVehicleId) 
                {
                    return; 
                }
            }

            var unit = collision.gameObject.GetComponentInParent<Unit.Unit>();

            if (unit != null)
            {
                if (unit.teamId == OwnerTeamId)
                {
                    DespawnBullet();
                    return;
                }

                //Debug.Log($"Hit Enemy: {unit.name} - Damage: {ammunitionData.damage}");
                unit.TakeDamage(ammunitionData.damage);
            }
            
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