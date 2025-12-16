using UnityEngine;
using GameStateSystem;
using Fusion;
using Unity.VisualScripting;

namespace BuildingPlacement.Buildings
{
    public class Building : Unit.Unit
    {
        public BuildingPlacement.Building buildingData;

        [Networked] protected float health { get; set; }
        public float Health { get { return health; } }

        public string BuildingName => buildingData != null ? buildingData.name : gameObject.name;

        public override void Spawned()
        {
            if (Object.HasStateAuthority)
            {
                health = buildingData.maxHealth;
            }
        }

        public override void TakeDamage(float damage)
        {
            if (!Object.HasStateAuthority) return;

            base.TakeDamage(damage);

            health -= damage;

            if (health <= 0)
            {
                Debug.Log($"Building {BuildingName} destroyed.");

                ParticleSystem exp = Instantiate(buildingData.explosionEffect, transform.position, Quaternion.identity);
                Destroy(exp.gameObject, exp.main.duration);

                if (this is MainStation)
                {
                    if (GameStateManager.Instance != null)
                    {
                        GameStateManager.Instance.ReportTeamLoss(teamId);
                    }
                }

                Runner.Despawn(Object);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            var unit = collision.gameObject.GetComponent<Unit.Unit>();
            if (unit == null)
                return;

            if (unit.teamId == teamId)
                return;

            Debug.Log($"Building {BuildingName} collided with unit: {collision.gameObject.name}");
        }
    }
}
