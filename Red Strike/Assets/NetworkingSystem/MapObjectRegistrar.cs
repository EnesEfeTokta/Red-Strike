using UnityEngine;
using Fusion;
using UISystem;

namespace NetworkingSystem
{
    public class MapObjectRegistrar : NetworkBehaviour
    {
        private Unit.Unit unitComponent;
        private TacticalOverviewController tacticalOverviewController;

        public override void Spawned()
        {
            unitComponent = GetComponent<Unit.Unit>();
            tacticalOverviewController = GetComponent<TacticalOverviewController>();

            if (tacticalOverviewController != null)
            {
                Sprite mapIconSprite = null;
                if (TryGetComponent<VehicleSystem.Vehicles.Vehicle>(out var vehicle))
                {
                    mapIconSprite = vehicle.vehicleData.vehicleIcon;
                }
                else if (TryGetComponent<BuildingPlacement.Buildings.Building>(out var building))
                {
                    mapIconSprite = building.buildingData.buildingIcon;
                }
                tacticalOverviewController.RegisterUnit(unitComponent, mapIconSprite);
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (tacticalOverviewController != null && unitComponent != null)
            {
                tacticalOverviewController.UnregisterUnit(unitComponent);
            }
        }
    }
}