using System.Collections.Generic;
using UnityEngine;
using VehicleSystem;
using BuildingPlacement;
using UISystem;
using System.Linq;
using UnityEngine.UIElements;
using NetworkingSystem;

namespace InputController
{
    public class InputController : MonoBehaviour
    {
        public static InputController Instance;

        [Header("UI & Database")]
        public UIDocument gameUIDocument;
        public BuildingsDatabase buildingsDatabase;
        public VehiclesDatabase vehiclesDatabase;

        [Header("Layers & Camera")]
        public LayerMask terrainLayer;
        public LayerMask selectableLayer;
        private Camera mainCamera;

        [Header("Settings")]
        public float minDistanceBetweenObjects = 5f;
        public int teamId = 0;

        private Building currentSelectedBuilding;
        private VehicleSystem.Vehicles.Vehicle currentSelectedVehicle;
        
        private VehiclesHUDController vehiclesHUDController;
        private BuildingHUDController buildingHUDController;

        private SelectionHighlighter vehicleHighlighter;

        private SelectionHighlighter targetHighlighter;
        private SelectionHighlighter tempBuildingHighlighter;

        // Limitleme için sayaç (İsteğe bağlı, şimdilik basit tutuyoruz)
        private Dictionary<string, int> buildingCounts = new Dictionary<string, int>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            mainCamera = Camera.main;
            vehiclesHUDController = GetComponent<VehiclesHUDController>();
            buildingHUDController = GetComponent<BuildingHUDController>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (currentSelectedBuilding != null)
                {
                    currentSelectedBuilding = null;
                    Debug.Log("Bina yerleştirme iptal edildi.");
                }
                else
                {
                    DeselectAll();
                }
                return;
            }

            if (tempBuildingHighlighter != null)
            {
                if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.R))
                {
                    tempBuildingHighlighter.transform.Rotate(0, 90, 0);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOverUI()) return;

                if (currentSelectedBuilding != null)
                {
                    PlaceBuilding();
                }
                else
                {
                    HandleObjectSelection();
                }
            }
        }

        private void PlaceBuilding()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, terrainLayer))
            {
                Vector3 spawnPosition = hitInfo.point;

                if (IsPositionValid(spawnPosition))
                {
                    bool isThereMainBuilding = FindObjectsByType<BuildingPlacement.Buildings.MainStation>(FindObjectsSortMode.None)
                        .Any(station => 
                        {
                            var unit = station.GetComponent<Unit.Unit>();
                            return unit != null && unit.teamId == teamId; 
                        });

                    if (currentSelectedBuilding.buildingName != "Main Station" && !isThereMainBuilding)
                    {
                        Debug.LogWarning("Önce bir Ana Üs (Main Station) yerleştirmelisiniz!");
                        return;
                    }

                    if (currentSelectedBuilding.buildingName == "Main Station" && isThereMainBuilding)
                    {
                        Debug.LogWarning("Zaten bir Ana Üs'sünüz var.");
                        return;
                    }

                    if (CommanderData.LocalCommander != null)
                    {
                        CommanderData.LocalCommander.RPC_SpawnBuilding(currentSelectedBuilding.buildingName, spawnPosition);
                        Debug.Log($"<color=green>Server'a İstek:</color> {currentSelectedBuilding.buildingName} kuruluyor...");
                    }
                    else
                    {
                        Debug.LogError("HATA: LocalCommander (Network Bağlantısı) bulunamadı!");
                        return;
                    }

                    currentSelectedBuilding = null;
                }
                else
                {
                    Debug.Log("Bu alan inşaat için uygun değil (Çok yakın veya engel var).");
                }
            }
        }

        private bool IsPositionValid(Vector3 position)
        {
            bool hasObstacle = Physics.CheckSphere(position, minDistanceBetweenObjects, selectableLayer);
            return !hasObstacle;
        }

        public void SelectBuildingToPlace(string buildingName)
        {
            Building buildingToSelect = buildingsDatabase.buildings.FirstOrDefault(b => b.buildingName == buildingName);

            if (buildingToSelect != null)
            {
                currentSelectedBuilding = buildingToSelect;
                Debug.Log($"Seçilen: {currentSelectedBuilding.buildingName}. Yere tıklayın.");
            }
            else
            {
                Debug.LogError($"Database Hatası: {buildingName} bulunamadı!");
            }
        }

        private void HandleObjectSelection()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (!Physics.Raycast(ray, out hitInfo, Mathf.Infinity, selectableLayer))
            {
                DeselectAll();
                return;
            }

            var unit = hitInfo.collider.GetComponent<Unit.Unit>();
            if (unit == null)
            {
                unit = hitInfo.collider.GetComponentInParent<Unit.Unit>();
                if(unit == null) { DeselectAll(); return; }
            }

            bool isFriendly = unit.teamId == teamId;
            bool isEnemy = unit.teamId != teamId;

            if (isEnemy)
            {
                if (currentSelectedVehicle != null)
                {
                    Debug.Log($"Saldırı Emri: {currentSelectedVehicle.name} -> {unit.name}");
                    currentSelectedVehicle.SetTargetEnemy(unit.gameObject);

                    if (targetHighlighter != null) targetHighlighter.DisableHighlight();
                    targetHighlighter = GetHighlighter(unit.gameObject);
                    targetHighlighter?.EnableHighlight();
                }
                else
                {
                    Debug.Log("Düşmanı seçmek için önce kendi aracınızı seçin.");
                }
                return;
            }

            DeselectAll();

            switch (unit.unitType)
            {
                case Unit.UnitType.Vehicle:
                    currentSelectedVehicle = unit.GetComponent<VehicleSystem.Vehicles.Vehicle>();
                    if (currentSelectedVehicle != null)
                    {
                        vehicleHighlighter = GetHighlighter(unit.gameObject);
                        vehicleHighlighter?.EnableHighlight();
                        vehiclesHUDController?.ShowVehicleDetails(currentSelectedVehicle);
                    }
                    break;

                case Unit.UnitType.Building:
                    tempBuildingHighlighter = GetHighlighter(unit.gameObject);
                    tempBuildingHighlighter?.EnableHighlight();
                    
                    var building = unit.GetComponent<BuildingPlacement.Buildings.Building>();
                    if (building != null) buildingHUDController.ShowBuildingDetails(building);
                    break;
            }
        }

        private void DeselectAll()
        {
            vehiclesHUDController?.HideVehicleDetails();
            buildingHUDController?.HideBuildingDetails();

            vehicleHighlighter?.DisableHighlight();
            targetHighlighter?.DisableHighlight();
            tempBuildingHighlighter?.DisableHighlight();

            vehicleHighlighter = null;
            targetHighlighter = null;
            tempBuildingHighlighter = null;
            
            currentSelectedVehicle = null;
            currentSelectedBuilding = null;
        }

        private SelectionHighlighter GetHighlighter(GameObject obj)
        {
            var hl = obj.GetComponent<SelectionHighlighter>();
            if (hl == null) hl = obj.GetComponentInParent<SelectionHighlighter>();
            return hl;
        }

        private bool IsPointerOverUI()
        {
            if (gameUIDocument == null) return false;
            
            if (UnityEngine.EventSystems.EventSystem.current != null && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return true;

            Vector2 panelLocalPos = RuntimePanelUtils.ScreenToPanel(gameUIDocument.rootVisualElement.panel, Input.mousePosition);
            VisualElement pickedElement = gameUIDocument.rootVisualElement.panel.Pick(panelLocalPos);

            if (pickedElement == null) return false;
            if (pickedElement.name == "root-container") return false;
            if (pickedElement == gameUIDocument.rootVisualElement) return false;

            return true;
        }
    }
}