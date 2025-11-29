using UnityEngine;
using AmmunitionSystem.Ammunitions;

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

            if (targetObject != null)
            {
                LookAtTarget(targetObject.transform);
            }
        }

        protected override void FireShot()
        {
            if (ammunition_bullet != null && currentAmmunition_bullet > 0)
            {
                if(targetObject == null) return;

                Vector3 directionA = (targetObject.transform.position - barrelPoint_A.position).normalized;
                Quaternion rotationA = Quaternion.LookRotation(directionA);
                
                GameObject bullet_A = Instantiate(ammunition_bullet.ammunitionPrefab, barrelPoint_A.position, rotationA);
                var bulletScriptA = bullet_A.GetComponent<Ammunition>();
                if(bulletScriptA != null) bulletScriptA.ownerVehicle = this;

                if(muzzleFlashEffect_A != null) muzzleFlashEffect_A.Play();

                Vector3 directionB = (targetObject.transform.position - barrelPoint_B.position).normalized;
                Quaternion rotationB = Quaternion.LookRotation(directionB);

                GameObject bullet_B = Instantiate(ammunition_bullet.ammunitionPrefab, barrelPoint_B.position, rotationB);
                
                var bulletScriptB = bullet_B.GetComponent<Ammunition>();
                if(bulletScriptB != null) bulletScriptB.ownerVehicle = this;
                
                if(muzzleFlashEffect_B != null) muzzleFlashEffect_B.Play();

                currentAmmunition_bullet -= 2;
            }
        }

        protected override void LaunchRocket()
        {
            if (ammunition_rocket != null)
            {
                GameObject rocket_A = Instantiate(ammunition_rocket.ammunitionPrefab, rocketLaunchPoint_A.position, rocketLaunchPoint_A.rotation);
                rocket_A.GetComponent<Ammunition>().ownerVehicle = this;
                rocket_A.GetComponent<Ammunition>().SetRocket(targetObject.transform);

                GameObject rocket_B = Instantiate(ammunition_rocket.ammunitionPrefab, rocketLaunchPoint_B.position, rocketLaunchPoint_B.rotation);
                rocket_B.GetComponent<Ammunition>().ownerVehicle = this;
                rocket_B.GetComponent<Ammunition>().SetRocket(targetObject.transform);

                currentAmmunition_rocket -= 2;
                RocketObjectVisibility(false);
            }
        }

        private void LookAtTarget(Transform target)
        {
            Vector3 directionToTarget = target.position - barrelTransform.position;
            Vector3 localDirection = transform.InverseTransformDirection(directionToTarget);
            Quaternion targetLocalRotation = Quaternion.LookRotation(localDirection);
            barrelTransform.localRotation = Quaternion.Slerp(barrelTransform.localRotation, targetLocalRotation, Time.deltaTime * turnSpeed);
        }

        protected override void ReloadRocketAmmunition()
        {
            base.ReloadRocketAmmunition();
            RocketObjectVisibility(true);
            Debug.Log("Rockets Reloaded");
        }

        private void RocketObjectVisibility(bool isVisible)
        {
            if (rocketObject != null)
            {
                foreach (GameObject rocket in rocketObject)
                {
                    if (rocket != null) rocket.SetActive(isVisible);
                }
            }
        }
    }
}
