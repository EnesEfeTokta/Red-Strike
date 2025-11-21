using UnityEngine;

namespace UISystem
{
    public class BuildingHUDController : GameHUDController
    {
        protected override void OnEnable()
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("Bu objede UIDocument bileşeni bulunamadı!", this);
                return;
            }
            var root = uiDocument.rootVisualElement;

            mainStationButton = root.Q<Button>("main-station-button");
            hangarButton = root.Q<Button>("hangar-button");
            energyTowerButton = root.Q<Button>("energy-tower-button");

            mainStationButton.clicked += OnMainStationClicked;
            hangarButton.clicked += OnHangarClicked;
            energyTowerButton.clicked += OnEnergyTowerClicked;
        }
    }
}
