using UnityEngine;
using System.Collections.Generic;

namespace VehicleSystem
{
    [CreateAssetMenu(fileName = "New Vehicle", menuName = "Vehicle System/Vehicle")]
    public class Vehicle : ScriptableObject
    {
        [Header("General Settings")]
        public string vehicleName;
        public VehicleTypes vehicleType;
        public GameObject vehiclePrefab;
        public int maxCreatableCount;

        [Header("Vehicle Stats")]
        public float speed;
        public float coverageAreaRadius;
        public float turnSpeed;
        public float stoppingDistance;
        public float maxHealth;
        public float fuelCapacity;
        public float fuelConsumptionRate;

        [Header("Effects")]
        public ParticleSystem explosionEffect;
        public AudioClip engineSound;
        public AudioClip bulletFireSound;
        public AudioClip rocketFireSound;

        [Header("Ammunition Settings")]
        public List<VehicleAmmunition> ammunitionSettings = new List<VehicleAmmunition>();
    }
}
