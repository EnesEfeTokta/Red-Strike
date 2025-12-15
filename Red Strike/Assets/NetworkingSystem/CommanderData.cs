using UnityEngine;
using Fusion;
using System.Linq;

namespace NetworkingSystem
{
    public class CommanderData : NetworkBehaviour
    {
        public static CommanderData LocalCommander;

        [Networked]
        public int PlayerTeamID { get; set; }

        private ChangeDetector _changeDetector;

        public override void Spawned()
        {
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

            if (Object.HasInputAuthority)
            {
                Debug.Log($"Benim Komutan objem yüklendi! (Spawned)");
                LocalCommander = this;
            }
            else
            {
                Debug.Log($"Rakibin Komutan objesi yüklendi.");
            }
        }

        public override void Render()
        {
            foreach (var change in _changeDetector.DetectChanges(this))
            {
                if (change == nameof(PlayerTeamID))
                {
                    OnTeamIdChanged();
                }
            }

            if (Object.HasInputAuthority)
            {
                if (InputController.InputController.Instance != null)
                {
                    if (PlayerTeamID != 0 && InputController.InputController.Instance.teamId != PlayerTeamID)
                    {
                        InputController.InputController.Instance.teamId = PlayerTeamID;
                        GameStateSystem.GameStateManager.Instance.PlayerTeamId = PlayerTeamID;
                        Debug.Log($"<color=yellow>ZORLA EŞİTLEME:</color> InputController ve GameStateManager ID'leri {PlayerTeamID} olarak düzeltildi.");
                    }
                }
            }
        }

        private void OnTeamIdChanged()
        {
            if (Object.HasInputAuthority)
            {
                if (InputController.InputController.Instance != null && PlayerTeamID != 0)
                {
                    InputController.InputController.Instance.teamId = PlayerTeamID;
                    GameStateSystem.GameStateManager.Instance.PlayerTeamId = PlayerTeamID;
                    Debug.Log($"<color=green>GÜNCELLEME:</color> Takım ID'si {PlayerTeamID} olarak değişti ve ayarlandı!");
                }
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_SpawnBuilding(string buildingName, Vector3 position)
        {
            Debug.Log($"Server: {Object.InputAuthority} oyuncusu {buildingName} kurmak istiyor.");

            if (InputController.InputController.Instance == null) return;

            var database = InputController.InputController.Instance.buildingsDatabase;
            var buildingData = database.buildings.FirstOrDefault(b => b.buildingName == buildingName);

            if (buildingData != null && buildingData.buildingPrefab != null)
            {
                NetworkObject spawnedObj = Runner.Spawn(buildingData.buildingPrefab, position, Quaternion.identity, Object.InputAuthority);

                var unitScript = spawnedObj.GetComponent<Unit.Unit>();
                if (unitScript != null)
                {
                    unitScript.teamId = PlayerTeamID;
                    Debug.Log($"Bina kuruldu ({buildingName}). Takım ID: {PlayerTeamID} atandı.");
                }
            }
            else
            {
                Debug.LogError($"Server: {buildingName} prefabı bulunamadı!");
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_SpawnVehicle(string vehicleName, Vector3 position)
        {
            Debug.Log($"Server: {Object.InputAuthority} oyuncusu {vehicleName} üretmek istiyor.");

            if (InputController.InputController.Instance == null) return;

            var database = InputController.InputController.Instance.vehiclesDatabase;
            var vehicleData = database.vehicles.FirstOrDefault(v => v.vehicleName == vehicleName);

            if (vehicleData != null && vehicleData.vehiclePrefab != null)
            {
                NetworkObject spawnedObj = Runner.Spawn(vehicleData.vehiclePrefab, position, Quaternion.identity, Object.InputAuthority);

                var unitScript = spawnedObj.GetComponent<Unit.Unit>();
                if (unitScript != null)
                {
                    unitScript.teamId = PlayerTeamID;
                    Debug.Log($"Araç üretildi ({vehicleData.vehicleName}). Takım ID: {PlayerTeamID} atandı.");
                }
            }
            else
            {
                Debug.LogError($"Server: {vehicleData.vehicleName} prefabı bulunamadı!");
            }
        }

        public void OnDisconnect()
        {
            if (Runner != null && Runner.IsRunning)
            {
                Debug.Log($"Oyuncu oyundan çıkarılıyor. Takım ID: {PlayerTeamID}");
                Runner.Shutdown();
            }
        }
    }
}