using TMPro;
using UnityEngine;

namespace VehicleSystem
{
    public class VehiclesDetailManager : MonoBehaviour
    {
        public TMP_Text vehicleNameText;
        public TMP_Text fuelLevelText;
        public TMP_Text bulletCountText;
        public TMP_Text targetObjectNameText;

        private Vehicles.Vehicle currentlySelectedVehicle;

        private void Start()
        {
            HideDetailsPanel();
        }

        private void Update()
        {
            if (currentlySelectedVehicle != null)
            {
                UpdateUIElements();
            }
        }

        public void UpdateVehicleDetails(Vehicles.Vehicle vehicle)
        {
            currentlySelectedVehicle = vehicle;
            UpdateUIElements();
        }

        public void HideDetailsPanel()
        {
            vehicleNameText.text = "Vehicle: ";
            fuelLevelText.text = "Fuel Level: ";
            bulletCountText.text = "Ammunition: ";
            targetObjectNameText.text = "Target: ";
        }

        private void UpdateUIElements()
        {
            vehicleNameText.text = $"Vehicle: {currentlySelectedVehicle.vehicleData.vehicleName}";
            fuelLevelText.text = $"Fuel Level: {currentlySelectedVehicle.GetVehicleStatus().Item1:0.00}";
            bulletCountText.text = $"Ammunition: {currentlySelectedVehicle.GetVehicleStatus().Item2}/{currentlySelectedVehicle.GetVehicleStatus().Item3}";

            if (currentlySelectedVehicle.targetObject != null)
            {
                targetObjectNameText.text = $"Target: {currentlySelectedVehicle.targetObject.name}";
            }
            else
            {
                targetObjectNameText.text = "Target: None";
            }
        }
    }
}
