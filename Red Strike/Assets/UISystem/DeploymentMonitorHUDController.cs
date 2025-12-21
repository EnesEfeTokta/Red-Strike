using UnityEngine;
using UnityEngine.UIElements;

namespace UISystem
{
    public class DeploymentMonitorHUDController : GameHUDController
    {
        public int Player1 = 1;
        public int Player2 = 2;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (root == null) return;

            var closeButton = root.Q<Button>("deployment-close-button");
            if (closeButton != null) closeButton.clicked += () => { ChangeDeploymentMonitorVisible(); };
        }

        protected override void Update()
        {
            base.Update();

            if (root == null) return;
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ChangeDeploymentMonitorVisible();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ChangeDeploymentMonitorVisible();
            }
        }

        public void UpdatePlayerName(int playerId, string playerName)
        {
            if (root == null) return;

            string labelName = (playerId == Player1) ? "p1-player-name-label" : "p2-player-name-label";
            Label nameLabel = root.Q<Label>(labelName);

            if (nameLabel != null)
            {
                nameLabel.text = playerName;
            }
        }

        public void UpdateUnitSlots(int playerId, string unitId, int currentCount, int maxCapacity)
        {
            if (root == null) return;
            string formattedUnitId = unitId.Trim().ToLower().Replace(" ", "-");

            string prefix = (playerId == Player1) ? "p1" : "p2";
            string containerName = $"{prefix}-{formattedUnitId}-slots";

            VisualElement container = root.Q<VisualElement>(containerName);

            if (container == null) return;

            container.Clear();

            for (int i = 0; i < maxCapacity; i++)
            {
                VisualElement slot = new VisualElement();
                slot.AddToClassList("status-slot");

                if (i < currentCount)
                {
                    slot.AddToClassList("slot-filled");
                }
                else
                {
                    slot.AddToClassList("slot-empty");
                }

                container.Add(slot);
            }
        }

        public void ClearAllUnitSlots()
        {
            if (root == null) return;

            string[] unitIds = new string[]
            {
                "main-station", "hangar", "energy-tower",
                "infantry", "trike", "quad", "tank-combat", "tank-heavy-a", "tank-heavy-b",
                "ornithopter-a", "ornithopter-b"
            };

            foreach (string unitId in unitIds)
            {
                UpdateUnitSlots(Player1, unitId, 0, 0);
                UpdateUnitSlots(Player2, unitId, 0, 0);
            }
        }

        public void ChangeDeploymentMonitorVisible()
        {
            if (root == null) return;

            if (deploymentMonitorPanel.style.display == DisplayStyle.None)
            {
                deploymentMonitorPanel.style.display = DisplayStyle.Flex;
            }
            else
            {
                deploymentMonitorPanel.style.display = DisplayStyle.None;
            }
        }
    }
}