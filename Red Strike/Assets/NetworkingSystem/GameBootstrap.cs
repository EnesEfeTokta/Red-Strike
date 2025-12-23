using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;

namespace NetworkingSystem
{
    public class GameBootstrap : MonoBehaviour, INetworkRunnerCallbacks
    {
        public static GameBootstrap Instance;

        public string LocalPlayerName { get; set; } = "Unknown";

        [Header("Ayarlar")]
        public NetworkRunner networkRunnerPrefab;
        public NetworkPrefabRef playerDataPrefab;
        public int gameSceneIndex = 1;

        private NetworkRunner _runner;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public async void StartGame(GameMode mode, string sessionName)
        {
            if (_runner != null) Destroy(_runner.gameObject);

            _runner = Instantiate(networkRunnerPrefab);
            _runner.name = "FusionRunner";
            DontDestroyOnLoad(_runner);
            _runner.AddCallbacks(this);

            var sceneManager = _runner.GetComponent<NetworkSceneManagerDefault>();
            if (sceneManager == null) sceneManager = _runner.gameObject.AddComponent<NetworkSceneManagerDefault>();

            await _runner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = sessionName,
                Scene = SceneRef.FromIndex(gameSceneIndex),
                SceneManager = sceneManager,
                PlayerCount = 2,
            });
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                runner.Spawn(playerDataPrefab, Vector3.zero, Quaternion.identity, player);
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
#pragma warning disable UNT0006
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
#pragma warning restore UNT0006
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    }
}