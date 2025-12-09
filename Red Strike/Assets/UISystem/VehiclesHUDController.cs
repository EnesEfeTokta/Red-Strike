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

            if (detailsPanel == null)
            {
                Debug.LogError("vehicle-details-panel bulunamadı! UXML'i kontrol edin.");
                return;
            }

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

            if (currentlySelectedVehicle != null && detailsPanel.style.display == DisplayStyle.Flex)
            {
                RefreshVehicleDetails();
            }
        }

        private void RefreshVehicleDetails()
        {
            if (currentlySelectedVehicle == null) return;

            var status = currentlySelectedVehicle.GetVehicleStatus();
            
            string vehicleName = status.Item1;
            string fuel = status.Item2.ToString("F1");
            string ammo = $"{status.Item3} / {status.Item4}";
            string targetName = currentlySelectedVehicle.targetObject != null ? currentlySelectedVehicle.targetObject.name : "None";
            string health = status.Item5.ToString("F0");

            if(vehicleNameLabel != null) vehicleNameLabel.text = vehicleName;
            if(fuelLabel != null) fuelLabel.text = $"Fuel: {fuel}";
            if(ammoLabel != null) ammoLabel.text = $"Ammo: {ammo}";
            if(targetLabel != null) targetLabel.text = $"Target: {targetName}";
            if(healthLabel != null) healthLabel.text = $"Health: {health}";
        }

        public void HideVehicleDetails()
        {
            currentlySelectedVehicle = null;
            if (detailsPanel != null)
                detailsPanel.style.display = DisplayStyle.None;
        }

        public void ShowVehicleDetails(Vehicle vehicleToShow)
        {
            currentlySelectedVehicle = vehicleToShow;
            if (detailsPanel != null)
                detailsPanel.style.display = DisplayStyle.Flex;
        }
    }
}