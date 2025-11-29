using UnityEngine;

namespace VehicleSystem.Vehicles.Trike
{
    public class Trike : GroundVehicle
    {
        [Header("Trike Settings")]
        public Transform barrelTransform;
        public Transform barrelPoint;
        public ParticleSystem muzzleFlashEffect;

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

            if (ammunition_bullet != null && currentAmmunition_bullet > 0)
            {
                GameObject bullet = Instantiate(ammunition_bullet.ammunitionPrefab, barrelPoint.position, barrelPoint.rotation);
                bullet.GetComponent<Rigidbody>().linearVelocity = barrelPoint.forward * bulletAmmunitionSettings.ammunition.speed;
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

        private void LookAtTarget(Transform target)
        {
            Vector3 directionToTarget = target.position - barrelTransform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            barrelTransform.rotation = Quaternion.Slerp(barrelTransform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}
