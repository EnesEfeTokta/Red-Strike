using UnityEngine;
using Unit;

namespace BuildingPlacement.Buildings
{
    public class Building : Unit.Unit
    {
        public BuildingPlacement.Building buildingData;
        public ParticleSystem[] buildEffects;

        protected float health;
        private float maxHealth = 100f;
        public float CurrentHealth => health;
        public string BuildingName => buildingData != null ? buildingData.name : gameObject.name;

        private void Start()
        {
            maxHealth = buildingData.maxHealth;
            health = maxHealth;
        }

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);

            health -= damage;
            health = Mathf.Max(0, health);

            Debug.Log($"Building {BuildingName} took {damage} damage. Remaining health: {health}");

            if (health <= 0)
            {
                Debug.Log($"Building {BuildingName} destroyed.");
                ParticleSystem exp = Instantiate(buildingData.explosionEffect, transform.position, Quaternion.identity);
                OnDestroy();
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

        [ContextMenu("Test Delete Main Station")]
        public void TestDelete() // TEST METHOD
        {
            Debug.Log("Main Station Test Delete triggered.");
            TakeDamage(health + 1); // Ensure destruction
        }
    }
}
