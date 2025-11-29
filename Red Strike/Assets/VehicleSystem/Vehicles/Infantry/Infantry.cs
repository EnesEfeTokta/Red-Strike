using UnityEngine;

namespace VehicleSystem.Vehicles.Infantry
{
    public class Infantry : GroundVehicle
    {
        [Header("Infantry Settings")]
        public Transform barrelTransform;
        public ParticleSystem muzzleFlashEffect;

        protected override void FireShot()
        {
            base.FireShot();
            
            if (ammunition_bullet != null && currentAmmunition_bullet > 0)
            {
                GameObject bullet = Instantiate(ammunition_bullet.ammunitionPrefab, barrelTransform.position, barrelTransform.rotation);
                bullet.GetComponent<Rigidbody>().linearVelocity = barrelTransform.forward * bulletAmmunitionSettings.ammunition.speed;
                bullet.GetComponent<AmmunitionSystem.Ammunitions.Ammunition>().ownerVehicle = this;
                
                muzzleFlashEffect.Play();
                
                currentAmmunition_bullet--;
            }
            else if (currentAmmunition_bullet <= 0)
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
