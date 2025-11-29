using UnityEngine;

namespace AmmunitionSystem
{
    [CreateAssetMenu(fileName = "New Ammunition", menuName = "Ammunition System/Ammunition")]
    public class Ammunition : ScriptableObject
    {
        public AmmunitionType ammunitionType;
        public string ammunitionName;
        public float damage;
        public float speed;
        public float range;
        public float lifetime;
        public GameObject ammunitionPrefab;
    }
}
