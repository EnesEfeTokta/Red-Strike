using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VehicleSystem;
using BuildingPlacement;

namespace InputController
{
    public class InputController : MonoBehaviour
    {
        public LayerMask terrainLayer;
        public LayerMask selectableLayer;
        private Camera mainCamera;
        public BuildingsDatabase buildingsDatabase;
        private Dictionary<string, int> buildingCounts = new Dictionary<string, int>();
        public Button[] createBuildButtons;
        private Building selectedBuilding;
        private List<GameObject> placedObjects = new List<GameObject>();
        public float minDistanceBetweenObjects = 5f;
        public VehiclesDetailManager vehiclesDetailManager;

        private void Start()
        {
            mainCamera = Camera.main;
            foreach (Button button in createBuildButtons)
            {
                button.onClick.AddListener(() => OnCreateBuildingButtonClicked(button));
            }
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

        private void SelectObject()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, selectableLayer))
            {
                VehicleSystem.Vehicles.Vehicle clickedVehicle = hitInfo.collider.GetComponent<VehicleSystem.Vehicles.Vehicle>();

                if (clickedVehicle != null)
                {
                    vehiclesDetailManager.UpdateVehicleDetails(clickedVehicle);
                }
                else
                {
                    vehiclesDetailManager.HideDetailsPanel();
                }
            }
            else
            {
                vehiclesDetailManager.HideDetailsPanel();
            }
        }

        private void OnCreateBuildingButtonClicked(Button button)
        {
            int buttonIndex = System.Array.IndexOf(createBuildButtons, button);
            if (buttonIndex >= 0 && buttonIndex < buildingsDatabase.buildings.Count)
            {
                selectedBuilding = buildingsDatabase.buildings[buttonIndex];
                Debug.Log("Seçilen bina: " + selectedBuilding.buildingName + ". Yerleştirmek için araziye tıklayın.");
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