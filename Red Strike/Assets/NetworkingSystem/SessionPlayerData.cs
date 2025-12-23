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
                string myName = "Unknown";

                if (GameBootstrap.Instance != null)
                {
                    myName = GameBootstrap.Instance.LocalPlayerName;
                }

                if (Runner.IsServer)
                {
                    SyncedName = myName;
                }
                else
                {
                    RPC_SetPlayerName(myName);
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

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_SetPlayerName(string name)
        {
            SyncedName = name;
        }

        private void UpdateHUD()
        {
            if (string.IsNullOrEmpty(SyncedName.ToString())) return;

            var dphud = FindFirstObjectByType<DeploymentMonitorHUDController>();
            if (dphud != null)
            {
                int id = (Object.InputAuthority.PlayerId == 1) ? 1 : 2;
                dphud.UpdatePlayerName(id, SyncedName.ToString());
            }

            var vsHud = FindFirstObjectByType<VSPanelHUDController>();
            if (vsHud != null)
            {
                int id = (Object.InputAuthority.PlayerId == 1) ? 1 : 2;
                vsHud.UpdatePlayerName(id, SyncedName.ToString());
            }
        }
    }
}