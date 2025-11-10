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

        protected override void FireShot()
        {
            base.FireShot();

            BarrelAimAtTarget();
            
            if (bulletPrefab != null && currentAmmunition > 0)
            {
                GameObject bullet = Instantiate(bulletPrefab, barrelTransform.position, barrelTransform.rotation);
                bullet.GetComponent<Rigidbody>().linearVelocity = barrelTransform.forward * bulletSpeed;
                
                currentAmmunition--;
            }
            else if (currentAmmunition <= 0)
            {
                Debug.Log("Out of ammunition, reloading...");
                ReloadAmmunition();
            }
        }

        public void Defend()
        {
            // Infantry defend logic here
        }

        private void BarrelAimAtTarget()
        {
            // Aim the barrel towards the target object
        }
    }
}
