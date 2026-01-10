using UnityEngine;
using GameStateSystem;
using Fusion;

namespace BuildingPlacement.Buildings
{
    public class Building : Unit.Unit
    {
        public BuildingPlacement.Building buildingData;

        [Networked] public float health { get; set; }
        public float maxHealth;
        public string buildingName;
        public Animator animator;

        public override void Spawned()
        {
            base.Spawned();
            health = buildingData.maxHealth;
            maxHealth = buildingData.maxHealth;
            buildingName = buildingData.buildingName;
        }

        public override void TakeDamage(float damage)
        {
            if (!Object.HasStateAuthority) return;

            base.TakeDamage(damage);

            health -= damage;

            if (health <= 0)
            {
                NotificationSystem.NotificationSystem.Show(
                    "Building Destroyed",
                    $"{buildingName} has been destroyed.",
                    NotificationSystem.NotificationType.Warning
                );

                ParticleSystem exp = Instantiate(buildingData.explosionEffect, transform.position, Quaternion.identity);
                Destroy(exp.gameObject, exp.main.duration);

                if (this is MainStation)
                {
                    if (GameStateManager.Instance != null)
                    {
                        GameStateManager.Instance.ReportTeamLoss(teamId);
                    }
                }

                GameStateManager.Instance.ReportUnitDestroyed(this);
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
        }
    }
}
