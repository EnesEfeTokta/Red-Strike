using UnityEngine;
using UnityEngine.UIElements;
using VehicleSystem.Vehicles;

namespace UISystem
{
    public class VehiclesHUDController : GameHUDController
    {
        private VisualElement detailsPanel;
        
        private Label vehicleNameLabel;
        private ProgressBar fuelProgressBar;
        private Label ammoLabel;
        private Label targetLabel;
        private ProgressBar healthProgressBar;

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
            fuelProgressBar = detailsPanel.Q<ProgressBar>("shared-vehicle-fuel-bar");
            ammoLabel = detailsPanel.Q<Label>("shared-vehicle-ammo-label");
            targetLabel = detailsPanel.Q<Label>("shared-vehicle-target-label");
            healthProgressBar = detailsPanel.Q<ProgressBar>("shared-vehicle-health-bar");

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
            
            string vehicleName = status.vehicleName;
            float currentHealth = status.currentHealth;
            float maxHealth = status.maxHealth;
            float currentFuel = status.currentFuel;
            float maxFuelValue = status.maxFuel;
            int bulletCurrent = status.bulletCurrent;
            int bulletMax = status.bulletMax;
            int bulletReloadCount = status.bulletReloadCount;
            int rocketCurrent = status.rocketCurrent;
            int rocketMax = status.rocketMax;
            int rocketReloadCount = status.rocketReloadCount;

            string targetName = currentlySelectedVehicle.targetObject != null ? currentlySelectedVehicle.targetObject.name : "None";

            if(vehicleNameLabel != null) vehicleNameLabel.text = vehicleName;
            if(healthProgressBar != null) { healthProgressBar.highValue = maxHealth; healthProgressBar.value = currentHealth; }
            if(fuelProgressBar != null) { fuelProgressBar.highValue = maxFuelValue; fuelProgressBar.value = currentFuel; }
            if(ammoLabel != null) ammoLabel.text = $"Bullets: {bulletCurrent}/{bulletMax} | Rockets: {rocketCurrent}/{rocketMax}";
            if(targetLabel != null) targetLabel.text = $"Target: {targetName}";
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