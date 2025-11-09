using UnityEngine;

namespace VehicleSystem.Vehicles.Infantry
{
    public class Infantry : Vehicle
    {
        public Transform barrelTransform;

        protected override void MoveTo(Vector3 destination)
        {
            base.MoveTo(destination);
            BarrelAimAtTarget();
        }

        public void Attack()
        {
            // Infantry attack logic here
        }

        public void Defend()
        {
            // Infantry defend logic here
        }

        private void BarrelAimAtTarget()
        {
            // Aim the barrel towards the target object
        }

        private void IdleAnimation()
        {
            
        }
    }
}
