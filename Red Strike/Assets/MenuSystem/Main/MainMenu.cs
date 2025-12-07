using UnityEngine;

namespace MenuSystem.Main
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Sun and Planet Rotation")]
        public Transform sunTransform;
        public float rotationSpeed = 70f;

        public Transform planetTransform;
        public float planetRotationSpeed = 30f;

        private void Update()
        {
            if (sunTransform != null)
            {
                sunTransform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }

            if (planetTransform != null)
            {
                planetTransform.Rotate(Vector3.up, planetRotationSpeed * Time.deltaTime);
            }
        }

        public void OnPlayButtonPressed()
        {
            // Load the main game scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        }

        public void OnOptionsButtonPressed()
        {
            // Load the options menu scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("OptionsMenu");
        }

        public void OnExitButtonPressed()
        {
            // Exit the application
            Application.Quit();
        }
    }
}
