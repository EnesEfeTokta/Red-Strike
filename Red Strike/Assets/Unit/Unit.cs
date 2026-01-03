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

        protected NetworkObject networkObject;
        protected NetworkTransform networkTransform;

        public MeshRenderer objectRenderer;

        private void Awake()
        {
            networkObject = GetComponent<NetworkObject>();
            networkTransform = GetComponent<NetworkTransform>();
        }

        public virtual void TakeDamage(float damage) { }

        public virtual void ChangeMaterial(Material newMaterial)
        {
            if (objectRenderer != null && newMaterial != null)
            {
                objectRenderer.material = newMaterial;
            }
        }
    }

    public enum UnitType { Vehicle, Building }
}