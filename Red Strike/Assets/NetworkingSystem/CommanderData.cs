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
                //Debug.Log($"Benim Komutan objem yüklendi! (Spawned)");
                LocalCommander = this;
            }
            else
            {
                //Debug.Log($"Rakibin Komutan objesi yüklendi.");
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
                        GameStateSystem.GameStateManager.Instance.LocalPlayerTeamId = PlayerTeamID;
                        //Debug.Log($"<color=yellow>ZORLA EŞİTLEME:</color> InputController ve GameStateManager ID'leri {PlayerTeamID} olarak düzeltildi.");
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
                    GameStateSystem.GameStateManager.Instance.LocalPlayerTeamId = PlayerTeamID;
                    //Debug.Log($"<color=green>GÜNCELLEME:</color> Takım ID'si {PlayerTeamID} olarak değişti ve ayarlandı!");
                }
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_SpawnBuilding(string buildingName, Vector3 position)
        {
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
                    unitScript.ChangeMaterial(InputController.InputController.Instance.userData.selectedFaction.unitMaterial);
                    GameStateSystem.GameStateManager.Instance.ReportUnitConstructed(unitScript);
                    //Debug.Log($"Bina kuruldu ({buildingName}). Takım ID: {PlayerTeamID} atandı.");
                }
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_SpawnVehicle(string vehicleName, Vector3 position)
        {
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
                    GameStateSystem.GameStateManager.Instance.ReportUnitConstructed(unitScript);
                    unitScript.ChangeMaterial(InputController.InputController.Instance.userData.selectedFaction.unitMaterial);
                    //Debug.Log($"Araç üretildi ({vehicleData.vehicleName}). Takım ID: {PlayerTeamID} atandı.");
                }
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_SpawnAmmunition(
            string ammunitionName,
            Vector3 position,
            Quaternion rotation,
            float damage,
            NetworkObject ownerVehicleNetObj,
            NetworkId targetId = default)
        {
            if (Runner == null || !Runner.IsServer) return;

            if (InputController.InputController.Instance == null) return;

            var database = InputController.InputController.Instance.ammunitionDatabase;
            var ammunitionData = database.ammunitions.FirstOrDefault(a => a.ammunitionName == ammunitionName);

            if (ammunitionData != null && ammunitionData.ammunitionPrefab != null)
            {
                NetworkObject spawnedObj = Runner.Spawn(ammunitionData.ammunitionPrefab, position, rotation, Object.InputAuthority);
                var ammunitionScript = spawnedObj.GetComponent<AmmunitionSystem.Ammunitions.Ammunition>();
                var vehicleScript = ownerVehicleNetObj.GetComponent<VehicleSystem.Vehicles.Vehicle>();

                if (ammunitionScript != null && vehicleScript != null)
                {
                    ammunitionScript.OwnerTeamId = vehicleScript.teamId;
                    ammunitionScript.OwnerVehicleId = ownerVehicleNetObj.Id;

                    ammunitionScript.damage = damage;

                    if (ammunitionData.ammunitionType == AmmunitionSystem.AmmunitionType.Rocket && targetId.IsValid)
                    {
                        ammunitionScript.SetRocketTarget(targetId);
                    }

                    //Debug.Log($"Mühimmat fırlatıldı: {ammunitionName}. Takım: {vehicleScript.teamId}");
                }
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_SpawnExplosionEffect(Vector3 position)
        {
            if (InputController.InputController.Instance == null) return;

            var explosionEffectPrefab = InputController.InputController.Instance.ammunitionDatabase.ammunitions.
                FirstOrDefault(e => e.ammunitionType == AmmunitionSystem.AmmunitionType.Rocket)?.explosionEffectPrefab;

            if (explosionEffectPrefab != null)
            {
                GameObject effect = Instantiate(explosionEffectPrefab, position, Quaternion.identity);

                Destroy(effect, 3f);
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