using System.Collections.Generic;
using UnityEngine;
using BuildingPlacement;

namespace BuildingPlacement
{
    [CreateAssetMenu(fileName = "BuildingsDatabase", menuName = "Buildings/BuildingsDatabase")]
    public class BuildingsDatabase : ScriptableObject
    {
        public List<Building> buildings;
    }
}