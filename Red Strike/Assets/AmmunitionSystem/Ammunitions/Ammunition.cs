using UnityEngine;
using VehicleSystem.Vehicles;
using Fusion;

namespace AmmunitionSystem.Ammunitions
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(NetworkTransform))]
    public class Ammunition : NetworkBehaviour
    {
        public AmmunitionSystem.Ammunition ammunitionData;

        [Networked] public int OwnerTeamId { get; set; } = -1;
        
        [Networked] public NetworkId OwnerVehicleId { get; set; }

        public virtual void SetRocket(Transform targetTransform) { }
    }
}