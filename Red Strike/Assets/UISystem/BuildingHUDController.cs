using UnityEngine.UIElements;
using BuildingPlacement.Buildings;
using VehicleSystem;

namespace UISystem
{
    public class BuildingHUDController : GameHUDController
    {
        private VisualElement buildingDetailsPanel;
        
        private Label sharedBuildTypeLabel;
        private Label sharedHealthLabel;
        
        private VisualElement mainStationContent;
        private VisualElement hangarContent;
        private VisualElement energyTowerContent;

        private Label msShieldLabel;
        private Label hIsReadyLabel;
        private Label hProductionLabel;
        private Label etCapacityLabel;
        private Label etDensityLabel; 

        protected Building currentlySelectedBuilding;

        protected override void OnEnable()
        {
            base.OnEnable();

            buildingDetailsPanel = root.Q<VisualElement>("building-details-panel");
            sharedBuildTypeLabel = root.Q<Label>("shared-build-type-label");
            sharedHealthLabel = root.Q<Label>("shared-health-label");

            mainStationContent = root.Q<VisualElement>("main-station-content-area");
            hangarContent = root.Q<VisualElement>("hangar-content-area");
            energyTowerContent = root.Q<VisualElement>("energy-tower-content-area");

            if (mainStationContent != null)
                msShieldLabel = mainStationContent.Q<Label>("shield-label");

            if (energyTowerContent != null)
            {
                etCapacityLabel = energyTowerContent.Q<Label>("capacity-label");
                etDensityLabel = energyTowerContent.Q<Label>("density-label");
            }

            if (hangarContent != null)
            {
                hIsReadyLabel = hangarContent.Q<Label>("is-ready-label");
                hProductionLabel = hangarContent.Q<Label>("in-production-label");
                
                BindHangarButtons();
            }

            var mainStationBtn = root.Q<Button>("main-station-button");
            var hangarBtn = root.Q<Button>("hangar-button");
            var energyTowerBtn = root.Q<Button>("energy-tower-button");

            if(mainStationBtn != null) mainStationBtn.clicked += () => OnBuildingCategoryClicked("Main Station");
            if(hangarBtn != null) hangarBtn.clicked += () => OnBuildingCategoryClicked("Hangar");
            if(energyTowerBtn != null) energyTowerBtn.clicked += () => OnBuildingCategoryClicked("Energy Tower");

            HideBuildingDetails();
        }

        protected override void Update()
        {
            base.Update();
            if (currentlySelectedBuilding != null && buildingDetailsPanel.style.display == DisplayStyle.Flex)
            {
                UpdateBuildingData();
            }
        }

        public void ShowBuildingDetails(Building building)
        {
            if (building == null) return;

            currentlySelectedBuilding = building;
            sharedBuildTypeLabel.text = building.BuildingName;

            HideAllContents();

            if (building is MainStation)
            {
                if(mainStationContent != null) mainStationContent.style.display = DisplayStyle.Flex;
            }
            else if (building is Hangar)
            {
                if(hangarContent != null) hangarContent.style.display = DisplayStyle.Flex;
            }
            else if (building is EnergyTower)
            {
                if(energyTowerContent != null) energyTowerContent.style.display = DisplayStyle.Flex;
            }

            buildingDetailsPanel.style.display = DisplayStyle.Flex;
        }

        public void HideBuildingDetails()
        {
            currentlySelectedBuilding = null;
            if(buildingDetailsPanel != null)
                buildingDetailsPanel.style.display = DisplayStyle.None;
        }

        private void HideAllContents()
        {
            if (mainStationContent != null) mainStationContent.style.display = DisplayStyle.None;
            if (hangarContent != null) hangarContent.style.display = DisplayStyle.None;
            if (energyTowerContent != null) energyTowerContent.style.display = DisplayStyle.None;
        }

        private void UpdateBuildingData()
        {
            if (currentlySelectedBuilding == null) return;

            sharedHealthLabel.text = $"Health: {currentlySelectedBuilding.CurrentHealth:F0}";

            if (currentlySelectedBuilding is MainStation ms)
            {
                if (msShieldLabel != null) msShieldLabel.text = $"Shield: {ms.ShieldAmount:F0}%";
            }
            else if (currentlySelectedBuilding is Hangar hg)
            {
                if (hIsReadyLabel != null) hIsReadyLabel.text = $"Is Ready: {hg.IsReady}";
                if (hProductionLabel != null) hProductionLabel.text = $"Prod: {hg.InProductionUnitName}"; 
            }
            else if (currentlySelectedBuilding is EnergyTower et)
            {
                if (etCapacityLabel != null) etCapacityLabel.text = $"Capacity: {et.GetStatus().current:F0} / {et.GetStatus().max:F0}";
                if (etDensityLabel != null) etDensityLabel.text = $"Density: {et.GetStatus().count:F0} / {et.GetStatus().limit:F0}";
            }
        }

        private void BindHangarButtons()
        {
            if (hangarContent == null) return;

            void BindBtn(string btnName, VehicleTypes type)
            {
                Button btn = hangarContent.Q<Button>(btnName);
                if (btn != null)
                {
                    btn.clicked += () => 
                    {
                        if (currentlySelectedBuilding is Hangar currentHangar)
                        {
                            currentHangar.CreateVehicle(type);
                        }
                    };
                }
            }

            BindBtn("infantry-create-button", VehicleTypes.Infantry);
            BindBtn("trike-create-button", VehicleTypes.Trike);
            BindBtn("quad-create-button", VehicleTypes.Quad);
            BindBtn("tank-combat-create-button", VehicleTypes.Tank_Combat);
            BindBtn("tank-heavy-a-create-button", VehicleTypes.Tank_Heavy_A);
            BindBtn("tank-heavy-b-create-button", VehicleTypes.Tank_Heavy_B);
            BindBtn("ornithopter-a-create-button", VehicleTypes.Ornithopter_A);
            BindBtn("ornithopter-b-create-button", VehicleTypes.Ornithopter_B);
        }

        private void OnBuildingCategoryClicked(string buildingName)
        {
            if (inputController != null)
                inputController.SelectBuildingToPlace(buildingName);
        }
    }
}