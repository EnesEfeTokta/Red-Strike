using System.Collections.Generic;
using UnityEngine;
using VehicleSystem.Vehicles;
using BuildingPlacement;
using UISystem;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace InputController
{
    public class InputController : MonoBehaviour
    {
        public UIDocument gameUIDocument;

        public LayerMask terrainLayer;
        public LayerMask selectableLayer;
        private Camera mainCamera;
        public BuildingsDatabase buildingsDatabase;
        private Dictionary<string, int> buildingCounts = new Dictionary<string, int>();
        private Building currentSelectedBuilding;
        private Vehicle currentSelectedVehicle;
        private List<GameObject> placedObjects = new List<GameObject>();
        public float minDistanceBetweenObjects = 5f;
        public VehiclesHUDController vehiclesHUDController;
        public BuildingHUDController buildingHUDController;

        private SelectionHighlighter vehicleHighlighter;
        private SelectionHighlighter targetHighlighter;

        private SelectionHighlighter tempBuildingHighlighter;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1) && currentSelectedBuilding != null)
            {
                DeselectAll();
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOverUI())
                {
                    return;
                }

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

        private bool IsPointerOverUI()
        {
            if (gameUIDocument == null) return false;

            Vector2 mousePosition = Input.mousePosition;
            Vector2 pointerPosition = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
            VisualElement pickedElement = gameUIDocument.rootVisualElement.panel.Pick(pointerPosition);

            if (pickedElement == null) return false;
            if (pickedElement.name == "root-container") return false;
            if (pickedElement == gameUIDocument.rootVisualElement) return false;

            return true;
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
                    if (buildingCounts.ContainsKey(currentSelectedBuilding.buildingName) &&
                        buildingCounts[currentSelectedBuilding.buildingName] >= currentSelectedBuilding.maxCreatedUnits)
                    {
                        Debug.Log(currentSelectedBuilding.buildingName + " için maksimum yerleştirme limitine ulaşıldı.");
                        return;
                    }
                    GameObject placedObject = Instantiate(currentSelectedBuilding.buildingPrefab, spawnPosition, Quaternion.identity);

                    if (placedObject.GetComponent<SelectionHighlighter>() == null)
                        placedObject.AddComponent<SelectionHighlighter>();

                    placedObjects.Add(placedObject);

                    if (buildingCounts.ContainsKey(currentSelectedBuilding.buildingName))
                        buildingCounts[currentSelectedBuilding.buildingName]++;
                    else
                        buildingCounts[currentSelectedBuilding.buildingName] = 1;

                    currentSelectedBuilding = null;
                    Debug.Log(placedObject.name + " yerleştirildi.");
                }
                else
                {
                    Debug.Log("Bu pozisyon başka bir objeye çok yakın!");
                }
            }
        }

        public void SelectBuildingToPlace(string buildingName)
        {
            Building buildingToSelect = buildingsDatabase.buildings.FirstOrDefault(b => b.buildingName == buildingName);

            if (buildingToSelect != null)
            {
                currentSelectedBuilding = buildingToSelect;
                Debug.Log("Seçilen bina: " + currentSelectedBuilding.buildingName + ". Yerleştirmek için araziye tıklayın.");
            }
            else
            {
                Debug.LogError(buildingName + " isminde bir bina veritabanında bulunamadı!");
            }
        }

        private void HandleObjectSelection()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, selectableLayer))
            {
                GameObject hitObject = hitInfo.collider.gameObject;

                if (currentSelectedVehicle != null && hitObject.CompareTag("Enemy"))
                {
                    Debug.Log($"Araç ({currentSelectedVehicle.name}) hedefe ({hitObject.name}) kilitlendi.");
                    currentSelectedVehicle.SetTargetEnemy(hitObject);

                    if (targetHighlighter != null)
                        targetHighlighter.DisableHighlight();

                    targetHighlighter = GetHighlighter(hitObject);
                    if (targetHighlighter != null)
                        targetHighlighter.EnableHighlight();

                    return;
                }

                DeselectAll();

                switch (hitObject.tag)
                {
                    case "Vehicle":
                        currentSelectedVehicle = hitObject.GetComponent<Vehicle>();

                        if (currentSelectedVehicle != null)
                        {
                            vehicleHighlighter = GetHighlighter(hitObject);
                            if (vehicleHighlighter != null)
                                vehicleHighlighter.EnableHighlight();

                            if (currentSelectedVehicle.targetObject != null)
                            {
                                targetHighlighter = GetHighlighter(currentSelectedVehicle.targetObject);
                                if (targetHighlighter != null)
                                    targetHighlighter.EnableHighlight();
                            }

                            if (vehiclesHUDController != null)
                                vehiclesHUDController.ShowVehicleDetails(currentSelectedVehicle);
                        }
                        break;

                    case "Build":
                        tempBuildingHighlighter = GetHighlighter(hitObject);
                        if (tempBuildingHighlighter != null)
                            tempBuildingHighlighter.EnableHighlight();

                        var b = hitObject.GetComponent<BuildingPlacement.Buildings.Building>();
                        if (b != null) buildingHUDController.ShowBuildingDetails(b);
                        break;

                    case "Enemy":
                        tempBuildingHighlighter = GetHighlighter(hitObject);
                        if (tempBuildingHighlighter != null)
                            tempBuildingHighlighter.EnableHighlight();
                        break;

                    default:
                        DeselectAll();
                        break;
                }
            }
            else
            {
                DeselectAll();
            }
        }

        private void DeselectAll()
        {
            // 1. UI Kapat
            if (vehiclesHUDController != null) vehiclesHUDController.HideVehicleDetails();
            if (buildingHUDController != null) buildingHUDController.HideBuildingDetails();

            // 2. Outline'ları Kapat
            if (vehicleHighlighter != null)
            {
                vehicleHighlighter.DisableHighlight();
                vehicleHighlighter = null;
            }

            if (targetHighlighter != null)
            {
                targetHighlighter.DisableHighlight();
                targetHighlighter = null;
            }

            if (tempBuildingHighlighter != null)
            {
                tempBuildingHighlighter.DisableHighlight();
                tempBuildingHighlighter = null;
            }

            // 3. Hafızayı Temizle
            currentSelectedVehicle = null;
            currentSelectedBuilding = null;
        }

        private SelectionHighlighter GetHighlighter(GameObject obj)
        {
            var hl = obj.GetComponent<SelectionHighlighter>();
            if (hl == null) hl = obj.GetComponentInParent<SelectionHighlighter>();
            return hl;
        }

        private bool IsPositionValid(Vector3 position)
        {
            foreach (GameObject placedObject in placedObjects)
            {
                if (Vector3.Distance(position, placedObject.transform.position) < minDistanceBetweenObjects)
                {
                    return false;
                }
            }
            return true;
        }
    }
}