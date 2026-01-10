using UnityEngine;
using Fusion;

namespace NetworkingSystem
{
    public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
    {
        [Header("Player Prefab")]
        [SerializeField] private NetworkObject _playerCommanderPrefab;

        public void PlayerJoined(PlayerRef player)
        {
            if (Runner.IsServer)
            {
                //Debug.Log($"Oyuncu Katıldı: {player.PlayerId}. Objesi oluşturuluyor...");

                NetworkObject networkPlayer = Runner.Spawn(_playerCommanderPrefab, Vector3.zero, Quaternion.identity, player);

                var commanderData = networkPlayer.GetComponent<CommanderData>();

                int teamIDToAssign = player.PlayerId;

                commanderData.PlayerTeamID = teamIDToAssign;

                //Debug.Log($"Oyuncu {player.PlayerId} için Takım ID {teamIDToAssign} atandı.");
            }
        }
    }
}
