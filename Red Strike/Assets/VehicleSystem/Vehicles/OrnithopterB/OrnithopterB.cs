using UnityEngine;
using VehicleSystem;

namespace VehicleSystem.Vehicles.OrnithopterB
{
    public class OrnithopterB : AirVehicle
    {
        public Transform barrelPoint_A;
        public Transform barrelPoint_B;
        public Transform barrelTransform;
        public ParticleSystem muzzleFlashEffect_A;
        public ParticleSystem muzzleFlashEffect_B;

        protected override void Update()
        {
            base.Update();

            if (targetObject != null && isAttacking)
            {
                LookAtTarget(targetObject.transform);
            }
        }

        protected override void FireShot()
        {
            base.FireShot();

            if (ammunition != null && currentAmmunition > 0)
            {
                GameObject bullet_A = Instantiate(ammunition.ammunitionPrefab, barrelPoint_A.position, barrelPoint_A.rotation);
                bullet_A.GetComponent<Rigidbody>().linearVelocity = barrelPoint_A.forward * bulletSpeed;
                bullet_A.GetComponent<AmmunitionSystem.Ammunitions.Ammunition>().ownerVehicle = this;

                muzzleFlashEffect_A.Play();

                GameObject bullet_B = Instantiate(ammunition.ammunitionPrefab, barrelPoint_B.position, barrelPoint_B.rotation);
                bullet_B.GetComponent<Rigidbody>().linearVelocity = barrelPoint_B.forward * bulletSpeed;
                bullet_B.GetComponent<AmmunitionSystem.Ammunitions.Ammunition>().ownerVehicle = this;
                
                muzzleFlashEffect_B.Play();

                currentAmmunition -= 2;
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
            Vector3 localDirection = transform.InverseTransformDirection(directionToTarget);
            Quaternion targetLocalRotation = Quaternion.LookRotation(localDirection);
            barrelTransform.localRotation = Quaternion.Slerp(barrelTransform.localRotation, targetLocalRotation, Time.deltaTime * turnSpeed);
        }
    }
}
