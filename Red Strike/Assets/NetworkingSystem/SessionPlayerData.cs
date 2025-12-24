using Fusion;
using UISystem;

namespace NetworkingSystem
{
    public class SessionPlayerData : NetworkBehaviour
    {
        [Networked] public NetworkString<_32> SyncedName { get; set; }
        [Networked] public int SyncedAvatarIndex { get; set; }

        private ChangeDetector _changes;

        public override void Spawned()
        {
            _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

            if (Object.HasInputAuthority)
            {
                string myName = "Unknown";
                int myAvatar = 0;

                if (GameBootstrap.Instance != null)
                {
                    myName = GameBootstrap.Instance.LocalPlayerName;
                    myAvatar = GameBootstrap.Instance.LocalAvatarIndex;
                }

                if (Runner.IsServer)
                {
                    SyncedName = myName;
                    SyncedAvatarIndex = myAvatar;
                }
                else
                {
                    RPC_SetPlayerData(myName, myAvatar);
                }
            }

            UpdateHUD();
        }

        public override void Render()
        {
            foreach (var change in _changes.DetectChanges(this))
            {
                if (change == nameof(SyncedName) || change == nameof(SyncedAvatarIndex))
                {
                    UpdateHUD();
                }
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_SetPlayerData(string name, int avatarIndex)
        {
            SyncedName = name;
            SyncedAvatarIndex = avatarIndex;
        }

        private void UpdateHUD()
        {
            if (string.IsNullOrEmpty(SyncedName.ToString())) return;

            var vsHud = FindFirstObjectByType<VSPanelHUDController>();
            if (vsHud != null)
            {
                int id = (Object.InputAuthority.PlayerId == 1) ? 1 : 2;
                
                vsHud.UpdatePlayerName(id, SyncedName.ToString());
                vsHud.UpdatePlayerAvatar(id, SyncedAvatarIndex);
            }
            
            var dphud = FindFirstObjectByType<DeploymentMonitorHUDController>();
            if(dphud != null)
            {
                int id = (Object.InputAuthority.PlayerId == 1) ? 1 : 2;
                dphud.UpdatePlayerName(id, SyncedName.ToString());
            }
        }
    }
}