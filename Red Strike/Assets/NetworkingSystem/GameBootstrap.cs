using UnityEngine;
using Fusion;
using System.Threading.Tasks;

namespace NetworkingSystem
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Fusion Ayarları")]
        [SerializeField] private NetworkRunner _networkRunnerPrefab;

        [SerializeField] private int _gameSceneIndex = 1;

        private async Task StartGame(GameMode mode)
        {
            NetworkRunner runner = Instantiate(_networkRunnerPrefab);

            runner.name = "NetworkRunner_Session";
            DontDestroyOnLoad(runner);

            var sceneManager = runner.GetComponent<NetworkSceneManagerDefault>();

            await runner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = "UzaySavasiOda1",
                Scene = SceneRef.FromIndex(_gameSceneIndex),
                SceneManager = sceneManager,
                PlayerCount = 2,
            });
        }

        public async void StartHost()
        {
            await StartGame(GameMode.Host);
        }

        public async void StartClient()
        {
            await StartGame(GameMode.Client);
        }
    }
}
