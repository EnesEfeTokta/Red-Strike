using UnityEngine;
using VehicleSystem;
using NetworkingSystem;

namespace BuildingPlacement.Buildings
{
    public class Hangar : Building
    {
        // TEST
        public bool IsReady = true;
        public string InProductionUnitName = "None";

        private Vector3 vehicleSpawnPoint;

        public VehiclesDatabase vehiclesDatabase;

        private void Start()
        {
            vehicleSpawnPoint = transform.position + transform.forward * 20f;
            vehicleSpawnPoint.y += 2f;
        }

        public void CreateVehicle(VehicleTypes vehicleType)
        {
            if (CanCreateVehicle())
            {
                InProductionUnitName = vehicleType.ToString();
                IsReady = false;
                
                Debug.Log($"Hangar started creating a new vehicle: {InProductionUnitName}");
                CreateVehicleInstance(vehicleType);
            }
            else
            {
                Debug.LogWarning("Hangar is not ready to create a new vehicle or already in production.");
            }
        }

        private void CreateVehicleInstance(VehicleTypes vehicleType)
        {
            foreach (Vehicle item in vehiclesDatabase.vehicles)
            {
                if (item.vehicleType == vehicleType)
                {
                    CommanderData.LocalCommander.RPC_SpawnVehicle(item.vehicleName, vehicleSpawnPoint);
                    InProductionUnitName = "None";
                    IsReady = true;
                    return;
                }
            }
        }

        public bool CanCreateVehicle()
        {
            return IsReady && InProductionUnitName == "None";
        }
    }
}
