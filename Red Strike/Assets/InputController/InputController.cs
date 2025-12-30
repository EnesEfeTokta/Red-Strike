using UnityEngine;
using VehicleSystem;
using BuildingPlacement;
using UISystem;
using System.Linq;
using UnityEngine.UIElements;
using NetworkingSystem;
using AmmunitionSystem;
using UserSystem;
using GameStateSystem;

namespace InputController
{
    public class InputController : MonoBehaviour
    {
        public static InputController Instance;

        [Header("UI & Database")]
        public UIDocument gameUIDocument;
        public BuildingsDatabase buildingsDatabase;
        public VehiclesDatabase vehiclesDatabase;
        public AmmunitionDatabase ammunitionDatabase;
        public User userData;

        [Header("Layers & Camera")]
        public LayerMask terrainLayer;
        public LayerMask selectableLayer;
        private Camera mainCamera;

        [Header("Settings")]
        public float minDistanceBetweenObjects = 5f;
        public int teamId = 0;

        [Header("Audio")]
        public AudioClip selectionSound;
        public AudioClip placementSound;
        public AudioClip errorSound;
        private AudioSource audioSource;

        private BuildingPlacement.Buildings.Building currentSelectedActiveBuilding;
        private Building buildingDataToPlace;
        private VehicleSystem.Vehicles.Vehicle currentSelectedVehicle;

        private VehiclesHUDController vehiclesHUDController;
        private BuildingHUDController buildingHUDController;

        private SelectionHighlighter vehicleHighlighter;

        private SelectionHighlighter targetHighlighter;
        private SelectionHighlighter tempBuildingHighlighter;

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

            audioSource = GetComponentInChildren<AudioSource>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (buildingDataToPlace != null) buildingDataToPlace = null;
                else DeselectAll();

                return;
            }

            if (currentSelectedActiveBuilding != null && tempBuildingHighlighter != null)
            {
                if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.R))
                {
                    currentSelectedActiveBuilding.RPC_Rotate90();
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOverUI()) return;

                if (buildingDataToPlace != null) PlaceBuilding();
                else HandleObjectSelection();
            }
        }

        private void PlaceBuilding()
        {
            if (buildingDataToPlace == null) return;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, terrainLayer))
            {
                Vector3 spawnPosition = hitInfo.point;

                if (!IsPositionValid(spawnPosition))
                {
                    audioSource.PlayOneShot(errorSound);
                    return;
                }

                bool isThereMainBuilding = FindObjectsByType<BuildingPlacement.Buildings.MainStation>(FindObjectsSortMode.None)
                    .Any(station =>
                    {
                        var unit = station.GetComponent<Unit.Unit>();
                        return unit != null && unit.teamId == teamId;
                    });

                if (buildingDataToPlace.buildingName != "Main Station" && !isThereMainBuilding)
                {
                    audioSource.PlayOneShot(errorSound); // "Önce Main Station kurmalısın"
                    return;
                }

                if (buildingDataToPlace.buildingName == "Main Station" && isThereMainBuilding)
                {
                    audioSource.PlayOneShot(errorSound); // "Zaten Main Station var"
                    return;
                }

                if (CommanderData.LocalCommander != null)
                {
                    bool isLimitReached = GameStateManager.Instance.HasReachedLimit(
                        teamId,
                        buildingDataToPlace.buildingName,
                        buildingDataToPlace.maxCreatableCount
                    );

                    if (isLimitReached)
                    {
                        audioSource.PlayOneShot(errorSound); // "Bu bina için limit doldu"
                        return;
                    }

                    spawnPosition = spawnPosition + new Vector3(0, buildingDataToPlace.heightOffset, 0);
                    CommanderData.LocalCommander.RPC_SpawnBuilding(buildingDataToPlace.buildingName, spawnPosition);

                    audioSource.PlayOneShot(placementSound);
                }

                buildingDataToPlace = null;
            }
        }
        private bool IsPositionValid(Vector3 position)
        {
            bool hasObstacle = Physics.CheckSphere(position, minDistanceBetweenObjects, selectableLayer);
            return !hasObstacle;
        }

        public void SelectBuildingToPlace(string buildingName)
        {
            Building buildingData = buildingsDatabase.buildings
                .FirstOrDefault(b => b.buildingName == buildingName);

            if (buildingData != null)
            {
                DeselectAll();

                buildingDataToPlace = buildingData;
                audioSource.PlayOneShot(selectionSound);
            }
            else
            {
                audioSource.PlayOneShot(errorSound);
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
                if (unit == null) { DeselectAll(); return; }
            }

            bool isFriendly = unit.teamId == teamId;
            bool isEnemy = unit.teamId != teamId;

            if (isEnemy)
            {
                if (currentSelectedVehicle != null)
                {
                    currentSelectedVehicle.SetTargetEnemy(unit.gameObject);
                    audioSource.PlayOneShot(selectionSound);

                    if (targetHighlighter != null) targetHighlighter.DisableHighlight();
                    targetHighlighter = GetHighlighter(unit.gameObject);
                    targetHighlighter?.EnableHighlight(teamId, unit.teamId);
                }
                else
                {
                    audioSource.PlayOneShot(errorSound); // "Önce bir arac seçmelisin"
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
                        vehicleHighlighter?.EnableHighlight(teamId, unit.teamId);
                        vehiclesHUDController?.ShowVehicleDetails(currentSelectedVehicle);
                        audioSource.PlayOneShot(selectionSound);

                        if (currentSelectedVehicle.animator != null)
                            currentSelectedVehicle.animator.SetBool("isSelected", true);
                    }
                    break;

                case Unit.UnitType.Building:
                    currentSelectedActiveBuilding = unit.GetComponent<BuildingPlacement.Buildings.Building>();

                    if (currentSelectedActiveBuilding != null)
                    {
                        tempBuildingHighlighter = GetHighlighter(unit.gameObject);
                        tempBuildingHighlighter?.EnableHighlight(teamId, unit.teamId);

                        buildingHUDController.ShowBuildingDetails(currentSelectedActiveBuilding);
                        audioSource.PlayOneShot(selectionSound);

                        if (currentSelectedActiveBuilding.animator != null)
                            currentSelectedActiveBuilding.animator.SetBool("isSelected", true);
                    }
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

            if (currentSelectedVehicle != null && currentSelectedVehicle.animator != null)
            {
                currentSelectedVehicle.animator.SetBool("isSelected", false);
            }

            if (currentSelectedActiveBuilding != null && currentSelectedActiveBuilding.animator != null)
            {
                currentSelectedActiveBuilding.animator.SetBool("isSelected", false);
            }

            vehicleHighlighter = null;
            targetHighlighter = null;
            tempBuildingHighlighter = null;

            currentSelectedVehicle = null;
            currentSelectedActiveBuilding = null;
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