using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UISystem
{
    public class FirstScreenManager : MonoBehaviour
    {
        public UIDocument uIDocument;

        private void OnEnable()
        {
            var root = uIDocument.rootVisualElement;

            Button playGameButton = root.Q<Button>("play-game-button");
            Button quitButton = root.Q<Button>("quit-button");
            Button infoButton = root.Q<Button>("info-button");

            playGameButton.clicked += PlayGame;
            quitButton.clicked += QuitGame;
            infoButton.clicked += ShowInfo;
        }

        private void PlayGame()
        {
            Debug.Log("Play Game button clicked!");
            SceneManager.LoadScene("GameScene");
        }

        private void QuitGame()
        {
            Debug.Log("Quit Game button clicked!");
            Application.Quit();
        }

        private void ShowInfo()
        {
            Debug.Log("Info button clicked!");
        }
    }
}
