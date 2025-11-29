using UnityEngine;

namespace VehicleSystem.Vehicles.OrnithopterA
{
    public class OrnithopterA : AirVehicle
    {
        [Header("Ornithopter A Settings")]
        public Transform barrelPoint_A;
        public Transform barrelPoint_B;
        public Transform barrelTransform;
        public ParticleSystem muzzleFlashEffect_A;
        public ParticleSystem muzzleFlashEffect_B;
        public GameObject[] rocketObject;
        public Transform rocketLaunchPoint_A;
        public Transform rocketLaunchPoint_B;

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

        protected override void LaunchRocket()
        {
            base.LaunchRocket();

            if (ammunition_rocket != null)
            {
                GameObject rocket_A = Instantiate(ammunition_rocket.ammunitionPrefab, rocketLaunchPoint_A.position, rocketLaunchPoint_A.rotation);
                rocket_A.GetComponent<Rigidbody>().linearVelocity = rocketLaunchPoint_A.forward * rocketAmmunitionSettings.ammunition.speed;
                rocket_A.GetComponent<AmmunitionSystem.Ammunitions.Ammunition>().ownerVehicle = this;

                GameObject rocket_B = Instantiate(ammunition_rocket.ammunitionPrefab, rocketLaunchPoint_B.position, rocketLaunchPoint_B.rotation);
                rocket_B.GetComponent<Rigidbody>().linearVelocity = rocketLaunchPoint_B.forward * rocketAmmunitionSettings.ammunition.speed;
                rocket_B.GetComponent<AmmunitionSystem.Ammunitions.Ammunition>().ownerVehicle = this;
                currentAmmunition_rocket -= 2;
            }
            else
            {
                Debug.Log("No rockets available to launch.");
            }
        }

        private void LookAtTarget(Transform target)
        {
            Vector3 directionToTarget = target.position - barrelTransform.position;
            Vector3 localDirection = transform.InverseTransformDirection(directionToTarget);
            Quaternion targetLocalRotation = Quaternion.LookRotation(localDirection);
            barrelTransform.localRotation = Quaternion.Slerp(barrelTransform.localRotation, targetLocalRotation, Time.deltaTime * turnSpeed);
        }

        private void RocketObjectVisibility(bool isVisible)
        {
            foreach (GameObject rocket in rocketObject)
            {
                rocket.SetActive(isVisible);
            }
        }
    }
}
