using NetworkingSystem;
using UnityEngine;

namespace GameStateSystem
{
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance;

        public enum GameState { MainMenu, InGame, GameOver }
        public GameState CurrentState { get; private set; }

        public int PlayerTeamId = 0;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            CurrentState = GameState.InGame;
        }

        public void GameOver()
        {
            Debug.Log("Oyun Bitti!");
            CurrentState = GameState.GameOver;
            FindAnyObjectByType<UISystem.GameStatusHUDController>().ShowGameOverPanel();
            CommanderData.LocalCommander?.OnDisconnect();
        }
    }
}
