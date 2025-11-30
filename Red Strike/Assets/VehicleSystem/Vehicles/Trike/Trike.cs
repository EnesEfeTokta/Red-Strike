using UnityEngine;

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

            GameObject bullet = Instantiate(ammunition_bullet.ammunitionPrefab, barrelPoint.position, barrelPoint.rotation);
            bullet.GetComponent<AmmunitionSystem.Ammunitions.Ammunition>().ownerVehicle = this;

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
