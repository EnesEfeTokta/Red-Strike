using UnityEngine;
using System.Collections.Generic;
using VehicleSystem.Vehicles;

namespace BuildingPlacement.Buildings
{
    public class EnergyTower : Building
    {
        [Header("Energy Tower Settings")]
        public float maxCapacity = 500f;
        public float currentCapacity = 0f;
        public int maxDensity = 3;
        public float rechargeRate = 10f;
        public bool isActive = true;

        private List<Vehicle> connectedVehicles = new List<Vehicle>();

        private void Start()
        {
            currentCapacity = maxCapacity;
        }

        private void Update()
        {
            if (!isActive) return;

            if (currentCapacity < maxCapacity)
            {
                currentCapacity += rechargeRate * Time.deltaTime;
                if (currentCapacity > maxCapacity) currentCapacity = maxCapacity;
            }
        }

        public bool IsAvailable()
        {
            return connectedVehicles.Count < maxDensity && isActive && currentCapacity > 5f;
        }

        public float GiveEnergy(float requestedAmount)
        {
            if (!isActive || currentCapacity <= 0) return 0f;

            float amountToGive = Mathf.Min(requestedAmount, currentCapacity);
            currentCapacity -= amountToGive;

            return amountToGive;
        }

        public bool NewVehicleConnected(Vehicle vehicle)
        {
            if (connectedVehicles.Contains(vehicle)) 
            {
                return true; 
            }

            if (connectedVehicles.Count < maxDensity)
            {
                connectedVehicles.Add(vehicle);
                return true;
            }

            return false;
        }

        public void VehicleDisconnected(Vehicle vehicle)
        {
            if (connectedVehicles.Contains(vehicle))
            {
                connectedVehicles.Remove(vehicle);
            }
        }

        public (float current, float max, int count, int limit) GetStatus()
        {
            return (currentCapacity, maxCapacity, connectedVehicles.Count, maxDensity);
        }
    }
}