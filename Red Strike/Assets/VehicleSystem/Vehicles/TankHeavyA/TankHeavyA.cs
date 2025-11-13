using UnityEngine;
using VehicleSystem.Vehicles;

namespace VehicleSystem.Vehicles.TankHeavyA
{
    public class TankHeavyA : GroundVehicle
    {
        public Transform barrelTransform_A;
        public Transform barrelTransform_B;
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

            if (bulletPrefab != null && currentAmmunition > 0)
            {
                GameObject bullet = Instantiate(bulletPrefab, barrelPoint.position, barrelPoint.rotation);
                bullet.GetComponent<Rigidbody>().linearVelocity = barrelPoint.forward * bulletSpeed;

                currentAmmunition--;
            }
            else if (currentAmmunition <= 0)
            {
                Debug.Log("Out of ammunition, reloading...");
                ReloadAmmunition();
            }
        }

        private void LookAtTarget(Transform target)
        {
            Vector3 directionToTarget_A = target.position - barrelTransform_A.position;
            Vector3 localDirection_A = barrelTransform_A.parent.InverseTransformDirection(directionToTarget_A);
            localDirection_A.y = 0;

            if (localDirection_A != Vector3.zero)
            {
                Quaternion targetRotation_A = Quaternion.LookRotation(localDirection_A);
                Quaternion localRotation_A = barrelTransform_A.localRotation;
                barrelTransform_A.localRotation = Quaternion.Slerp(localRotation_A, targetRotation_A, Time.deltaTime * turnSpeed);
            }

            Vector3 directionToTarget_B = target.position - barrelTransform_B.position;
            Vector3 localDirection_B = barrelTransform_B.parent.InverseTransformDirection(directionToTarget_B);
            localDirection_B.x = 0;

            if (localDirection_B != Vector3.zero)
            {
                Quaternion targetRotation_B = Quaternion.LookRotation(localDirection_B);
                Quaternion localRotation_B = barrelTransform_B.localRotation;
                barrelTransform_B.localRotation = Quaternion.Slerp(localRotation_B, targetRotation_B, Time.deltaTime * turnSpeed);
            }
        }
    }
}
