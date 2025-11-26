using System.Collections.Generic;
using UnityEngine;

namespace AmmunitionSystem
{
    [CreateAssetMenu(fileName = "Ammunition Database", menuName = "Ammunition System/Ammunition Database")]
    public class AmmunitionDatabase : ScriptableObject
    {
        public List<Ammunition> ammunitions;
    }
}
