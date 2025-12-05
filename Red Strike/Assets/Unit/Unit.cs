using UnityEngine;

namespace Unit
{
    public class Unit : MonoBehaviour
    {
        [Header("Unit Info")]
        public int teamId;
        public PlayerType playerType;

        public virtual void TakeDamage(float damage)
        {
            // Implement damage logic here
        }
    }
}
