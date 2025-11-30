using UnityEngine;

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
            Destroy(gameObject, ammunitionData.lifetime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (hasExploded) return;

            if (ownerVehicle != null && collision.gameObject == ownerVehicle.gameObject)
                return;

            if (collision.gameObject.CompareTag("Ammunition"))
                return;

            hasExploded = true;

            if (explosionEffectPrefab != null)
            {
                GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
                Destroy(explosionEffect, 2f);
            }

            if (collision.gameObject.CompareTag("Enemy"))
            {
                Debug.Log($"Hit enemy: {collision.gameObject.name}, Damage: {ammunitionData.damage}");
                // örn: collision.gameObject.GetComponent<Health>()?.TakeDamage(ammunitionData.damage);
            }

            Destroy(gameObject);
        }
    }
}
