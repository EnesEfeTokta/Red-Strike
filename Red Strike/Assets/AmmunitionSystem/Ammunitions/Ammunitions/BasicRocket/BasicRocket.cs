using UnityEngine;

namespace AmmunitionSystem.Ammunitions.BasicRocket
{
    [RequireComponent(typeof(Rigidbody))]
    public class BasicRocket : Ammunition
    {
        private Rigidbody rb;
        public GameObject explosionEffectPrefab;
        public Transform target;
        public float speed = 20f;
        public float rotationSpeed = 5f;
        public float accelerationRate = 1.5f;

        private bool hasExploded = false;
        private float currentSpeed;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            currentSpeed = speed * 0.5f;
        }

        private void Start()
        {
            Destroy(gameObject, ammunitionData.lifetime);
        }

        public override void SetRocket(Transform targetTransform)
        {
            target = targetTransform;
        }

        private void FixedUpdate()
        {
            if (hasExploded) return;

            currentSpeed = Mathf.Lerp(currentSpeed, speed, Time.fixedDeltaTime * accelerationRate);

            if (target != null)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed));

                rb.linearVelocity = transform.forward * currentSpeed;
            }
            else
            {
                rb.linearVelocity = transform.forward * currentSpeed;
            }
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
