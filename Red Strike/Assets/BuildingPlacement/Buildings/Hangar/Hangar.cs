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
        public AudioClip errorSound;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            vehicleSpawnPoint = transform.position + transform.forward * 5f;
            vehicleSpawnPoint.y += 2f;
        }

        public void CreateVehicle(VehicleTypes vehicleType)
        {
            var targetVehicleData = vehiclesDatabase.vehicles.FirstOrDefault(v => v.vehicleType == vehicleType);

            if (targetVehicleData == null) 
            {
                //Debug.LogError("Araç database'de bulunamadı!");
                return;
            }

            bool limitReached = GameStateManager.Instance.HasReachedLimit(
                teamId,
                targetVehicleData.vehicleName, 
                targetVehicleData.maxCreatableCount
            );

            if (limitReached)
            {
                //Debug.LogWarning("Araç limitine ulaşıldı!");
                audioSource.PlayOneShot(errorSound);
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
                audioSource.PlayOneShot(errorSound);
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