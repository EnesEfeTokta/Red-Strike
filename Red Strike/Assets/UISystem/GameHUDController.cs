using UnityEngine;
using UnityEngine.UIElements;

namespace UISystem
{
    public class GameHUDController : MonoBehaviour
    {
        public InputController.InputController inputController;
        protected UIDocument uiDocument;

        protected virtual void OnEnable() { /* Boş bırakıldı, alt sınıflar tarafından geçersiz kılınabilir */ }

        protected virtual void OnDisable() { /* Boş bırakıldı, alt sınıflar tarafından geçersiz kılınabilir */ }

        protected virtual void Update() { /* Boş bırakıldı, alt sınıflar tarafından geçersiz kılınabilir */ }
    }
}