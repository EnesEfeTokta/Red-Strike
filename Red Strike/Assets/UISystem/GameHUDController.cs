using UnityEngine;
using UnityEngine.UIElements;
using VehicleSystem.Vehicles;

namespace UISystem
{
    public class GameHUDController : MonoBehaviour
    {
        public InputController.InputController inputController;

        private Button mainStationButton;
        private Button hangarButton;
        private Button energyTowerButton;

        protected UIDocument uiDocument;

        protected virtual void OnEnable()
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

        private void OnDisable()
        {
            mainStationButton.clicked -= OnMainStationClicked;
            hangarButton.clicked -= OnHangarClicked;
            energyTowerButton.clicked -= OnEnergyTowerClicked;
        }

        protected virtual void Update() { /* Boş bırakıldı, alt sınıflar tarafından geçersiz kılınabilir */ }

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