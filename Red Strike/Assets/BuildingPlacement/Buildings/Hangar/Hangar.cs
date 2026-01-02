using UnityEngine;
using VehicleSystem;
using NetworkingSystem;
using GameStateSystem;
using System.Linq;

namespace BuildingPlacement.Buildings
{
    public class Hangar : Building
    {
        public bool IsReady = true;
        public string InProductionUnitName = "None";

        private Vector3 vehicleSpawnPoint;
        public VehiclesDatabase vehiclesDatabase;

        private AudioSource audioSource;
        public AudioClip vehicleCreateSound;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            vehicleSpawnPoint = transform.position + transform.forward * 5f;
            vehicleSpawnPoint.y += 2f;
        }

        public void CreateVehicle(VehicleTypes vehicleType)
        {
            var targetVehicleData = vehiclesDatabase.vehicles.FirstOrDefault(v => v.vehicleType == vehicleType);

            if (targetVehicleData == null) return;

            bool limitReached = GameStateManager.Instance.HasReachedLimit(
                teamId,
                targetVehicleData.vehicleName, 
                targetVehicleData.maxCreatableCount
            );

            if (limitReached)
            {
                NotificationSystem.NotificationSystem.Show(
                    "Vehicle Creation Error",
                    "You have reached the limit for creating " + targetVehicleData.vehicleName + ".",
                    NotificationSystem.NotificationType.Error
                );
                return;
            }

            if (CanCreateVehicle())
            {
                InProductionUnitName = vehicleType.ToString();
                IsReady = false;
                audioSource.PlayOneShot(vehicleCreateSound);
                animator.SetTrigger("isProducing");
                CreateVehicleInstance(vehicleType);
            }
            else
            {
                NotificationSystem.NotificationSystem.Show(
                    "Vehicle Creation Error",
                    "Hangar is not ready to create a new vehicle.",
                    NotificationSystem.NotificationType.Warning
                );
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

        public bool CanCreateVehicle() => IsReady && InProductionUnitName == "None";
    }
}