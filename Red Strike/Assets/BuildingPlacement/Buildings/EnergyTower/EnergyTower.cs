using UnityEngine;
using System.Collections.Generic;
using VehicleSystem.Vehicles;

namespace BuildingPlacement.Buildings
{
    public class EnergyTower : Building
    {
        [Header("Energy Tower Settings")]
        public float maxFuelCapacity = 500f;
        public float currentFuelCapacity = 0f;
        public int maxDensityCapacity = 3;
        public int currentDensity = 3;
        public float rechargeRate = 10f;
        public bool isActive = true;

        private List<Vehicle> connectedVehicles = new List<Vehicle>();

        private void Start()
        {
            currentFuelCapacity = maxFuelCapacity;
        }

        private void Update()
        {
            if (!isActive) return;

            if (currentFuelCapacity < maxFuelCapacity)
            {
                currentFuelCapacity += rechargeRate * Time.deltaTime;
                if (currentFuelCapacity > maxFuelCapacity) currentFuelCapacity = maxFuelCapacity;
            }
        }

        public bool IsAvailable()
        {
            return connectedVehicles.Count < maxDensityCapacity && isActive && currentFuelCapacity > 5f;
        }

        public float GiveEnergy(float requestedAmount)
        {
            if (!isActive || currentFuelCapacity <= 0) return 0f;

            float amountToGive = Mathf.Min(requestedAmount, currentFuelCapacity);
            currentFuelCapacity -= amountToGive;

            return amountToGive;
        }

        public bool NewVehicleConnected(Vehicle vehicle)
        {
            if (connectedVehicles.Contains(vehicle)) 
            {
                return true; 
            }

            if (connectedVehicles.Count < maxDensityCapacity)
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

        public (float currentFuelCapacity, float maxFuelCapacity,
                int connectedVehicles, int maxDensityCapacity
                ) GetStatus()
        {
            return (currentFuelCapacity, maxFuelCapacity, connectedVehicles.Count, maxDensityCapacity);
        }
    }
}