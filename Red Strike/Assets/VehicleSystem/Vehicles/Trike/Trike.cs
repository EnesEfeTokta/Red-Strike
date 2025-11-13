using UnityEngine;
using VehicleSystem.Vehicles;

namespace VehicleSystem.Vehicles.Trike
{
    public class Trike : GroundVehicle
    {
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
            Vector3 directionToTarget = target.position - barrelTransform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            barrelTransform.rotation = Quaternion.Slerp(barrelTransform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}
