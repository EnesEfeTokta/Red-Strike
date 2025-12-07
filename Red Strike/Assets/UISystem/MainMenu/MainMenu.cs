using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

namespace UISystem.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Sun and Planet Rotation")]
        public Transform sunTransform;
        public float rotationSpeed = 70f;

        public Transform planetTransform;
        public float planetRotationSpeed = 30f;

        [Header("Cinemachine Cameras")]
        public CinemachineCamera cinemachineCamera_A;
        public CinemachineCamera cinemachineCamera_B;
        public CinemachineCamera cinemachineCamera_C;

        public enum MenuState { MainMenu, OptionsMenu, CreditsMenu }
        private MenuState currentMenuState = MenuState.MainMenu;

        private UIDocument document;
        private VisualElement mainMenuContainer;
        private VisualElement quitOverlay;
        private VisualElement settingsOverlay;
        private Slider masterVolumeSlider;
        private Toggle fullscreenToggle;

        private void Awake()
        {
            document = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
            var root = document.rootVisualElement;

            settingsOverlay = root.Q<VisualElement>("settings-overlay");
            var backBtn = root.Q<Button>("back-button");
            var applyBtn = root.Q<Button>("apply-button");
            masterVolumeSlider = root.Q<Slider>("master-volume-slider");
            fullscreenToggle = root.Q<Toggle>("fullscreen-toggle");

            if (backBtn != null) backBtn.clicked += () => BackToMainMenu();
            if (applyBtn != null) applyBtn.clicked += () => ApplySettings();

            mainMenuContainer = root.Q<VisualElement>("menu-container");
            var startBtn = root.Q<Button>("play-button");
            var settingsBtn = root.Q<Button>("options-button");
            var quitBtn = root.Q<Button>("quit-button");

            if (startBtn != null) startBtn.clicked += () => OnPlayButtonPressed();
            if (settingsBtn != null) settingsBtn.clicked += () => OnOptionsButtonPressed();
            if (quitBtn != null) quitBtn.clicked += () => OnExitButtonPressed();

            quitOverlay = root.Q<VisualElement>("quit-overlay");
            var confirmQuitBtn = root.Q<Button>("confirm-quit-button");
            var cancelQuitBtn = root.Q<Button>("cancel-quit-button");

            if (confirmQuitBtn != null) confirmQuitBtn.clicked += () => OnApplicationQuit();
            if (cancelQuitBtn != null) cancelQuitBtn.clicked += () => CanselQuit();
        }

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

            if (currentMenuState == MenuState.MainMenu)
            {
                InputHandler();
            }
        }

        private void InputHandler()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnPlayButtonPressed();
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                OnOptionsButtonPressed();
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                OnExitButtonPressed();
            }
        }

        private void OnPlayButtonPressed()
        {
            // Load the main game scene
            CameraTransition(MenuState.MainMenu);
        }

        private void OnOptionsButtonPressed()
        {
            // Load the options menu scene
            CameraTransition(MenuState.OptionsMenu);
            
            currentMenuState = MenuState.OptionsMenu;

            settingsOverlay.style.display = DisplayStyle.Flex;
            mainMenuContainer.style.display = DisplayStyle.None;
        }

        private void OnExitButtonPressed()
        {
            // Exit the application
            CameraTransition(MenuState.CreditsMenu);

            currentMenuState = MenuState.CreditsMenu;

            quitOverlay.style.display = DisplayStyle.Flex;
            mainMenuContainer.style.display = DisplayStyle.None;
        }

        private void CameraTransition(MenuState targetState)
        {
            switch (targetState)
            {
                case MenuState.MainMenu:
                    SetCameraPriority_MainMenu(10, 0, 0);
                    break;
                case MenuState.OptionsMenu:
                    SetCameraPriority_MainMenu(0, 10, 0);
                    break;
                case MenuState.CreditsMenu:
                    SetCameraPriority_MainMenu(0, 0, 10);
                    break;
            }
        }

        private void SetCameraPriority_MainMenu(int priority_A, int priority_B, int priority_C)
        {
            cinemachineCamera_A.Priority = priority_A;
            cinemachineCamera_B.Priority = priority_B;
            cinemachineCamera_C.Priority = priority_C;
        }

        private void ApplySettings()
        {
            // Apply audio and display settings
            float masterVolume = masterVolumeSlider.value;
            bool isFullscreen = fullscreenToggle.value;

            AudioListener.volume = masterVolume;
            Screen.fullScreen = isFullscreen;

            Debug.Log("Settings applied: Master Volume = " + masterVolume + ", Fullscreen = " + isFullscreen);
        }

        private void BackToMainMenu()
        {
            currentMenuState = MenuState.MainMenu;
            CameraTransition(MenuState.MainMenu);

            settingsOverlay.style.display = DisplayStyle.None;
            mainMenuContainer.style.display = DisplayStyle.Flex;
        }

        private void CanselQuit()
        {
            currentMenuState = MenuState.MainMenu;
            CameraTransition(MenuState.MainMenu);

            quitOverlay.style.display = DisplayStyle.None;
            mainMenuContainer.style.display = DisplayStyle.Flex;
        }

        private void OnApplicationQuit()
        {
            Debug.Log("Application is quitting...");
            Application.Quit();
        }
    }
}
