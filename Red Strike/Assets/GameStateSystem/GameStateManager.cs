using Fusion;
using UnityEngine;
using UISystem;
using BuildingPlacement.Buildings;
using VehicleSystem.Vehicles;

namespace GameStateSystem
{
    public class GameStateManager : NetworkBehaviour
    {
        public static GameStateManager Instance;

        [Networked] public int WinningTeamId { get; set; } = -1;
        public int LocalPlayerTeamId = 0;

        [Networked, Capacity(32)] 
        private NetworkDictionary<NetworkString<_32>, int> UnitCounts { get; }

        private ChangeDetector _changes;
        private DeploymentMonitorHUDController deploymentMonitorHUDController;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            deploymentMonitorHUDController = GetComponent<DeploymentMonitorHUDController>();
        }

        public override void Spawned()
        {
            _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

            if (Object.HasStateAuthority)
            {
                WinningTeamId = -1;
                UnitCounts.Clear();
            }
            
            UpdateAllUI();
        }

        public override void Render()
        {
            foreach (var change in _changes.DetectChanges(this))
            {
                if (change == nameof(WinningTeamId))
                {
                    HandleGameOverUI();
                }
                
                if (change == nameof(UnitCounts))
                {
                    UpdateAllUI();
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

            if (WinningTeamId == LocalPlayerTeamId) hud.ShowVictoryPanel();
            else hud.ShowGameOverPanel();
        }

        public void ReportUnitConstructed(Unit.Unit unit)
        {
            if (Object.HasStateAuthority)
            {
                ModifyUnitCount(unit, 1);
            }
            else
            {
                int maxCap = (unit is Building) ? ((Building)unit).buildingData.maxCreatableCount : (unit is Vehicle) ? ((Vehicle)unit).vehicleData.maxCreatableCount : 0;
                RPC_ModifyUnitCount(unit.teamId, GetUnitName(unit), maxCap, 1);
            }
        }

        public void ReportUnitDestroyed(Unit.Unit unit)
        {
            if (Object.HasStateAuthority)
            {
                ModifyUnitCount(unit, -1);
            }
            else
            {
                RPC_ModifyUnitCount(unit.teamId, GetUnitName(unit), GetMaxCapacity(unit), -1);
            }
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_ModifyUnitCount(int teamId, string unitName, int maxCapacity, int changeAmount)
        {
            string key = $"{teamId}_{unitName}";
            
            int current = 0;
            if (UnitCounts.ContainsKey(key))
            {
                current = UnitCounts[key];
            }

            int newValue = Mathf.Clamp(current + changeAmount, 0, maxCapacity);
            UnitCounts.Set(key, newValue);
        }

        private void ModifyUnitCount(Unit.Unit unit, int amount)
        {
            string unitName = GetUnitName(unit);
            string key = $"{unit.teamId}_{unitName}";

            int current = 0;
            if (UnitCounts.ContainsKey(key))
            {
                current = UnitCounts[key];
            }

            int newValue = Mathf.Clamp(current + amount, 0, GetMaxCapacity(unit));
            UnitCounts.Set(key, newValue);
        }

        private void UpdateAllUI()
        {
            if (deploymentMonitorHUDController == null) return;

            foreach (var kvp in UnitCounts)
            {
                string[] parts = kvp.Key.ToString().Split('_');
                if (parts.Length < 2) continue;

                if (int.TryParse(parts[0], out int teamId))
                {
                    string unitName = parts[1];
                    int count = kvp.Value;
                    int maxCap = 10; // Maksimum kapasiteyi bilmediğimiz için 10 olarak başlatıyoruz

                    deploymentMonitorHUDController.UpdateUnitSlots(teamId, unitName, count, maxCap);
                }
            }
        }

        private string GetUnitName(Unit.Unit unit)
        {
            if (unit is Building b) return b.buildingData.buildingName;
            if (unit is Vehicle v) return v.vehicleData.vehicleName;
            return "Unknown";
        }

        private int GetMaxCapacity(Unit.Unit unit)
        {
            if (unit is Building b) return b.buildingData.maxCreatableCount;
            if (unit is Vehicle v) return v.vehicleData.maxCreatableCount;
            return 0;
        }
    }
}