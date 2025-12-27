using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using GameSettings;
using NetworkingSystem;
using UserSystem;

namespace MainMenuSystem
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Scene Elements")]
        public Transform sunTransform;
        public float rotationSpeed = 70f;

        public Transform planetTransform;
        public float planetRotationSpeed = 30f;

        [Header("Cinemachine Cameras")]
        public CinemachineCamera cinemachineCamera_Login;
        public CinemachineCamera cinemachineCamera_Main;
        public CinemachineCamera cinemachineCamera_Options;
        public CinemachineCamera cinemachineCamera_Credits;

        private CinemachineBasicMultiChannelPerlin cameraNoise_Main;

        [Header("Spaceship Animation")]
        public GameObject spaceShip;
        public float shipDuration = 5f;
        public Vector3 shipStartPosition;
        public Vector3 targetPosition;

        [Header("System References")]
        public Settings settings;

        [Header("Audio Settings")]
        public AudioSource soundEffectSource;
        public AudioSource musicSource;
        public AudioClip buttonClickSound1;
        public AudioClip buttonClickSound2;
        public AudioClip launchSound;

        private void Start()
        {
            cameraNoise_Main = cinemachineCamera_Main.GetComponent<CinemachineBasicMultiChannelPerlin>();
            if (spaceShip != null)
            {
                spaceShip.transform.position = shipStartPosition;
            }

            var userManager = GetComponent<UserManager>();

            if (userManager != null && userManager.currentUser != null && GameBootstrap.Instance != null)
            {
                GameBootstrap.Instance.LocalPlayerName = userManager.currentUser.userName ?? "Unknown";

                int index = 0;
                if (userManager.currentUser.availableAvatars != null && userManager.currentUser.avatar != null)
                {
                    index = System.Array.IndexOf(userManager.currentUser.availableAvatars, userManager.currentUser.avatar);
                    if (index == -1) index = 0;
                }

                GameBootstrap.Instance.LocalAvatarIndex = index;
            }

            SwitchToCamera(CameraState.Login);
        }

        private void Update()
        {
            if (sunTransform != null)
                sunTransform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            if (planetTransform != null)
                planetTransform.Rotate(Vector3.up, planetRotationSpeed * Time.deltaTime);
        }

        public enum CameraState { Login, Main, Options, Credits }
        public void SwitchToCamera(CameraState state)
        {
            cinemachineCamera_Login.Priority = 0;
            cinemachineCamera_Main.Priority = 0;
            cinemachineCamera_Options.Priority = 0;
            cinemachineCamera_Credits.Priority = 0;

            switch (state)
            {
                case CameraState.Login:
                    cinemachineCamera_Login.Priority = 10;
                    break;
                case CameraState.Main:
                    cinemachineCamera_Main.Priority = 10;
                    break;
                case CameraState.Options:
                    cinemachineCamera_Options.Priority = 10;
                    break;
                case CameraState.Credits:
                    cinemachineCamera_Credits.Priority = 10;
                    break;
            }
        }

        public IEnumerator LaunchSequence()
        {
            PlaySoundOneShot(launchSound);
            
            if (cinemachineCamera_Main != null && spaceShip != null)
            {
                cinemachineCamera_Main.Target.TrackingTarget = spaceShip.transform;
            }

            float elapsed = 0f;

            while (elapsed < shipDuration)
            {
                if (spaceShip != null)
                {
                    spaceShip.transform.position = Vector3.Lerp(shipStartPosition, targetPosition, elapsed / shipDuration);
                }
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (spaceShip != null)
            {
                spaceShip.transform.position = targetPosition;
            }
        }

        public void ApplyAudioSettings(float masterVol, float musicVol)
        {
            AudioListener.volume = masterVol / 100f;

            if (settings != null)
            {
                settings.masterVolume = masterVol;
                settings.musicVolume = musicVol;
            }
        }

        public void ApplyVideoSettings(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;

            if (settings != null)
            {
                settings.isFullscreen = isFullscreen;
            }
        }

        public enum AudioClipType { ButtonClick1, ButtonClick2, Launch }
        public void PlaySound(AudioClipType clipType)
        {
            switch (clipType)
            {
                case AudioClipType.ButtonClick1:
                    PlaySoundOneShot(buttonClickSound1);
                    break;
                case AudioClipType.ButtonClick2:
                    PlaySoundOneShot(buttonClickSound2);
                    break;
                case AudioClipType.Launch:
                    PlaySoundOneShot(launchSound);
                    break;
            }
        }

        private void PlaySoundOneShot(AudioClip clip)
        {
            if (soundEffectSource != null)
            {
                soundEffectSource.PlayOneShot(clip);
            }
        }
    }
}