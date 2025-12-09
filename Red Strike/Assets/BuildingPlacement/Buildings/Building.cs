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
            foreach (ParticleSystem effect in buildEffects)
            {
                var main = effect.main;
                main.startColor = playerType == PlayerType.Red ? Color.red : Color.blue;
            }

            maxHealth = buildingData.maxHealth;
            health = maxHealth;
        }

        public override void TakeDamage(float damage)
        {
            health -= damage;
            health = Mathf.Max(0, health);

            Debug.Log($"Building {BuildingName} took {damage} damage. Remaining health: {health}");

            if (health <= 0)
            {
                Debug.Log($"Building {BuildingName} destroyed.");
                Instantiate(buildingData.explosionEffect, transform.position, Quaternion.identity);
                Destroy(gameObject);
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
