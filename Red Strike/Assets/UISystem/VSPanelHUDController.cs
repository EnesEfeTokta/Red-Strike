using UnityEngine.UIElements;
using UnityEngine;

namespace UISystem
{
    public class VSPanelHUDController : GameHUDController
    {
        private Image avatarP1;
        private Image avatarP2;

        private Label nameP1;
        private Label nameP2;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (vsPanel == null) return;

            avatarP1 = vsPanel.Q<Image>("p1-avatar-image");
            avatarP2 = vsPanel.Q<Image>("p2-avatar-image");

            nameP1 = vsPanel.Q<Label>("p1-player-name-label");
            nameP2 = vsPanel.Q<Label>("p2-player-name-label");
        }

        public void UpdatePlayerName(int playerId, string playerName)
        {
            if (root == null) return;
            if (playerId == 1 && nameP1 != null)
            {
                nameP1.text = playerName;
            }
            else if (playerId == 2 && nameP2 != null)
            {
                nameP2.text = playerName;
            }
        }

        public void UpdatePlayerAvatar(int playerId, int avatarIndex)
        {
            if (root == null) return;

            var user = InputController.InputController.Instance.userData;
            if (user == null || user.availableAvatars == null) return;

            Sprite selectedSprite = null;
            if (user.availableAvatars != null && avatarIndex >= 0 && avatarIndex < user.availableAvatars.Length)
            {
                selectedSprite = user.availableAvatars[avatarIndex];
            }
            else
            {
                if (user.availableAvatars != null && user.availableAvatars.Length > 0)
                    selectedSprite = user.availableAvatars[0];
            }

            if (selectedSprite != null)
            {
                if (playerId == 1 && avatarP1 != null)
                {
                    avatarP1.sprite = selectedSprite;
                }
                else if (playerId == 2 && avatarP2 != null)
                {
                    avatarP2.sprite = selectedSprite;
                }
            }
        }
    }
}
