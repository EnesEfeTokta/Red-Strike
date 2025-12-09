using UnityEngine;

namespace GameSettings
{
    public class Settings : ScriptableObject
    {
        [Header("Audio Settings")]
        public float masterVolume = 1.0f;
        public float musicVolume = 0.8f;

        [Header("Graphics Settings")]
        public bool fullscreen = true;
    }
}
