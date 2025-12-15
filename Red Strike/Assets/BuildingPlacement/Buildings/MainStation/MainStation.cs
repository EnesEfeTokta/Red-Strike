using UnityEngine;

namespace BuildingPlacement.Buildings
{
    public class MainStation : Building
    {
        public Transform radarTransform;
        public float radarRotationSpeed = 20f;
        public float ShieldAmount = 100f; // TEST

        private void Update()
        {
            radarTransform.Rotate(Vector3.up, radarRotationSpeed * Time.deltaTime);
        }

        public (string, float) GetBuildingStatus()
        {
            return (buildingData.name, health);
        }
    }
}
