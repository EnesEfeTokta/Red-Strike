using UnityEngine;

namespace AmmunitionSystem.Ammunitions.BasicBullet
{
    [RequireComponent(typeof(Rigidbody))]
    public class BasicBullet : Ammunition
    {
        private Rigidbody rb;

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
            if (ownerVehicle != null && collision.gameObject == ownerVehicle.gameObject) return;
            
            if (collision.gameObject.CompareTag("Ammunition")) return;

            if (collision.gameObject.CompareTag("Enemy"))
            {
                Debug.Log($"Hit enemy: {collision.gameObject.name}, Damage: {ammunitionData.damage}");
                // collision.gameObject.GetComponent<HealthSystem>()?.TakeDamage(ammunitionData.damage);
            }

            Destroy(gameObject);
        }
    }
}