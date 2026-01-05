using UnityEngine;
using UnityEngine.UIElements;

namespace UISystem
{
    public class TacticalOverviewController : GameHUDController
    {
        private VisualElement viewForces;
        private VisualElement viewSector;
        
        private Button tabForces;
        private Button tabSector;
        private Button btnClose;

        private const string ActiveTabClass = "tab-active";
        private const string SlotBaseClass = "status-slot";
        private const string SlotFilledClass = "slot-filled";

        protected override void OnEnable()
        {
            base.OnEnable();

            if (tacticalOverviewPanel == null)
            {
                Debug.LogError("tacticalOverviewPanel VisualElement is null!");
                return;
            }

            viewForces = tacticalOverviewPanel.Q<VisualElement>("view-forces");
            viewSector = tacticalOverviewPanel.Q<VisualElement>("view-sector");

            tabForces = tacticalOverviewPanel.Q<Button>("tab-forces");
            tabSector = tacticalOverviewPanel.Q<Button>("tab-sector");
            btnClose = tacticalOverviewPanel.Q<Button>("btn-close-window");

            // 4. Olayları (Events) Bağla
            if (btnClose != null)
                btnClose.clicked += () => SetWindowVisibility(false);

            if (tabForces != null)
                tabForces.clicked += ShowForcesView;

            if (tabSector != null)
                tabSector.clicked += ShowSectorView;
        }

        protected override void Update()
        {
            base.Update();

            if (tacticalOverviewPanel == null || tacticalOverviewPanel == null) return;

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                bool isVisible = tacticalOverviewPanel.style.display == DisplayStyle.Flex;
                SetWindowVisibility(!isVisible);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (tacticalOverviewPanel.style.display == DisplayStyle.Flex)
                {
                    SetWindowVisibility(false);
                }
            }
        }

        public void SetWindowVisibility(bool isVisible)
        {
            if (tacticalOverviewPanel == null) return;

            tacticalOverviewPanel.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;

            if (isVisible)
            {
                ShowForcesView();
            }
        }

        private void ShowForcesView()
        {
            if (viewForces == null || viewSector == null) return;

            viewForces.style.display = DisplayStyle.Flex;
            viewSector.style.display = DisplayStyle.None;

            tabForces?.AddToClassList(ActiveTabClass);
            tabSector?.RemoveFromClassList(ActiveTabClass);
        }

        private void ShowSectorView()
        {
            if (viewForces == null || viewSector == null) return;

            viewForces.style.display = DisplayStyle.None;
            viewSector.style.display = DisplayStyle.Flex;

            tabForces?.RemoveFromClassList(ActiveTabClass);
            tabSector?.AddToClassList(ActiveTabClass);
        }

        public void UpdatePlayerName(int playerId, string playerName)
        {
            if (tacticalOverviewPanel == null) return;

            string labelName = (playerId == 1) ? "p1-player-name-label" : "p2-player-name-label";
            Label nameLabel = tacticalOverviewPanel.Q<Label>(labelName);

            if (nameLabel != null)
            {
                nameLabel.text = playerName;
            }
        }

        public void UpdateUnitSlots(int playerId, string unitId, int currentCount, int maxCapacity)
        {
            if (tacticalOverviewPanel == null) return;

            string formattedUnitId = unitId.Trim().ToLower().Replace(" ", "-");
            string prefix = (playerId == 1) ? "p1" : "p2";
            
            string containerName = $"{prefix}-{formattedUnitId}-slots";

            VisualElement container = tacticalOverviewPanel.Q<VisualElement>(containerName);

            if (container == null) return;

            container.Clear();

            for (int i = 0; i < maxCapacity; i++)
            {
                VisualElement slot = new VisualElement();
                slot.AddToClassList(SlotBaseClass);

                if (i < currentCount)
                {
                    slot.AddToClassList(SlotFilledClass);
                }

                container.Add(slot);
            }
        }

        public void ClearAllUnitSlots()
        {
            string[] unitIds = new string[]
            {
                "main-station", "hangar", "energy-tower",
                "infantry", "trike", "quad", "tank-combat", "tank-heavy-a", "tank-heavy-b",
                "ornithopter-a", "ornithopter-b"
            };

            foreach (string unitId in unitIds)
            {
                UpdateUnitSlots(1, unitId, 0, 0);
                UpdateUnitSlots(2, unitId, 0, 0);
            }
        }
    }
}