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

        private Vehicle currentlySelectedVehicle;

        protected override void OnEnable()
        {
            base.OnEnable();

            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("Bu objede UIDocument bileşeni bulunamadı!", this);
                return;
            }
            var root = uiDocument.rootVisualElement;

            detailsPanel = root.Q<VisualElement>("details-panel");

            vehicleNameLabel = root.Q<Label>("vehicle-name-label");
            fuelLabel = root.Q<Label>("fuel-label");
            ammoLabel = root.Q<Label>("ammo-label");
            targetLabel = root.Q<Label>("target-label");
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
