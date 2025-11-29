using UnityEngine;

namespace VehicleSystem.Vehicles.OrnithopterB
{
    public class OrnithopterB : AirVehicle
    {
        [Header("Ornithopter B Settings")]
        public Transform barrelPoint_A;
        public Transform barrelPoint_B;
        public Transform barrelTransform;
        public ParticleSystem muzzleFlashEffect_A;
        public ParticleSystem muzzleFlashEffect_B;

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
                GameObject bullet_A = Instantiate(ammunition_bullet.ammunitionPrefab, barrelPoint_A.position, barrelPoint_A.rotation);
                bullet_A.GetComponent<Rigidbody>().linearVelocity = barrelPoint_A.forward * bulletAmmunitionSettings.ammunition.speed;
                bullet_A.GetComponent<AmmunitionSystem.Ammunitions.Ammunition>().ownerVehicle = this;

                muzzleFlashEffect_A.Play();

                GameObject bullet_B = Instantiate(ammunition_bullet.ammunitionPrefab, barrelPoint_B.position, barrelPoint_B.rotation);
                bullet_B.GetComponent<Rigidbody>().linearVelocity = barrelPoint_B.forward * bulletAmmunitionSettings.ammunition.speed;
                bullet_B.GetComponent<AmmunitionSystem.Ammunitions.Ammunition>().ownerVehicle = this;
                
                muzzleFlashEffect_B.Play();

                currentAmmunition_bullet -= 2;
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
            Vector3 localDirection = transform.InverseTransformDirection(directionToTarget);
            Quaternion targetLocalRotation = Quaternion.LookRotation(localDirection);
            barrelTransform.localRotation = Quaternion.Slerp(barrelTransform.localRotation, targetLocalRotation, Time.deltaTime * turnSpeed);
        }
    }
}
