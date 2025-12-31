using UnityEngine.UIElements;

namespace UISystem
{
    public class HUDNotificationController : GameHUDController
    {
        public void AddNotificationToUI(VisualElement notificationItem)
        {
            if (notificationContainer != null)
            {
                notificationContainer.Insert(0, notificationItem);
            }
        }

        public void RemoveNotificationFromUI(VisualElement notificationItem)
        {
            if (notificationItem != null && notificationItem.parent != null)
            {
                notificationItem.parent.Remove(notificationItem);
            }
        }
    }
}
