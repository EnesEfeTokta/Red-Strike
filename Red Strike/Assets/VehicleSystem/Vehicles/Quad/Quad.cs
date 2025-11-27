using UnityEngine;

namespace VehicleSystem.Vehicles.Quad
{
    public class Quad : GroundVehicle
    {
        public Transform barrelTransform_A;
        public Transform barrelTransform_B;
        public Transform barrelPoint_A;
        public Transform barrelPoint_B;
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
            Vector3 directionToTarget_A = target.position - barrelTransform_A.position;
            Vector3 localDirection_A = barrelTransform_A.parent.InverseTransformDirection(directionToTarget_A);
            localDirection_A.y = 0;

            if (localDirection_A != Vector3.zero)
            {
                Quaternion targetRotation_A = Quaternion.LookRotation(localDirection_A);
                Quaternion localRotation_A = barrelTransform_A.localRotation;
                barrelTransform_A.localRotation = Quaternion.Slerp(localRotation_A, targetRotation_A, Time.deltaTime * turnSpeed);
            }

            Vector3 directionToTarget_B = target.position - barrelTransform_B.position;
            Vector3 localDirection_B = barrelTransform_B.parent.InverseTransformDirection(directionToTarget_B);
            localDirection_B.x = 0;

            if (localDirection_B != Vector3.zero)
            {
                Quaternion targetRotation_B = Quaternion.LookRotation(localDirection_B);
                Quaternion localRotation_B = barrelTransform_B.localRotation;
                barrelTransform_B.localRotation = Quaternion.Slerp(localRotation_B, targetRotation_B, Time.deltaTime * turnSpeed);
            }
        }
    }
}
