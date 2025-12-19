using UnityEngine;
using Fusion;

namespace AmmunitionSystem.Ammunitions.Ammunitions.BasicTankShell
{
    [RequireComponent(typeof(Rigidbody))]
    public class BasicTankShell : Ammunition
    {
        private Rigidbody rb;
        public GameObject explosionEffectPrefab;
        private bool hasExploded = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            rb.linearVelocity = transform.forward * ammunitionData.speed;
            
            if (Object.HasStateAuthority)
            {
                Invoke(nameof(DespawnBullet), ammunitionData.lifetime);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (Object.HasStateAuthority == false) return;

            if (hasExploded) return;

            if (collision.gameObject.TryGetComponent<NetworkObject>(out var netObj))
            {
                if (netObj.Id == OwnerVehicleId) return;
            }

            var unit = collision.gameObject.GetComponent<Unit.Unit>();
            if (unit == null) return;

            if (unit.teamId == OwnerTeamId)
            {
                DespawnBullet();
                return;
            }

            //Debug.Log($"Hit unit: {collision.gameObject.name}, Damage: {ammunitionData.damage}");
            unit.TakeDamage(ammunitionData.damage);

            hasExploded = true;

            if (explosionEffectPrefab != null)
            {
                GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
                Destroy(explosionEffect, 2f);
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
