using UnityEngine;
using UnityEngine.UIElements;
using VehicleSystem.Vehicles;

namespace UISystem
{
    public class VehiclesHUDController : GameHUDController
    {
        private VisualElement detailsPanel;
        
        private Label vehicleNameLabel;
        private Label fuelLabel;
        private Label ammoLabel;
        private Label targetLabel;
        private Label healthLabel;

        private Vehicle currentlySelectedVehicle;

        protected override void OnEnable()
        {
            base.OnEnable();

            detailsPanel = root.Q<VisualElement>("vehicle-details-panel");

            vehicleNameLabel = detailsPanel.Q<Label>("shared-vehicle-name-label");
            fuelLabel = detailsPanel.Q<Label>("shared-vehicle-fuel-label");
            ammoLabel = detailsPanel.Q<Label>("shared-vehicle-ammo-label");
            targetLabel = detailsPanel.Q<Label>("shared-vehicle-target-label");
            healthLabel = detailsPanel.Q<Label>("shared-vehicle-health-label");

            HideVehicleDetails();
        }

        protected override void Update()
        {
            base.Update();

            if (currentlySelectedVehicle != null)
            {
                RefreshVehicleDetails();
            }
        }

        private void RefreshVehicleDetails()
        {
            var status = currentlySelectedVehicle.GetVehicleStatus();
            string vehicleName = status.Item1;
            string fuel = status.Item2.ToString("F1");
            string ammo = status.Item3 + " / " + status.Item4;
            string targetName = currentlySelectedVehicle.targetObject != null ? currentlySelectedVehicle.targetObject.name : "None";

            vehicleNameLabel.text = vehicleName;
            fuelLabel.text = "Fuel Level: " + fuel;
            ammoLabel.text = "Ammunition: " + ammo;
            targetLabel.text = "Target: " + targetName;
            healthLabel.text = "Health: " + status.Item5.ToString("F1");
        }

        public void HideVehicleDetails()
        {
            currentlySelectedVehicle = null;
            detailsPanel.style.display = DisplayStyle.None;
        }

        public void ShowVehicleDetails(Vehicle vehicleToShow)
        {
            currentlySelectedVehicle = vehicleToShow;
            detailsPanel.style.display = DisplayStyle.Flex;
        }
    }
}
