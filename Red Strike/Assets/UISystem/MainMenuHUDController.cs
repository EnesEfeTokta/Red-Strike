using MainMenuSystem;
using UnityEngine;
using UnityEngine.UIElements;
using UserSystem;

namespace UISystem
{
    public class MainMenuHUDController : MonoBehaviour
    {
        private UIDocument uiDocument;

        private VisualElement root;

        private VisualElement settingsOverlay;
        private VisualElement quitOverlay;

        private VisualElement contentGameplay;
        private VisualElement contentUser;

        private Button btnTabGame;
        private Button btnTabUser;

        private MainMenu mainMenu;

        private void Start()
        {
            mainMenu = GetComponent<MainMenu>();
        }

        private void OnEnable()
        {
            uiDocument = GetComponent<UIDocument>();

            if (uiDocument == null) return;

            root = uiDocument.rootVisualElement;

            settingsOverlay = root.Q<VisualElement>("settings-overlay");
            quitOverlay = root.Q<VisualElement>("quit-overlay");

            if (settingsOverlay != null) settingsOverlay.style.display = DisplayStyle.None;
            if (quitOverlay != null) quitOverlay.style.display = DisplayStyle.None;

            contentGameplay = root.Q<VisualElement>("content-gameplay");
            contentUser = root.Q<VisualElement>("content-user");

            var btnPlay = root.Q<Button>("btn-play");
            var btnOptions = root.Q<Button>("btn-options");
            var btnQuit = root.Q<Button>("btn-quit");

            if (btnPlay != null) btnPlay.clicked += StartGame;
            if (btnOptions != null) btnOptions.clicked += OpenSettings;
            if (btnQuit != null) btnQuit.clicked += OpenQuitPanel;

            var btnCloseSettings = root.Q<Button>("btn-close-settings");
            var btnApplySettings = root.Q<Button>("btn-apply-settings");

            if (btnCloseSettings != null) btnCloseSettings.clicked += CloseSettings;
            if (btnApplySettings != null) btnApplySettings.clicked += ApplySettings;

            btnTabGame = root.Q<Button>("tab-game");
            btnTabUser = root.Q<Button>("tab-user");

            if (btnTabGame != null) btnTabGame.clicked += () => SwitchSettingsTab(true);
            if (btnTabUser != null) btnTabUser.clicked += () => SwitchSettingsTab(false);

            var btnConfirmQuit = root.Q<Button>("btn-confirm-quit");
            var btnCancelQuit = root.Q<Button>("btn-cancel-quit");

            if (btnCancelQuit != null) btnCancelQuit.clicked += CloseQuitPanel;
            if (btnConfirmQuit != null) btnConfirmQuit.clicked += QuitGame;

            var btnUpdateUserName = root.Q<Button>("btn-update-username");
            if (btnUpdateUserName != null) btnUpdateUserName.clicked += () => 
                GetComponent<UserManager>().UpdateProfile(root.Q<TextField>("input-username")?.value ?? "Commander");

            var inputUserName = root.Q<TextField>("input-username");
            if (inputUserName != null) inputUserName.value = GetComponent<UserManager>().currentUser.userName;

            var toggleFullscreen = root.Q<Toggle>("toggle-fullscreen");
            if (toggleFullscreen != null) toggleFullscreen.value = Screen.fullScreen;
            
            var sliderMasterVolume = root.Q<Slider>("slider-master-volume");
            if (sliderMasterVolume != null) sliderMasterVolume.value = mainMenu.settings.masterVolume * 100f;
            var sliderMusicVolume = root.Q<Slider>("slider-music-volume");
            if (sliderMusicVolume != null) sliderMusicVolume.value = mainMenu.settings.musicVolume * 100f;
            var isFullscreen = root.Q<Toggle>("toggle-fullscreen");
            if (isFullscreen != null) isFullscreen.value = mainMenu.settings.isFullscreen;
            
            SwitchSettingsTab(true);
        }

        private void StartGame()
        {
            Debug.Log("OYUN BAŞLATILIYOR...");
            // SceneManager.LoadScene("GameScene");
        }

        private void QuitGame()
        {
            Debug.Log("OYUNDAN ÇIKILIYOR...");
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private void OpenSettings()
        {
            mainMenu?.SwitchToCamera(MainMenu.CameraState.Options);
            if (settingsOverlay != null) settingsOverlay.style.display = DisplayStyle.Flex;
        }

        private void CloseSettings()
        {
            mainMenu?.SwitchToCamera(MainMenu.CameraState.Main);
            if (settingsOverlay != null) settingsOverlay.style.display = DisplayStyle.None;
        }

        private void ApplySettings()
        {
            MainMenu mainMenu = GetComponent<MainMenu>();
            if (mainMenu != null)
            {
                float masterVol = root.Q<Slider>("slider-master-volume")?.value ?? 100f;
                float musicVol = root.Q<Slider>("slider-music-volume")?.value ?? 100f;

                mainMenu.ApplyAudioSettings(masterVol, musicVol);
                
                bool isFullscreen = root.Q<Toggle>("toggle-fullscreen")?.value ?? true;
                mainMenu.ApplyVideoSettings(isFullscreen);
            }
        }

        private void OpenQuitPanel()
        {
            mainMenu?.SwitchToCamera(MainMenu.CameraState.Credits);
            if (quitOverlay != null) quitOverlay.style.display = DisplayStyle.Flex;
        }

        private void CloseQuitPanel()
        {
            mainMenu?.SwitchToCamera(MainMenu.CameraState.Main);
            if (quitOverlay != null) quitOverlay.style.display = DisplayStyle.None;
        }

        private void SwitchSettingsTab(bool showGameplay)
        {
            if (contentGameplay != null)
                contentGameplay.style.display = showGameplay ? DisplayStyle.Flex : DisplayStyle.None;

            if (contentUser != null)
                contentUser.style.display = showGameplay ? DisplayStyle.None : DisplayStyle.Flex;


            if (showGameplay)
            {
                btnTabGame?.AddToClassList("tab-active");
                btnTabUser?.RemoveFromClassList("tab-active");
            }
            else
            {
                btnTabGame?.RemoveFromClassList("tab-active");
                btnTabUser?.AddToClassList("tab-active");
            }
        }
    }
}