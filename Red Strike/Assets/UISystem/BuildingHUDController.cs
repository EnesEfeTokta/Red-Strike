using UnityEngine;
using UnityEngine.UIElements;
using BuildingPlacement.Buildings;
using VehicleSystem;

namespace UISystem
{
    public class BuildingHUDController : GameHUDController
    {
        [Header("Templates (UXML)")]
        public VisualTreeAsset mainStationTemplate;
        public VisualTreeAsset hangarTemplate;
        public VisualTreeAsset energyTowerTemplate;

        protected VisualElement buildingDetailsPanel;
        private Label sharedBuildTypeLabel;
        private Label sharedHealthLabel;
        
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
            
            buildingDynamicContentContainer.Clear();
            
            if (building is MainStation)
            {
                InstantiateTemplate(mainStationTemplate);
                msShieldLabel = buildingDynamicContentContainer.Q<Label>("shield-label");
            }
            else if (building is Hangar hangarScript)
            {
                InstantiateTemplate(hangarTemplate);
                
                hIsReadyLabel = buildingDynamicContentContainer.Q<Label>("is-ready-label");
                hProductionLabel = buildingDynamicContentContainer.Q<Label>("in-production-label");

                SetupHangarButtons(hangarScript);
            }
            else if (building is EnergyTower)
            {
                InstantiateTemplate(energyTowerTemplate);
                etCapacityLabel = buildingDynamicContentContainer.Q<Label>("capacity-label");
                etDensityLabel = buildingDynamicContentContainer.Q<Label>("density-label");
            }

            buildingDetailsPanel.style.display = DisplayStyle.Flex;
        }

        public void HideBuildingDetails()
        {
            currentlySelectedBuilding = null;
            if(buildingDetailsPanel != null)
                buildingDetailsPanel.style.display = DisplayStyle.None;
        }

        private void UpdateBuildingData()
        {
            if (currentlySelectedBuilding == null) return;

            sharedHealthLabel.text = $"Health: {currentlySelectedBuilding.CurrentHealth:F0}";

            if (currentlySelectedBuilding is MainStation ms)
            {
                if (msShieldLabel != null) 
                    msShieldLabel.text = $"Shield: {ms.ShieldAmount:F0}%";
            }
            else if (currentlySelectedBuilding is Hangar hg)
            {
                if (hIsReadyLabel != null) 
                    hIsReadyLabel.text = $"Is Ready: {hg.IsReady}";
                if (hProductionLabel != null) 
                    hProductionLabel.text = $"Prod: {hg.InProductionUnitName}"; 
            }
            else if (currentlySelectedBuilding is EnergyTower et)
            {
                if (etCapacityLabel != null) 
                    etCapacityLabel.text = $"Capacity: {et.GetStatus().current:F0} / {et.GetStatus().max:F0}";
                if (etDensityLabel != null) 
                    etDensityLabel.text = $"Density: {et.GetStatus().count:F0} / {et.GetStatus().limit:F0}";
            }
        }

        private void SetupHangarButtons(Hangar hangar)
        {
            void BindVehicleBtn(string btnName, VehicleTypes type)
            {
                Button btn = buildingDynamicContentContainer.Q<Button>(btnName);
                
                if (btn != null)
                {
                    btn.clicked += () => 
                    {
                        hangar.CreateVehicle(type); 
                    };
                }
            }

            BindVehicleBtn("infantry-create-button", VehicleTypes.Infantry);
            BindVehicleBtn("trike-create-button", VehicleTypes.Trike);
            BindVehicleBtn("quad-create-button", VehicleTypes.Quad);
            BindVehicleBtn("tank-combat-create-button", VehicleTypes.Tank_Combat);
            BindVehicleBtn("tank-heavy-a-create-button", VehicleTypes.Tank_Heavy_A);
            BindVehicleBtn("tank-heavy-b-create-button", VehicleTypes.Tank_Heavy_B);
            BindVehicleBtn("ornithopter-a-create-button", VehicleTypes.Ornithopter_A);
            BindVehicleBtn("ornithopter-b-create-button", VehicleTypes.Ornithopter_B);
        }

        private void InstantiateTemplate(VisualTreeAsset template)
        {
            if (template != null)
            {
                template.CloneTree(buildingDynamicContentContainer);
            }
        }

        private void OnBuildingCategoryClicked(string buildingName)
        {
            if (inputController != null)
                inputController.SelectBuildingToPlace(buildingName);
        }
    }
}