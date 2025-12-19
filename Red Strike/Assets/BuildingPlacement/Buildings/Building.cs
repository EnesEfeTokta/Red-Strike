using UnityEngine;
using GameStateSystem;
using Fusion;

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
                //Debug.Log($"Building {BuildingName} destroyed.");

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

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_Rotate90()
        {
            networkTransform.Teleport(transform.position, transform.rotation * Quaternion.Euler(0, 90, 0));
        }

        private void OnCollisionEnter(Collision collision)
        {
            var unit = collision.gameObject.GetComponent<Unit.Unit>();
            if (unit == null)
                return;

            if (unit.teamId == teamId)
                return;

            //Debug.Log($"Building {BuildingName} collided with unit: {collision.gameObject.name}");
        }
    }
}
