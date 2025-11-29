using UnityEngine;

namespace AmmunitionSystem.Ammunitions.Ammunitions.BasicTankShell
{
    [RequireComponent(typeof(Rigidbody))]
    public class BasicTankShell : Ammunition
    {
        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            rb.linearVelocity = transform.forward * ammunitionData.speed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Debug.Log($"Hit enemy: {collision.gameObject.name}, Damage: {ammunitionData.damage}");
                // Here you would typically access the enemy's health component and apply damage
            }

            if (ownerVehicle.gameObject != collision.gameObject)
            {
                Destroy(gameObject);
            }
        }
    }
}
