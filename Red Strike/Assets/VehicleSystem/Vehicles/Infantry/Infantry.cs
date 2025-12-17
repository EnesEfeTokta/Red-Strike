using NetworkingSystem;
using UnityEngine;
namespace VehicleSystem.Vehicles.Infantry
{
    public class Infantry : GroundVehicle
    {
        [Header("Infantry Settings")]
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
            
            CommanderData.LocalCommander.RPC_SpawnAmmunition(
                ammunition_bullet.ammunitionName,
                barrelPoint.position,
                barrelPoint.rotation,
                Object);

            currentAmmunition_bullet--;
        }

        private void LookAtTarget(Transform target)
        {
            Vector3 directionToTarget = target.position - barrelTransform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            Vector3 localTargetEuler = (Quaternion.Inverse(transform.rotation) * targetRotation).eulerAngles;

            float targetX = localTargetEuler.x;

            Quaternion newLocalRotation = Quaternion.Euler(targetX, 0, 0);

            barrelTransform.localRotation = Quaternion.Slerp(barrelTransform.localRotation, newLocalRotation, Time.deltaTime * turnSpeed);
        }
    }
}
