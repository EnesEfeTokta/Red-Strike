using UnityEngine;

namespace BuildingPlacement
{
    [CreateAssetMenu(fileName = "NewBuilding", menuName = "Buildings/BuildingsDatabase")]
    public class Building : ScriptableObject
    {
        public string buildingName;
        public GameObject buildingPrefab;
        public int maxHealth;
        public float buildTime;
        public int maxCreatedUnits;
    }
}
