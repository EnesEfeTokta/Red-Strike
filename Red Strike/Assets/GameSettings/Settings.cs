using UnityEngine;

namespace GameSettings
{
    [CreateAssetMenu(fileName = "Settings", menuName = "Game Settings/Settings", order = 1)]
    public class Settings : ScriptableObject
    {
        [Header("Audio Settings")]
        [Range(0f, 1f)] public float masterVolume = 1.0f;
        [Range(0f, 1f)] public float musicVolume = 0.8f;

        [Header("Graphics Settings")]
        public bool isFullscreen = true;
    }
}
