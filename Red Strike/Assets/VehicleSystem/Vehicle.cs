using UnityEngine;

namespace VehicleSystem
{
    [CreateAssetMenu(fileName = "New Vehicle", menuName = "Vehicle System/Vehicle")]
    public class Vehicle : ScriptableObject
    {
        public string vehicleName;
        public float speed;
        public float coverageAreaRadius;
        public float turnSpeed;
        public float stoppingDistance;
        public float maxHealth;
        public float fuelCapacity;
        public float fuelConsumptionRate;
        public int maxAmmunition;
        public float bulletDamage;
        public float bulletSpeed;
        public float reloadTime;

        public GameObject bulletPrefab;
    }
}
