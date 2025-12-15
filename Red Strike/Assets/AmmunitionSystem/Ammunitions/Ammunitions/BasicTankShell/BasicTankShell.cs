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

        protected override void Start()
        {
            base.Start();
            rb.linearVelocity = transform.forward * ammunitionData.speed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (hasExploded) return;

            if (ownerVehicle != null && collision.gameObject == ownerVehicle.gameObject)
                return;

            var unit = collision.gameObject.GetComponent<Unit.Unit>();
            if (unit == null)
                return;

            if (unit.teamId == ownerVehicle.teamId)
                return;

            Debug.Log($"Hit unit: {collision.gameObject.name}, Damage: {ammunitionData.damage}");
            unit.TakeDamage(ammunitionData.damage);

            hasExploded = true;

            if (explosionEffectPrefab != null)
            {
                GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
                Destroy(explosionEffect, 2f);
            }

            Runner.Despawn(Object);
        }
    }
}
