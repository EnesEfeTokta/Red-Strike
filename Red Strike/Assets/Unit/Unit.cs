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

        [Networked] public int FactionIndex { get; set; }

        protected NetworkObject networkObject;
        protected NetworkTransform networkTransform;
        public MeshRenderer objectRenderer;

        private ChangeDetector changes;

        private void Awake()
        {
            networkObject = GetComponent<NetworkObject>();
            networkTransform = GetComponent<NetworkTransform>();
        }

        public override void Spawned()
        {
            changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            UpdateMaterial();
        }

        public override void Render()
        {
            if (changes == null) return;

            foreach (var change in changes.DetectChanges(this))
            {
                if (change == nameof(FactionIndex))
                {
                    UpdateMaterial();
                }
            }
        }

        public virtual void TakeDamage(float damage) { }

        private void UpdateMaterial()
        {
            if (objectRenderer == null) return;

            var user = InputController.InputController.Instance.userData;

            if (user != null && user.factionSelections != null && FactionIndex >= 0 && FactionIndex < user.factionSelections.Count)
            {
                objectRenderer.material = user.factionSelections[FactionIndex].unitMaterial;
            }
        }
    }

    public enum UnitType { Vehicle, Building }
}