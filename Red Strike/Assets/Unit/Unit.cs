using UnityEngine;

namespace Unit
{
    public class Unit : MonoBehaviour
    {
        [Header("Unit Info")]
        public int teamId;
        public PlayerType playerType;
        public UnitType unitType;

        public virtual void TakeDamage(float damage)
        {
            // Implement damage logic here
        }
    }

    public enum PlayerType { Red, Blue }
    public enum UnitType { Vehicle, Building }
}
