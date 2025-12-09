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

        private void Start()
        {
            inputController = GetComponent<InputController.InputController>();
        }

        protected virtual void OnEnable() 
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("Bu objede UIDocument bileşeni bulunamadı!", this);
                return;
            }
            root = uiDocument.rootVisualElement;
            buildingDynamicContentContainer = root.Q<VisualElement>("building-dynamic-content-container");
            vehicleDynamicContentContainer = root.Q<VisualElement>("vehicle-dynamic-content-container");
        }

        protected virtual void OnDisable() { }

        protected virtual void Update() { }
    }
}