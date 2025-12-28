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
        public float reloadTime;
        public Ammunition ammunition;
        public AudioClip sound;
    }
}
