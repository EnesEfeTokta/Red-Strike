using UnityEngine;
using VehicleSystem.Vehicles;
using BuildingPlacement.Buildings;

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
            if (ownerVehicle != null && collision.gameObject == ownerVehicle.gameObject)
                return;

            var unit = collision.gameObject.GetComponent<Unit.Unit>();
            if (unit == null)
                return;

            if (unit.teamId == ownerVehicle.teamId)
                return;

            Debug.Log($"Hit unit: {collision.gameObject.name}, Damage: {ammunitionData.damage}");
            // unit.GetComponent<HealthSystem>()?.TakeDamage(ammunitionData.damage);

            Destroy(gameObject);
        }

    }
}