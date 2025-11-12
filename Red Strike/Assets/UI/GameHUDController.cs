using UnityEngine;
using UnityEngine.UIElements;
using VehicleSystem.Vehicles;

namespace UI
{
    public class GameHUDController : MonoBehaviour
    {
        public InputController.InputController inputController;

        private VisualElement detailsPanel;

        private Button mainStationButton;
        private Button hangarButton;
        private Button energyTowerButton;

        private Button infantryButton;
        private Button trikeQuadButton;
        private Button ornithopterAButton;
        private Button ornithopterBButton;
        private Button tankCombatButton;
        private Button tankHeavyAButton;
        private Button tankHeavyBButton;

        private Label vehicleNameLabel;
        private Label fuelLabel;
        private Label ammoLabel;
        private Label targetLabel;

        private UIDocument uiDocument;

        private Vehicle currentlySelectedVehicle;

        private void OnEnable()
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("Bu objede UIDocument bileşeni bulunamadı!", this);
                return;
            }
            var root = uiDocument.rootVisualElement;

            detailsPanel = root.Q<VisualElement>("details-panel");

            mainStationButton = root.Q<Button>("main-station-button");
            hangarButton = root.Q<Button>("hangar-button");
            energyTowerButton = root.Q<Button>("energy-tower-button");
            infantryButton = root.Q<Button>("infantry-button");
            trikeQuadButton = root.Q<Button>("trike-quad-button");
            ornithopterAButton = root.Q<Button>("ornithopter-a-button");
            ornithopterBButton = root.Q<Button>("ornithopter-b-button");
            tankCombatButton = root.Q<Button>("tank-combat-button");
            tankHeavyAButton = root.Q<Button>("tank-heavy-a-button");
            tankHeavyBButton = root.Q<Button>("tank-heavy-b-button");

            vehicleNameLabel = root.Q<Label>("vehicle-name-label");
            fuelLabel = root.Q<Label>("fuel-label");
            ammoLabel = root.Q<Label>("ammo-label");
            targetLabel = root.Q<Label>("target-label");

            mainStationButton.clicked += OnMainStationClicked;
            hangarButton.clicked += OnHangarClicked;
            energyTowerButton.clicked += OnEnergyTowerClicked;
            infantryButton.clicked += OnInfantryClicked;
            trikeQuadButton.clicked += OnTrikeQuadClicked;
            ornithopterAButton.clicked += OnOrnithopterAClicked;
            ornithopterBButton.clicked += OnOrnithopterBClicked;
            tankCombatButton.clicked += OnTankCombatClicked;
            tankHeavyAButton.clicked += OnTankHeavyAClicked;
            tankHeavyBButton.clicked += OnTankHeavyBClicked;
        }

        private void OnDisable()
        {
            mainStationButton.clicked -= OnMainStationClicked;
            hangarButton.clicked -= OnHangarClicked;
            energyTowerButton.clicked -= OnEnergyTowerClicked;
            infantryButton.clicked -= OnInfantryClicked;
            trikeQuadButton.clicked -= OnTrikeQuadClicked;
            ornithopterAButton.clicked -= OnOrnithopterAClicked;
            ornithopterBButton.clicked -= OnOrnithopterBClicked;
            tankCombatButton.clicked -= OnTankCombatClicked;
            tankHeavyAButton.clicked -= OnTankHeavyAClicked;
            tankHeavyBButton.clicked -= OnTankHeavyBClicked;
        }

        private void Update()
        {
            if (currentlySelectedVehicle != null)
            {
                RefreshVehicleDetails();
            }
        }

        private void OnBuildingButtonClicked(string buildingName)
        {
            if (inputController != null)
            {
                inputController.SelectBuildingToPlace(buildingName);
            }
            else
            {
                Debug.LogError("InputController referansı GameHUDController'da atanmamış!");
            }
        }

        private void OnMainStationClicked()
        {
            OnBuildingButtonClicked("Main Station");
        }

        private void OnHangarClicked()
        {
            OnBuildingButtonClicked("Hangar");
        }

        private void OnEnergyTowerClicked()
        {
            OnBuildingButtonClicked("Energy Tower");
        }

        private void OnInfantryClicked()
        {
            Debug.Log("Infantry butonu tıklandı!");
        }

        private void OnTrikeQuadClicked()
        {
            Debug.Log("Trike Quad butonu tıklandı!");
        }

        private void OnOrnithopterAClicked()
        {
            Debug.Log("Ornithopter A butonu tıklandı!");
        }

        private void OnOrnithopterBClicked()
        {
            Debug.Log("Ornithopter B butonu tıklandı!");
        }

        private void OnTankCombatClicked()
        {
            Debug.Log("Tank Combat butonu tıklandı!");
        }

        private void OnTankHeavyAClicked()
        {
            Debug.Log("Tank Heavy A butonu tıklandı!");
        }

        private void OnTankHeavyBClicked()
        {
            Debug.Log("Tank Heavy B butonu tıklandı!");
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