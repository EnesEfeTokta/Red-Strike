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
        private Building selectedBuilding;
        private List<GameObject> placedObjects = new List<GameObject>();
        public float minDistanceBetweenObjects = 5f;
        public VehiclesHUDController vehiclesHUDController;
        public BuildingHUDController buildingHUDController;

        private SelectionHighlighter currentSelectionHighlighter;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1) && selectedBuilding != null)
            {
                Debug.Log("Bina yerleştirme iptal edildi.");
                selectedBuilding = null;
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOverUI())
                {
                    return;
                }

                if (selectedBuilding != null)
                {
                    PlaceBuilding();
                }
                else
                {
                    SelectObject();
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
                    if (buildingCounts.ContainsKey(selectedBuilding.buildingName) &&
                        buildingCounts[selectedBuilding.buildingName] >= selectedBuilding.maxCreatedUnits)
                    {
                        Debug.Log(selectedBuilding.buildingName + " için maksimum yerleştirme limitine ulaşıldı.");
                        return;
                    }
                    GameObject placedObject = Instantiate(selectedBuilding.buildingPrefab, spawnPosition, Quaternion.identity);

                    if (placedObject.GetComponent<SelectionHighlighter>() == null)
                        placedObject.AddComponent<SelectionHighlighter>();

                    placedObjects.Add(placedObject);

                    if (buildingCounts.ContainsKey(selectedBuilding.buildingName))
                        buildingCounts[selectedBuilding.buildingName]++;
                    else
                        buildingCounts[selectedBuilding.buildingName] = 1;

                    selectedBuilding = null;
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
                selectedBuilding = buildingToSelect;
                Debug.Log("Seçilen bina: " + selectedBuilding.buildingName + ". Yerleştirmek için araziye tıklayın.");
            }
            else
            {
                Debug.LogError(buildingName + " isminde bir bina veritabanında bulunamadı!");
            }
        }

        private void SelectObject()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, selectableLayer))
            {
                DeselectAll();

                currentSelectionHighlighter = hitInfo.collider.GetComponent<SelectionHighlighter>();


                if (currentSelectionHighlighter != null)
                    currentSelectionHighlighter.EnableHighlight();

                switch (hitInfo.collider.tag)
                {
                    case "Build":
                        BuildingPlacement.Buildings.Building clickedBuilding = hitInfo.collider.GetComponent<BuildingPlacement.Buildings.Building>();

                        if (clickedBuilding != null)
                        {
                            buildingHUDController.ShowBuildingDetails(clickedBuilding);
                            if (vehiclesHUDController != null) vehiclesHUDController.HideVehicleDetails();
                        }
                        break;

                    case "Vehicle":
                        Vehicle clickedVehicle = hitInfo.collider.GetComponent<Vehicle>();

                        if (vehiclesHUDController != null) vehiclesHUDController.ShowVehicleDetails(clickedVehicle);
                        buildingHUDController.HideBuildingDetails();
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
            if (vehiclesHUDController != null) vehiclesHUDController.HideVehicleDetails();
            buildingHUDController.HideBuildingDetails();

            if (currentSelectionHighlighter != null)
            {
                currentSelectionHighlighter.DisableHighlight();
                currentSelectionHighlighter = null;
            }
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