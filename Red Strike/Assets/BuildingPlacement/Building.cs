using UnityEngine;

namespace BuildingPlacement
{
    [CreateAssetMenu(fileName = "NewBuilding", menuName = "Buildings/BuildingsDatabase")]
    public class Building : ScriptableObject
    {
        [Header("Building Settings")]
        public string buildingName;
        public GameObject buildingPrefab;
        public int maxHealth;
        public float buildTime;
        public int maxCreatableCount;
        public float heightOffset;

        public Sprite buildingIcon;

        [Header("Effects")]
        public ParticleSystem explosionEffect;

    }
}
