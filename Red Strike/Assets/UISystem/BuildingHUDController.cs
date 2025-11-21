using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using UnityEngine.UIElements;
using BuildingPlacement.Buildings;

namespace UISystem
{
    public class BuildingHUDController : GameHUDController
    {
        private Button mainStationButton;
        private Button hangarButton;
        private Button energyTowerButton;


        #region Main Station Details
        private VisualElement mainStationDetailsPanel;
        private Label mainStationNameLabel;
        private Label mainStationHealthLabel;

        private Building currentlySelectedBuilding;
        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();

            mainStationDetailsPanel = root.Q<VisualElement>("main-station-details-panel");
            mainStationNameLabel = mainStationDetailsPanel.Q<Label>("build-type-label");
            mainStationHealthLabel = mainStationDetailsPanel.Q<Label>("building-health-label");

            mainStationButton = root.Q<Button>("main-station-button");
            hangarButton = root.Q<Button>("hangar-button");
            energyTowerButton = root.Q<Button>("energy-tower-button");

            mainStationButton.clicked += OnMainStationClicked;
            hangarButton.clicked += OnHangarClicked;
            energyTowerButton.clicked += OnEnergyTowerClicked;

            HideBuildingDetails();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            mainStationButton.clicked -= OnMainStationClicked;
            hangarButton.clicked -= OnHangarClicked;
            energyTowerButton.clicked -= OnEnergyTowerClicked;
        }

        protected override void Update()
        {
            base.Update();

            if (currentlySelectedBuilding != null)
            {
                RefreshVehicleDetails();
            }
        }

        private void RefreshVehicleDetails()
        {
            var status = currentlySelectedBuilding.GetBuildingStatus();
            string buildingName = status.Item1;
            string health = status.Item2.ToString("F1");
            mainStationNameLabel.text = buildingName;
            mainStationHealthLabel.text = "Health: " + health;
        }

        public void HideBuildingDetails()
        {
            currentlySelectedBuilding = null;
            mainStationDetailsPanel.style.display = DisplayStyle.None;
        }

        public void ShowBuildingDetails(Building buildingToShow)
        {
            currentlySelectedBuilding = buildingToShow;
            mainStationDetailsPanel.style.display = DisplayStyle.Flex;
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
    }
}
