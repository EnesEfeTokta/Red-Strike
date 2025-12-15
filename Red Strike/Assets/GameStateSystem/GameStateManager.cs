using Fusion;
using UnityEngine;
using UISystem;

namespace GameStateSystem
{
    public class GameStateManager : NetworkBehaviour
    {
        public static GameStateManager Instance;

        [Networked] public int WinningTeamId { get; set; } = -1;

        public int LocalPlayerTeamId = 0; 

        private ChangeDetector _changes;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public override void Spawned()
        {
            _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            
            if (Object.HasStateAuthority)
            {
                WinningTeamId = -1;
            }
        }

        public override void Render()
        {
            foreach (var change in _changes.DetectChanges(this))
            {
                if (change == nameof(WinningTeamId))
                {
                    HandleGameOverUI();
                }
            }
        }

        public void ReportTeamLoss(int losingTeamId)
        {
            if (Object.HasStateAuthority)
            {
                int winnerId = (losingTeamId == 1) ? 2 : 1;
                WinningTeamId = winnerId;
            }
            else
            {
                RPC_ReportLoss(losingTeamId);
            }
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_ReportLoss(int losingTeamId)
        {
            int winnerId = (losingTeamId == 1) ? 2 : 1;
            WinningTeamId = winnerId;
        }

        private void HandleGameOverUI()
        {
            if (WinningTeamId == -1) return;

            var hud = FindAnyObjectByType<GameStatusHUDController>();
            if (hud == null) return;

            Debug.Log($"Oyun Bitti! Kazanan: {WinningTeamId}, Benim Takımım: {LocalPlayerTeamId}");

            if (WinningTeamId == LocalPlayerTeamId)
            {
                hud.ShowVictoryPanel();
            }
            else
            {
                hud.ShowGameOverPanel();
            }
        }
    }
}