using UnityEngine;
using UnityEngine.UIElements;

namespace UISystem
{
    public class GameHUDController : MonoBehaviour
    {
        protected InputController.InputController inputController;

        protected UIDocument uiDocument;
        protected VisualElement root;

        protected VisualElement buildingDynamicContentContainer;
        protected VisualElement vehicleDynamicContentContainer;
        protected VisualElement gameOverPanel;
        protected VisualElement fadePanel;
        protected VisualElement victoryPanel;
        protected VisualElement deploymentMonitorPanel;
        protected VisualElement vsPanel;
        protected VisualElement notificationContainer;

        protected void Awake()
        {
            inputController = GetComponent<InputController.InputController>();
            uiDocument = GetComponent<UIDocument>();
        }

        protected virtual void OnEnable() 
        {
            if (uiDocument == null) return;
            root = uiDocument.rootVisualElement;

            buildingDynamicContentContainer = root.Q<VisualElement>("building-dynamic-content-container");
            vehicleDynamicContentContainer = root.Q<VisualElement>("vehicle-dynamic-content-container");
            gameOverPanel = root.Q<VisualElement>("game-over-panel");
            fadePanel = root.Q<VisualElement>("fade-panel");
            victoryPanel = root.Q<VisualElement>("victory-panel");
            vsPanel = root.Q<VisualElement>("vs-panel");
            notificationContainer = root.Q<VisualElement>("notification-list");
            if (notificationContainer != null) notificationContainer.Clear();
            deploymentMonitorPanel = root.Q<VisualElement>("deployment-panel");
            deploymentMonitorPanel.style.display = DisplayStyle.None;
        }

        protected virtual void OnDisable() { }

        protected virtual void Update() { }

        protected virtual void StartFadeIn()
        {
            if (fadePanel != null)
            {
                fadePanel.style.display = DisplayStyle.Flex;
            }
        }
    }
}