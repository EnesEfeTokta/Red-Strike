using UnityEngine.UIElements;

namespace UISystem
{
    public class VSPanelHUDController : GameHUDController
    {
        public void UpdatePlayerName(int playerId, string playerName)
        {
            if (root == null) return;

            string labelName = (playerId == 1) ? "p1-player-name-label" : "p2-player-name-label";
            Label nameLabel = vsPanel.Q<Label>(labelName);

            if (nameLabel != null)
            {
                nameLabel.text = playerName;
            }
        }
    }
}
