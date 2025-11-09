using System.Collections.Generic;
using UnityEngine;

namespace VehicleSystem
{
    [CreateAssetMenu(fileName = "New Vehicles Database", menuName = "Vehicle System/Vehicles Database")]    
    public class VehiclesDatabase : ScriptableObject
    {
        public List<Vehicle> vehicles;
    }
}
