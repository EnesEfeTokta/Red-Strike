using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using NetworkingSystem;

namespace UISystem
{
    public class TacticalOverviewController : GameHUDController
    {
        private VisualElement viewForces;
        private VisualElement viewSector;
        private VisualElement viewControls;

        private Button tabForces;
        private Button tabSector;
        private Button tabControls;
        private Button btnClose;

        [Header("Map Settings")]
        public float WorldWidth = 500f;
        public float WorldLength = 500f;

        public Transform MapCenterPoint;

        [Header("UI Texture")]
        public RenderTexture MapTexture;

        private VisualElement mapSurface;

        private const string ActiveTabClass = "tab-active";
        private const string SlotBaseClass = "status-slot";
        private const string SlotFilledClass = "slot-filled";

        private Dictionary<Unit.Unit, VisualElement> trackedUnits = new Dictionary<Unit.Unit, VisualElement>();

        private void Start()
        {
            InitializeMapUI();
        }

        private void InitializeMapUI()
        {
            if (tacticalOverviewPanel == null) return;

            if (mapSurface == null)
            {
                mapSurface = tacticalOverviewPanel.Q<VisualElement>("map-surface");

                if (mapSurface != null && MapTexture != null)
                {
                    mapSurface.style.backgroundImage = Background.FromRenderTexture(MapTexture);
                }
            }
        }

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
            viewControls = tacticalOverviewPanel.Q<VisualElement>("view-controls");

            tabForces = tacticalOverviewPanel.Q<Button>("tab-forces");
            tabSector = tacticalOverviewPanel.Q<Button>("tab-sector");
            tabControls = tacticalOverviewPanel.Q<Button>("tab-controls");
            btnClose = tacticalOverviewPanel.Q<Button>("btn-close-window");

            if (btnClose != null)
                btnClose.clicked += () => SetWindowVisibility(false);

            if (tabForces != null)
                tabForces.clicked += ShowForcesView;

            if (tabSector != null)
                tabSector.clicked += ShowSectorView;
            
            if (tabControls != null)
                tabControls.clicked += ShowControlsView;
        }

        protected override void Update()
        {
            base.Update();

            if (tacticalOverviewPanel == null ) return;

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

            if (mapSurface == null || mapSurface.style.display == DisplayStyle.None) return;

            foreach (var kvp in trackedUnits)
            {
                Unit.Unit unit = kvp.Key;
                VisualElement icon = kvp.Value;

                if (unit == null) continue;

                UpdateIconPosition(icon, unit.transform);
            }
        }

        public void RegisterUnit(Unit.Unit unit, Sprite iconSprite)
        {
            if (mapSurface == null) InitializeMapUI();
            if (mapSurface == null || trackedUnits.ContainsKey(unit)) return;

            VisualElement icon = new VisualElement();
            icon.AddToClassList("map-icon");

            if (iconSprite != null)
            {
                icon.style.backgroundImage = new StyleBackground(iconSprite);
                icon.style.backgroundColor = Color.clear;
            }
            else
            {
                int localTeamId = CommanderData.LocalCommander != null ? CommanderData.LocalCommander.PlayerTeamID : -1;
                if (unit.teamId == localTeamId) icon.AddToClassList("icon-friendly");
                else icon.AddToClassList("icon-enemy");
            }

            mapSurface.Add(icon);
            trackedUnits.Add(unit, icon);
        }

        public void UnregisterUnit(Unit.Unit unit)
        {
            if (trackedUnits.ContainsKey(unit))
            {
                VisualElement icon = trackedUnits[unit];

                if (mapSurface != null)
                    mapSurface.Remove(icon);

                trackedUnits.Remove(unit);
            }
        }

        private void UpdateIconPosition(VisualElement icon, Transform worldObj)
        {
            Vector3 relPos = worldObj.position - MapCenterPoint.position;

            float normalizedX = (relPos.x + (WorldWidth / 2)) / WorldWidth;
            float normalizedZ = (relPos.z + (WorldLength / 2)) / WorldLength;

            normalizedX = Mathf.Clamp01(normalizedX);
            normalizedZ = Mathf.Clamp01(normalizedZ);

            icon.style.left = Length.Percent(normalizedX * 100);
            icon.style.top = Length.Percent((1 - normalizedZ) * 100);
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
            viewControls.style.display = DisplayStyle.None;

            tabForces?.AddToClassList(ActiveTabClass);
            tabSector?.RemoveFromClassList(ActiveTabClass);
        }

        private void ShowSectorView()
        {
            if (viewForces == null || viewSector == null) return;

            viewForces.style.display = DisplayStyle.None;
            viewSector.style.display = DisplayStyle.Flex;
            viewControls.style.display = DisplayStyle.None;

            tabForces?.RemoveFromClassList(ActiveTabClass);
            tabSector?.AddToClassList(ActiveTabClass);
        }

        private void ShowControlsView()
        {
            if (viewForces == null || viewControls == null) return;

            viewForces.style.display = DisplayStyle.None;
            viewControls.style.display = DisplayStyle.Flex;
            viewSector.style.display = DisplayStyle.None;

            tabForces?.RemoveFromClassList(ActiveTabClass);
            tabControls?.AddToClassList(ActiveTabClass);
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