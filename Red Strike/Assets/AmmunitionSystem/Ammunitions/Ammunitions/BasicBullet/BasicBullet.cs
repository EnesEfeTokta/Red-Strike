using UnityEngine;

namespace AmmunitionSystem.Ammunitions.Ammunitions.BasicBullet
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
            Destroy(gameObject, ammunitionData.range / ammunitionData.speed);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Destroy(gameObject);
        }
    }
}
