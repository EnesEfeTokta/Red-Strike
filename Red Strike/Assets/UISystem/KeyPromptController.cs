using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using InputController;

namespace UISystem
{
    public class KeyPromptController : GameHUDController
    {
        public static KeyPromptController Instance;
        private List<VisualElement> _slots = new List<VisualElement>();
        private const int MAX_SLOTS = 5;

        public struct PromptRequest
        {
            public InputType inputType; // KeyCode yerine InputType
            public string description;

            public PromptRequest(InputType type, string desc)
            {
                inputType = type;
                description = desc;
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(this);
            else Instance = this;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (keyPromptContainer != null)
            {
                _slots.Clear();
                for (int i = 0; i < MAX_SLOTS; i++)
                {
                    var slot = keyPromptContainer.Q<VisualElement>($"Slot_{i}");
                    if (slot != null)
                    {
                        _slots.Add(slot);
                        ShowSlot(slot, false);
                    }
                }
            }
        }

        public void UpdatePrompts(List<PromptRequest> requests)
        {
            if (_slots.Count == 0) return;

            for (int i = 0; i < MAX_SLOTS; i++)
            {
                var slot = _slots[i];

                if (i < requests.Count)
                {
                    ConfigureSlot(slot, requests[i]);
                    ShowSlot(slot, true);
                }
                else
                {
                    ShowSlot(slot, false);
                }
            }
        }

        private void ConfigureSlot(VisualElement slot, PromptRequest req)
        {
            var iconEl = slot.Q<VisualElement>("KeyIcon");
            var labelEl = slot.Q<Label>("ActionText");

            // Veritabanından yeni enum ile ikon çekiyoruz
            Sprite iconSprite = InputController.InputController.Instance.iconDatabase.GetIcon(req.inputType);

            if (iconSprite != null)
            {
                iconEl.style.backgroundImage = new StyleBackground(iconSprite);
                iconEl.style.display = DisplayStyle.Flex;
            }
            else
            {
                // İkon bulunamazsa gizle ama yazıyı göster
                iconEl.style.display = DisplayStyle.None;
            }

            labelEl.text = req.description;
        }

        private void ShowSlot(VisualElement slot, bool show)
        {
            if (show)
            {
                slot.RemoveFromClassList("key-slot--hidden");
            }
            else
            {
                slot.AddToClassList("key-slot--hidden");
            }
        }
    }
}
