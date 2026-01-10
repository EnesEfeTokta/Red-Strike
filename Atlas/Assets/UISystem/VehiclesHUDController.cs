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
        private Label bulletCountLabel;
        private Label bulletReloadLabel;
        private Label rocketCountLabel;
        private Label rocketReloadLabel;
        private Label targetLabel;
        private ProgressBar healthProgressBar;
        private Button clearTargetButton;

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
            bulletCountLabel = detailsPanel.Q<Label>("bullet-count-label");
            bulletReloadLabel = detailsPanel.Q<Label>("bullet-reload-label");
            rocketCountLabel = detailsPanel.Q<Label>("rocket-count-label");
            rocketReloadLabel = detailsPanel.Q<Label>("rocket-reload-label");
            targetLabel = detailsPanel.Q<Label>("shared-vehicle-target-label");
            healthProgressBar = detailsPanel.Q<ProgressBar>("shared-vehicle-health-bar");
            clearTargetButton = detailsPanel.Q<Button>("btn-clear-target");

            if (clearTargetButton != null) clearTargetButton.clicked += OnClearTargetClicked;

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

            if (vehicleNameLabel != null) vehicleNameLabel.text = vehicleName;
            if (healthProgressBar != null) { healthProgressBar.highValue = maxHealth; healthProgressBar.value = currentHealth; }
            if (fuelProgressBar != null) { fuelProgressBar.highValue = maxFuelValue; fuelProgressBar.value = currentFuel; }
            if (bulletCountLabel != null) bulletCountLabel.text = $"{bulletCurrent} / {bulletMax}";
            if (bulletReloadLabel != null) bulletReloadLabel.text = $"RELOADS: {bulletReloadCount}";
            if (rocketCountLabel != null) rocketCountLabel.text = $"{rocketCurrent} / {rocketMax}";
            if (rocketReloadLabel != null) rocketReloadLabel.text = $"RELOADS: {rocketReloadCount}";
            if (targetLabel != null) targetLabel.text = $"{targetName} and its distance: {currentlySelectedVehicle.GetTargetDistance():0.0}m";

            if (currentlySelectedVehicle.targetObject != null)
            {
                if (clearTargetButton != null) clearTargetButton.style.display = DisplayStyle.Flex;
            }
            else
            {
                if (clearTargetButton != null) clearTargetButton.style.display = DisplayStyle.None;
            }
        }

        private void OnClearTargetClicked()
        {
            if (currentlySelectedVehicle != null)
            {
                currentlySelectedVehicle.ClearCommands();

                var targetLabel = root.Q<Label>("shared-vehicle-target-label");
                if (targetLabel != null) targetLabel.text = "TARGET: NONE";

                if (clearTargetButton != null) clearTargetButton.style.display = DisplayStyle.None;
            }
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