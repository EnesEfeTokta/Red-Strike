using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace NotificationSystem
{
    public enum NotificationType { Info, Warning, Error }

    public class NotificationSystem : MonoBehaviour
    {
        public static NotificationSystem Instance;

        [Header("Settings")]
        [SerializeField] private float notificationDuration = 3f;
        [SerializeField] private int maxNotifications = 5;

        private HUDNotificationController hudController;

        private Queue<VisualElement> _activeNotifications = new Queue<VisualElement>();

        public AudioClip infoSound;
        public AudioClip warningSound;
        public AudioClip errorSound;
        private AudioSource audioSource;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            hudController = GetComponent<HUDNotificationController>();
            audioSource = GetComponentInChildren<AudioSource>();
        }

        public static void Show(string title, string message, NotificationType type)
        {
            if (Instance != null)
            {
                Instance.CreateNotification(title, message, type);
            }
        }

        private void CreateNotification(string title, string message, NotificationType type)
        {
            if (hudController == null) return;

            VisualElement item = new VisualElement();
            item.AddToClassList("notification-item");

            switch (type)
            {
                case NotificationType.Info: item.AddToClassList("type-info"); PlaySound(infoSound); break;
                case NotificationType.Warning: item.AddToClassList("type-warning"); PlaySound(warningSound); break;
                case NotificationType.Error: item.AddToClassList("type-error"); PlaySound(errorSound); break;
            }

            VisualElement glowBar = new VisualElement();
            glowBar.AddToClassList("notif-glow-bar");
            item.Add(glowBar);

            VisualElement content = new VisualElement();
            content.AddToClassList("notif-content");

            Label titleLabel = new Label(title.ToUpper());
            titleLabel.AddToClassList("notif-title");
            content.Add(titleLabel);

            Label msgLabel = new Label(message);
            msgLabel.AddToClassList("notif-message");
            content.Add(msgLabel);

            item.Add(content);

            hudController.AddNotificationToUI(item);

            _activeNotifications.Enqueue(item);

            if (_activeNotifications.Count > maxNotifications)
            {
                VisualElement oldItem = _activeNotifications.Dequeue();
                hudController.RemoveNotificationFromUI(oldItem);
            }

            StartCoroutine(RemoveNotificationRoutine(item));
        }

        private IEnumerator RemoveNotificationRoutine(VisualElement item)
        {
            yield return new WaitForSeconds(notificationDuration);

            if (item != null)
            {
                item.AddToClassList("notification-hide");
                yield return new WaitForSeconds(0.5f);
                hudController.RemoveNotificationFromUI(item);
            }
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip != null) audioSource.PlayOneShot(clip);
        }
    }
}