using UnityEngine;

namespace AmmunitionSystem.Ammunitions.BasicRocket
{
    [RequireComponent(typeof(Rigidbody))]
    public class BasicRocket : Ammunition
    {
        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            Debug.Log($"Firing bullet with Damage: {ammunitionData.damage}, Speed: {ammunitionData.speed}");
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
                Debug.Log("Bullet collided with: " + collision.gameObject.name);
                Destroy(gameObject);
            }
        }
    }
}
