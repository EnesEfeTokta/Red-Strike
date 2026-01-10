using AmmunitionSystem;
using UnityEngine;

namespace VehicleSystem
{
    [System.Serializable]
    public class VehicleAmmunition
    {
        public bool isEnabled;
        public AmmunitionType ammunitionType;
        public int maxAmmunition;
        [Range(0f, 5f)] public float fireRate;
        [Range(0f, 10f)] public float reloadTime;
        public float damage;
        public Ammunition ammunition;
        public AudioClip sound;
    }
}
