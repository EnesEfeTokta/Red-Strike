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

        private SelectionHighlighter currentSelectionHighlighter;

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

        private void SelectObject()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, selectableLayer))
            {
                GameObject hitObject = hitInfo.collider.gameObject;

                if (currentSelectedVehicle != null && hitObject.CompareTag("Enemy"))
                {
                    Debug.Log($"Araç ({currentSelectedVehicle.name}) hedefe ({hitObject.name}) gönderiliyor.");
                    currentSelectedVehicle.SetTargetEnemy(hitObject);
                    DeselectAll();

                    return;
                }

                DeselectAll();

                currentSelectionHighlighter = hitObject.GetComponent<SelectionHighlighter>();
                if (currentSelectionHighlighter == null) currentSelectionHighlighter = hitObject.GetComponentInParent<SelectionHighlighter>();

                switch (hitObject.tag)
                {
                    case "Vehicle":
                        if (currentSelectionHighlighter != null) currentSelectionHighlighter.EnableHighlight();

                        Vehicle v = hitObject.GetComponent<Vehicle>();
                        if (v != null)
                        {
                            currentSelectedVehicle = v;
                            if (vehiclesHUDController != null) vehiclesHUDController.ShowVehicleDetails(v);
                        }
                        break;

                    case "Build":
                        if (currentSelectionHighlighter != null) currentSelectionHighlighter.EnableHighlight();

                        var b = hitObject.GetComponent<BuildingPlacement.Buildings.Building>();
                        if (b != null) buildingHUDController.ShowBuildingDetails(b);
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
            // UI Gizle 
            if (vehiclesHUDController != null) vehiclesHUDController.HideVehicleDetails();
            if (buildingHUDController != null) buildingHUDController.HideBuildingDetails();

            // Outline Kapat
            if (currentSelectionHighlighter != null)
            {
                currentSelectionHighlighter.DisableHighlight();
                currentSelectionHighlighter = null;
            }

            // Hafızayı temizle
            currentSelectedVehicle = null;
            currentSelectedBuilding = null;
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