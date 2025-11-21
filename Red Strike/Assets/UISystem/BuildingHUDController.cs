using UnityEngine;
using UnityEngine.UIElements;
using BuildingPlacement.Buildings;

namespace UISystem
{
    public class BuildingHUDController : GameHUDController
    {
        [Header("Templates (UXML)")]
        public VisualTreeAsset mainStationTemplate;
        public VisualTreeAsset hangarTemplate;
        public VisualTreeAsset energyTowerTemplate;

        private VisualElement buildingDetailsPanel;
        private Label sharedBuildTypeLabel;
        private Label sharedHealthLabel;
        
        // Main Station
        private Label msShieldLabel;

        // Hangar
        private Label hIsReadyLabel;
        private Label hProductionLabel;

        // Energy Tower
        private Label etCapacityLabel;
        private Label etDensityLabel;

        private Building currentlySelectedBuilding;

        protected override void OnEnable()
        {
            base.OnEnable();

            buildingDetailsPanel = root.Q<VisualElement>("building-details-panel");
            sharedBuildTypeLabel = root.Q<Label>("shared-build-type-label");
            sharedHealthLabel = root.Q<Label>("shared-health-label");

            var mainStationBtn = root.Q<Button>("main-station-button");
            var hangarBtn = root.Q<Button>("hangar-button");
            var energyTowerBtn = root.Q<Button>("energy-tower-button");

            if(mainStationBtn != null) mainStationBtn.clicked += () => OnBuildingButtonClicked("Main Station");
            if(hangarBtn != null) hangarBtn.clicked += () => OnBuildingButtonClicked("Hangar");
            if(energyTowerBtn != null) energyTowerBtn.clicked += () => OnBuildingButtonClicked("Energy Tower");

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
            else if (building is Hangar)
            {
                InstantiateTemplate(hangarTemplate);
                hIsReadyLabel = buildingDynamicContentContainer.Q<Label>("is-ready-label");
                hProductionLabel = buildingDynamicContentContainer.Q<Label>("in-production-label");
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
                    etCapacityLabel.text = $"Capacity: {et.CurrentCapacity}";
                if (etDensityLabel != null) 
                    etDensityLabel.text = $"Density: {et.Density}";
            }
        }

        private void InstantiateTemplate(VisualTreeAsset template)
        {
            if (template != null)
            {
                template.CloneTree(buildingDynamicContentContainer);
            }
            else
            {
                Debug.LogWarning("İstenen yapı için UXML Template atanmamış!");
            }
        }

        private void OnBuildingButtonClicked(string buildingName)
        {
            if (inputController != null)
                inputController.SelectBuildingToPlace(buildingName);
        }
    }
}