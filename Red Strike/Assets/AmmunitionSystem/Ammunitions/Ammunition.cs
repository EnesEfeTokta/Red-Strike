using UnityEngine;

namespace AmmunitionSystem.Ammunitions
{
    public class Ammunition : MonoBehaviour
    {
        public AmmunitionSystem.Ammunition ammunitionData;
        public VehicleSystem.Vehicles.Vehicle ownerVehicle;
        public virtual void SetRocket(Transform targetTransform)
        {
        }
    }
}
