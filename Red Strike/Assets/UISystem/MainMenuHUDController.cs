using System.Collections;
using MainMenuSystem;
using UnityEngine;
using UnityEngine.UIElements;
using UserSystem;
using NetworkingSystem;
using Fusion;

namespace UISystem
{
    public class MainMenuHUDController : MonoBehaviour
    {
        private UIDocument uiDocument;
        private MainMenu mainMenu;
        private UserManager userManager;

        // UI Root
        private VisualElement root;

        // Overlayler
        private VisualElement mainMenuOverlay;
        private VisualElement settingsOverlay;
        private VisualElement quitOverlay;
        private VisualElement loginOverlay;
        private VisualElement lobbyOverlay;
        private VisualElement fadePanel;

        // İçerikler
        private VisualElement contentGameplay;
        private VisualElement contentUser;

        // Elementler
        private TextField inputUserNameLogin;
        private TextField inputUserName;
        private TextField inputSessionName;
        private Label lobbyStatusLabel;
        private Image avatarImage;
        private Image factionIcon;
        private Label factionNameLabel;
        private Label factionDescLabel;

        // Buton Referansları
        private Button btnPlay, btnOptions, btnQuit;
        private Button btnCloseSettings, btnApplySettings, btnUpdateUsername;
        private Button btnCloseLobby, btnStartHost, btnStartClient;
        private Button btnTabGame, btnTabUser;
        private Button btnConfirmQuit, btnCancelQuit;
        private Button btnLogin;
        private Button btnAvatarNext, btnAvatarPrev;
        private Button btnFactionNext, btnFactionPrev;

        private void Awake()
        {
            if (!uiDocument) uiDocument = GetComponent<UIDocument>();
            mainMenu = GetComponent<MainMenu>();
            userManager = GetComponent<UserManager>();
        }

        private void OnEnable()
        {
            if (uiDocument == null) return;
            root = uiDocument.rootVisualElement;

            BindUIElements();

            RegisterEvents();

            SetDefaultValues();
        }

        private void OnDisable()
        {
            UnregisterEvents();
        }

        private IEnumerator Start()
        {
            yield return null;

            HidePanel(settingsOverlay);
            HidePanel(quitOverlay);
            HidePanel(lobbyOverlay);

            if (userManager != null)
            {
                if (!userManager.IsLoggedIn())
                {
                    ShowPanel(loginOverlay);
                    HidePanel(mainMenuOverlay);
                    mainMenu?.SwitchToCamera(MainMenu.CameraState.Login);
                }
                else
                {
                    HidePanel(loginOverlay);
                    ShowPanel(mainMenuOverlay);

                    if (inputUserName != null)
                        inputUserName.value = userManager.currentUser.userName ?? "";

                    mainMenu?.SwitchToCamera(MainMenu.CameraState.Main);
                }
            }

            SwitchSettingsTab(true);
        }

        #region UI BINDING AND EVENTS

        private void BindUIElements()
        {
            // Paneller
            mainMenuOverlay = root.Q<VisualElement>("main-menu-container");
            settingsOverlay = root.Q<VisualElement>("settings-overlay");
            quitOverlay = root.Q<VisualElement>("quit-overlay");
            loginOverlay = root.Q<VisualElement>("login-overlay");
            lobbyOverlay = root.Q<VisualElement>("lobby-overlay");
            fadePanel = root.Q<VisualElement>("fade-panel");

            contentGameplay = root.Q<VisualElement>("content-gameplay");
            contentUser = root.Q<VisualElement>("content-user");

            // Elementler
            inputUserNameLogin = root.Q<TextField>("input-username-login");
            inputUserName = root.Q<TextField>("input-username");
            inputSessionName = root.Q<TextField>("input-session-name");
            lobbyStatusLabel = root.Q<Label>("lobby-status-label");
            avatarImage = root.Q<Image>("current-avatar-image");
            factionIcon = root.Q<Image>("faction-icon");
            factionNameLabel = root.Q<Label>("faction-name-label");
            factionDescLabel = root.Q<Label>("faction-desc-label");

            // Butonlar
            btnPlay = root.Q<Button>("btn-play");
            btnOptions = root.Q<Button>("btn-options");
            btnQuit = root.Q<Button>("btn-quit");

            btnCloseSettings = root.Q<Button>("btn-close-settings");
            btnApplySettings = root.Q<Button>("btn-apply-settings");
            btnUpdateUsername = root.Q<Button>("btn-update-username");

            btnCloseLobby = root.Q<Button>("btn-close-lobby");
            btnStartHost = root.Q<Button>("btn-start-host");
            btnStartClient = root.Q<Button>("btn-start-client");

            btnTabGame = root.Q<Button>("tab-game");
            btnTabUser = root.Q<Button>("tab-user");

            btnConfirmQuit = root.Q<Button>("btn-confirm-quit");
            btnCancelQuit = root.Q<Button>("btn-cancel-quit");

            btnLogin = root.Q<Button>("btn-login");

            btnAvatarNext = root.Q<Button>("btn-avatar-next");
            btnAvatarPrev = root.Q<Button>("btn-avatar-prev");

            btnFactionNext = root.Q<Button>("btn-faction-next");
            btnFactionPrev = root.Q<Button>("btn-faction-prev");
        }

        private void RegisterEvents()
        {
            if (btnPlay != null) btnPlay.clicked += OpenLobby;
            if (btnOptions != null) btnOptions.clicked += OpenSettings;
            if (btnQuit != null) btnQuit.clicked += OpenQuitPanel;

            if (btnCloseSettings != null) btnCloseSettings.clicked += CloseSettings;
            if (btnApplySettings != null) btnApplySettings.clicked += ApplySettings;
            if (btnUpdateUsername != null) btnUpdateUsername.clicked += OnUpdateUserNameClicked;

            if (btnCloseLobby != null) btnCloseLobby.clicked += CloseLobby;
            if (btnStartHost != null) btnStartHost.clicked += OnStartHostClicked;
            if (btnStartClient != null) btnStartClient.clicked += OnStartClientClicked;

            if (btnTabGame != null) btnTabGame.clicked += () => SwitchSettingsTab(true);
            if (btnTabUser != null) btnTabUser.clicked += () => SwitchSettingsTab(false);

            if (btnCancelQuit != null) btnCancelQuit.clicked += CloseQuitPanel;
            if (btnConfirmQuit != null) btnConfirmQuit.clicked += QuitGame;

            if (btnLogin != null) btnLogin.clicked += OnLoginClicked;

            if (btnAvatarNext != null) btnAvatarNext.clicked += OnAvatarNext;
            if (btnAvatarPrev != null) btnAvatarPrev.clicked += OnAvatarPrev;

            if (btnFactionNext != null) btnFactionNext.clicked += OnFactionNext;
            if (btnFactionPrev != null) btnFactionPrev.clicked += OnFactionPrev;
        }

        private void UnregisterEvents()
        {
            if (btnPlay != null) btnPlay.clicked -= OpenLobby;
            if (btnOptions != null) btnOptions.clicked -= OpenSettings;
            if (btnQuit != null) btnQuit.clicked -= OpenQuitPanel;

            if (btnCloseSettings != null) btnCloseSettings.clicked -= CloseSettings;
            if (btnApplySettings != null) btnApplySettings.clicked -= ApplySettings;
            if (btnUpdateUsername != null) btnUpdateUsername.clicked -= OnUpdateUserNameClicked;

            if (btnCloseLobby != null) btnCloseLobby.clicked -= CloseLobby;
            if (btnStartHost != null) btnStartHost.clicked -= OnStartHostClicked;
            if (btnStartClient != null) btnStartClient.clicked -= OnStartClientClicked;

            if (btnCancelQuit != null) btnCancelQuit.clicked -= CloseQuitPanel;
            if (btnConfirmQuit != null) btnConfirmQuit.clicked -= QuitGame;

            if (btnLogin != null) btnLogin.clicked -= OnLoginClicked;

            if (btnAvatarNext != null) btnAvatarNext.clicked -= OnAvatarNext;
            if (btnAvatarPrev != null) btnAvatarPrev.clicked -= OnAvatarPrev;

            if (btnFactionNext != null) btnFactionNext.clicked -= OnFactionNext;
            if (btnFactionPrev != null) btnFactionPrev.clicked -= OnFactionPrev;
        }

        private void SetDefaultValues()
        {
            if (userManager == null || userManager.currentUser == null) return;

            if (avatarImage != null && userManager.currentUser.avatar != null)
            {
                avatarImage.sprite = userManager.currentUser.avatar;
            }

            UpdateFactionUI();
        }

        #endregion

        #region HELPER METHODS

        private void ShowPanel(VisualElement panel)
        {
            if (panel != null)
                panel.style.display = DisplayStyle.Flex;

            mainMenu?.PlaySound(MainMenu.AudioClipType.ButtonClick1);
        }

        private void HidePanel(VisualElement panel)
        {
            if (panel != null)
                panel.style.display = DisplayStyle.None;

            mainMenu?.PlaySound(MainMenu.AudioClipType.ButtonClick2);
        }

        private void OnAvatarNext() => OnAvatarChanged(AvatarChangeDirection.Next);
        private void OnAvatarPrev() => OnAvatarChanged(AvatarChangeDirection.Previous);

        private void OnFactionNext() => OnFactionChanged(FactionSelectionDirection.Next);
        private void OnFactionPrev() => OnFactionChanged(FactionSelectionDirection.Previous);

        #endregion

        #region LOGIC METHODS

        private void OnLoginClicked()
        {
            string newName = inputUserNameLogin?.value;
            if (string.IsNullOrEmpty(newName)) return;

            userManager?.LogIn(newName);

            HidePanel(loginOverlay);
            ShowPanel(mainMenuOverlay);
            mainMenu?.SwitchToCamera(MainMenu.CameraState.Main);

            if (inputUserName != null) inputUserName.value = newName;
        }

        private void OnUpdateUserNameClicked()
        {
            string newName = inputUserName?.value;
            if (!string.IsNullOrEmpty(newName)) userManager?.UpdateUserName(newName);
        }

        private void OpenLobby()
        {
            ShowPanel(lobbyOverlay);
            HidePanel(mainMenuOverlay);
            SetLobbyStatus("READY", Color.green);
        }

        private void CloseLobby()
        {
            HidePanel(lobbyOverlay);
            ShowPanel(mainMenuOverlay);
        }

        private void OpenSettings()
        {
            mainMenu?.SwitchToCamera(MainMenu.CameraState.Options);
            ShowPanel(settingsOverlay);
            HidePanel(mainMenuOverlay);
        }

        private void CloseSettings()
        {
            mainMenu?.SwitchToCamera(MainMenu.CameraState.Main);
            HidePanel(settingsOverlay);
            ShowPanel(mainMenuOverlay);
        }

        private void OpenQuitPanel()
        {
            mainMenu?.SwitchToCamera(MainMenu.CameraState.Credits);
            ShowPanel(quitOverlay);
            HidePanel(mainMenuOverlay);
        }

        private void CloseQuitPanel()
        {
            mainMenu?.SwitchToCamera(MainMenu.CameraState.Main);
            HidePanel(quitOverlay);
            ShowPanel(mainMenuOverlay);
        }

        private void ApplySettings()
        {
            mainMenu?.PlaySound(MainMenu.AudioClipType.ButtonClick1);
            if (mainMenu == null) return;
            float master = root.Q<Slider>("slider-master-volume")?.value ?? 100;
            float music = root.Q<Slider>("slider-music-volume")?.value ?? 100;
            bool full = root.Q<Toggle>("toggle-fullscreen")?.value ?? true;

            mainMenu.ApplyAudioSettings(master, music);
            mainMenu.ApplyVideoSettings(full);
        }

        private void OnStartHostClicked()
        {
            mainMenu?.PlaySound(MainMenu.AudioClipType.ButtonClick1);
            string session = inputSessionName?.value;
            if (string.IsNullOrEmpty(session))
            {
                SetLobbyStatus("INVALID SESSION ID", Color.red); return;
            }
            SetLobbyStatus("INITIALIZING HOST...", Color.yellow);
            StartCoroutine(StartGameSequence(GameMode.Host, session));
        }

        private void OnStartClientClicked()
        {
            mainMenu?.PlaySound(MainMenu.AudioClipType.ButtonClick1);
            string session = inputSessionName?.value;
            if (!string.IsNullOrEmpty(session))
            {
                SetLobbyStatus("SEARCHING UPLINK...", Color.yellow);
            }
            SetLobbyStatus("PREPARING FOR UPLINK...", Color.yellow);
            StartCoroutine(StartGameSequence(GameMode.Client, session));
        }

        private IEnumerator StartGameSequence(GameMode mode, string sessionName)
        {
            if (GameBootstrap.Instance != null && userManager != null)
            {
                GameBootstrap.Instance.LocalPlayerName = userManager.currentUser.userName;
            }

            yield return new WaitForSeconds(1f);
            HidePanel(lobbyOverlay);
            HidePanel(mainMenuOverlay);

            if (mainMenu != null)
            {
                mainMenu.StartCoroutine(mainMenu.LaunchSequence());
            }

            float waitTime = (mainMenu != null) ? mainMenu.shipDuration : 5f;

            if (fadePanel != null)
            {
                fadePanel.style.opacity = 0;
                fadePanel.style.display = DisplayStyle.Flex;

                yield return null;

                fadePanel.style.opacity = 1;
            }

            float timer = 0;
            while (timer < waitTime)
            {
                timer += Time.deltaTime;

                if (fadePanel != null && timer > waitTime - 1f)
                {
                    fadePanel.style.opacity = (timer - (waitTime - 1f));
                }

                yield return null;
            }

            if (mode == GameMode.Host)
            {
                GameBootstrap.Instance?.StartHost(sessionName);
            }
            else
            {
                GameBootstrap.Instance?.StartClient(sessionName);
            }
        }

        private void SetLobbyStatus(string msg, Color color)
        {
            if (lobbyStatusLabel != null)
            {
                lobbyStatusLabel.text = msg;
                lobbyStatusLabel.style.color = color;
            }
        }

        private void SwitchSettingsTab(bool showGame)
        {
            mainMenu?.PlaySound(MainMenu.AudioClipType.ButtonClick1);
            if (contentGameplay != null) contentGameplay.style.display = showGame ? DisplayStyle.Flex : DisplayStyle.None;
            if (contentUser != null) contentUser.style.display = showGame ? DisplayStyle.None : DisplayStyle.Flex;

            if (showGame)
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

        public enum AvatarChangeDirection { Previous, Next }
        private void OnAvatarChanged(AvatarChangeDirection direction)
        {
            mainMenu?.PlaySound(MainMenu.AudioClipType.ButtonClick2);

            if (userManager == null || userManager.currentUser == null || avatarImage == null) return;

            int currentIndex = System.Array.IndexOf(userManager.currentUser.availableAvatars, userManager.currentUser.avatar);
            if (currentIndex < 0) currentIndex = 0;

            int newIndex = currentIndex;
            if (direction == AvatarChangeDirection.Next)
                newIndex = (currentIndex + 1) % userManager.currentUser.availableAvatars.Length;
            else
                newIndex = (currentIndex - 1 + userManager.currentUser.availableAvatars.Length) % userManager.currentUser.availableAvatars.Length;

            userManager.UpdateAvatar(newIndex);
            avatarImage.sprite = userManager.currentUser.avatar;
            avatarImage.MarkDirtyRepaint();
        }

        public enum FactionSelectionDirection { Previous, Next }

        private void OnFactionChanged(FactionSelectionDirection direction)
        {
            mainMenu?.PlaySound(MainMenu.AudioClipType.ButtonClick2);

            if (userManager == null || userManager.currentUser == null) return;
            if (userManager.currentUser.factionSelections == null || userManager.currentUser.factionSelections.Count == 0) return;

            int currentIndex = userManager.GetCurrentFactionIndex();
            int count = userManager.currentUser.factionSelections.Count;

            int newIndex = currentIndex;
            if (direction == FactionSelectionDirection.Next)
                newIndex = (currentIndex + 1) % count;
            else
                newIndex = (currentIndex - 1 + count) % count;

            userManager.UpdateFactionSelection(newIndex);
            UpdateFactionUI();
        }

        private void UpdateFactionUI()
        {
            if (userManager == null || userManager.currentUser == null) return;
            if (userManager.currentUser.selectedFaction == null)
            {
                if (userManager.currentUser.factionSelections != null && userManager.currentUser.factionSelections.Count > 0)
                {
                    userManager.UpdateFactionSelection(0);
                }
                else return;
            }

            var info = userManager.currentUser.selectedFaction.GetFactionInfo();

            if (factionIcon != null) factionIcon.sprite = info.factionLogo;
            if (factionNameLabel != null) factionNameLabel.text = info.factionName;
            if (factionDescLabel != null) factionDescLabel.text = info.factionDescription;
        }

        private void QuitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        #endregion
    }
}