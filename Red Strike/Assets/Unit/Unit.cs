using UnityEngine;
using Fusion;

namespace Unit
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(NetworkTransform))]
    public class Unit : NetworkBehaviour
    {
        [Header("Unit Info")]
        [Networked] public int teamId { get; set; }
        public UnitType unitType;

        public virtual void TakeDamage(float damage)
        {
            if (!Object.HasStateAuthority) return;

            // Implement damage logic here
        }

        protected virtual void OnDestroy()
        {
            if (!Object.HasStateAuthority) return;

            Debug.Log($"Unit of type {unitType} from team {teamId} is being destroyed.");
            Runner.Despawn(Object);
        }
    }

    public enum UnitType { Vehicle, Building }
}
