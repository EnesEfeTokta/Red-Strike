using UnityEngine;
using NetworkingSystem;

namespace VehicleSystem.Vehicles.Trike
{
    public class Trike : GroundVehicle
    {
        [Header("Trike Settings")]
        public Transform barrelTransform;
        public Transform barrelPoint;

        protected override void Update()
        {
            base.Update();

            if (targetObject != null)
            {
                LookAtTarget(targetObject.transform);
            }
        }

        protected override void FireShot()
        {
            base.FireShot();

            CommanderData.LocalCommander.RPC_SpawnAmmunition(ammunition_bullet.ammunitionName, barrelPoint.position, barrelPoint.rotation, Object);
            currentAmmunition_bullet--;
        }

        private void LookAtTarget(Transform target)
        {
            Vector3 directionToTarget = target.position - barrelTransform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            barrelTransform.rotation = Quaternion.Slerp(barrelTransform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }
}
