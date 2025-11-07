using UnityEngine;

namespace BuildingPlacement.Buildings
{
    public class Building : MonoBehaviour
    {
        public PlayerType playerType;
        public Building buildingData;
        public ParticleSystem[] buildEffects;


        private float health;
        private float maxHealth = 100f;

        private void Start()
        {
            foreach (ParticleSystem effect in buildEffects)
            {
                var main = effect.main;
                main.startColor = playerType == PlayerType.Red ? Color.red : Color.blue;
            }

            maxHealth = buildingData.health;
            health = maxHealth;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Bullet"))
            {
                // Burada hasar hesaplaması yapılabilir.
            }
        }
    }

    public enum PlayerType
    {
        Red,
        Blue
    }
}
