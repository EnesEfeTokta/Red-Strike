using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BuildingPlacement;

namespace BuildingPlacement
{
    public class TerrainClickPlacer : MonoBehaviour
    {
        public LayerMask terrainLayer;
        private Camera mainCamera;
        public BuildingsDatabase buildingsDatabase;
        private Dictionary<string, int> buildingCounts = new Dictionary<string, int>();
        public Button[] createBuildButtons;
        private Building selectedBuilding;
        private List<GameObject> placedObjects = new List<GameObject>();
        public float minDistanceBetweenObjects = 5f;

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
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                RaycastHit hitInfo;

                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, terrainLayer))
                {
                    Vector3 spawnPosition = hitInfo.point;
                    spawnPosition.y = 0;

                    if (selectedBuilding != null && IsPositionValid(spawnPosition))
                    {
                        GameObject prefabToPlace = selectedBuilding.buildingPrefab;

                        if (buildingCounts.ContainsKey(selectedBuilding.buildingName))
                        {
                            if (buildingCounts[selectedBuilding.buildingName] >= selectedBuilding.maxCreatedUnits)
                            {
                                Debug.Log("Max units reached for " + selectedBuilding.buildingName);
                                return;
                            }
                            buildingCounts[selectedBuilding.buildingName]++;
                        }
                        else
                        {
                            buildingCounts[selectedBuilding.buildingName] = 1;
                        }

                        if (prefabToPlace != null)
                        {
                            GameObject placedObject = Instantiate(prefabToPlace, spawnPosition, Quaternion.identity);
                            placedObjects.Add(placedObject);
                        }
                    }
                }
            }
        }

        private void OnCreateBuildingButtonClicked(Button button)
        {
            int buttonIndex = System.Array.IndexOf(createBuildButtons, button);
            if (buttonIndex >= 0 && buttonIndex < buildingsDatabase.buildings.Count)
            {
                selectedBuilding = buildingsDatabase.buildings[buttonIndex];
                Debug.Log("Selected building: " + selectedBuilding.buildingName);
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