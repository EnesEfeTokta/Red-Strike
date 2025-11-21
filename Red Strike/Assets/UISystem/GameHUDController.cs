using UnityEngine;
using UnityEngine.UIElements;

namespace UISystem
{
    public class GameHUDController : MonoBehaviour
    {
        public InputController.InputController inputController;
        protected UIDocument uiDocument;
        protected VisualElement root;

        protected virtual void OnEnable() 
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                Debug.LogError("Bu objede UIDocument bileşeni bulunamadı!", this);
                return;
            }
            root = uiDocument.rootVisualElement;
        }

        protected virtual void OnDisable() { /* Boş bırakıldı, alt sınıflar tarafından geçersiz kılınabilir */ }

        protected virtual void Update() { /* Boş bırakıldı, alt sınıflar tarafından geçersiz kılınabilir */ }
    }
}