using Fusion;
using UnityEngine;
using UISystem;

namespace NetworkingSystem
{
    public class SessionPlayerData : NetworkBehaviour
    {
        [Networked] public NetworkString<_32> SyncedName { get; set; }
        
        private ChangeDetector _changes;

        public override void Spawned()
        {
            _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

            if (Object.HasInputAuthority)
            {
                if (GameBootstrap.Instance != null)
                {
                    SyncedName = GameBootstrap.Instance.LocalPlayerName;
                }
                else
                {
                    SyncedName = $"Commander {Object.InputAuthority.PlayerId}";
                }
            }

            UpdateHUD();
        }

        public override void Render()
        {
            foreach (var change in _changes.DetectChanges(this))
            {
                if (change == nameof(SyncedName))
                {
                    UpdateHUD();
                }
            }
        }

        private void UpdateHUD()
        {
            var hud = FindFirstObjectByType<DeploymentMonitorHUDController>();
            
            if (hud != null)
            {
                int playerIndex = (Object.InputAuthority.PlayerId == 1) ?  1 : 2;
                hud.UpdatePlayerName(playerIndex, SyncedName.ToString());
            }
        }
    }
}