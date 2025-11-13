using UnityEngine;

namespace VehicleSystem.Vehicles.Infantry
{
    public class Infantry : GroundVehicle
    {
        public Transform barrelTransform;

        protected override void FireShot()
        {
            base.FireShot();
            
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
    }
}
