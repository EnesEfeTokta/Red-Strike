using UnityEngine;

namespace VehicleSystem.Vehicles.Infantry
{
    public class Infantry : GroundVehicle
    {
        public Transform barrelTransform;
        public ParticleSystem muzzleFlashEffect;

        protected override void FireShot()
        {
            base.FireShot();
            
            if (ammunition != null && currentAmmunition > 0)
            {
                GameObject bullet = Instantiate(ammunition.ammunitionPrefab, barrelTransform.position, barrelTransform.rotation);
                bullet.GetComponent<Rigidbody>().linearVelocity = barrelTransform.forward * bulletSpeed;
                bullet.GetComponent<AmmunitionSystem.Ammunitions.Ammunition>().ownerVehicle = this;
                
                muzzleFlashEffect.Play();
                
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
