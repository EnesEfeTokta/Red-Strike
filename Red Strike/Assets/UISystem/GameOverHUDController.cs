using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace UISystem
{
    public class GameOverHUDController : GameHUDController
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            var returnToMenuButton = root.Q<Button>("return-to-menu-button");

            if (returnToMenuButton != null) returnToMenuButton.clicked += () => {StartFadeIn(); Invoke(nameof(GoToMenu), 4.5f); };
        }

        public void ShowGameOverPanel()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.style.display = DisplayStyle.Flex;
                fadePanel.style.opacity = 1f;
            }
        }

        private void GoToMenu()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
