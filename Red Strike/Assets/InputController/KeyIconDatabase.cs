using UnityEngine;
using System.Collections.Generic;

namespace InputController
{
    [CreateAssetMenu(fileName = "KeyIconDatabase", menuName = "Input/Key Icon Database")]
    public class KeyIconDatabase : ScriptableObject
    {
        [System.Serializable]
        public struct IconEntry
        {
            public InputType type;
            public Sprite icon;
        }

        public List<IconEntry> icons;

        public Sprite GetIcon(InputType type)
        {
            foreach (var entry in icons)
            {
                if (entry.type == type) return entry.icon;
            }
            return null;
        }
    }

    public enum InputType
    {
        // Klavye
        W, A, S, D,
        WASD,
        Space,
        E, F, R,
        Escape,
        Shift,
        Ctrl,
        Tab,

        // Fare
        LMB,            // Sol Tık (Left Mouse Button)
        RMB,            // Sağ Tık (Right Mouse Button)
        MMB,            // Orta Tık
        MouseScrollUp,  // Tekerlek Yukarı
        MouseScrollDown,// Tekerlek Aşağı
        MouseMoveY,      // Fare Hareketi Yönü
        MouseMoveX,      // Fare Hareketi X Yönü
        MouseDrag       // Fare Sürükleme
    }
}
