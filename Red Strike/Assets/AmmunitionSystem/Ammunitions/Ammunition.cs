using UnityEngine;
using VehicleSystem.Vehicles;

namespace AmmunitionSystem.Ammunitions
{
    public class Ammunition : MonoBehaviour
    {
        public AmmunitionSystem.Ammunition ammunitionData;
        public Vehicle ownerVehicle;
        public virtual void SetRocket(Transform targetTransform) { }
    }
}
