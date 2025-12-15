using UnityEngine;
using VehicleSystem.Vehicles;
using Fusion;
using System.Security.Cryptography;

namespace AmmunitionSystem.Ammunitions
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(NetworkTransform))]
    public class Ammunition : NetworkBehaviour
    {
        public AmmunitionSystem.Ammunition ammunitionData;
        public Vehicle ownerVehicle;
        public virtual void SetRocket(Transform targetTransform) { }

        protected virtual void Start()
        {
            Invoke(nameof(OnDestroy), ammunitionData.lifetime);
        }

        protected virtual void OnDestroy()
        {
            Runner.Despawn(Object);
        }
    }
}
