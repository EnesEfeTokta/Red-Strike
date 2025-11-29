using UnityEngine;
using System.Collections; // IEnumerator için gerekli
using AmmunitionSystem.Ammunitions;

namespace VehicleSystem.Vehicles.OrnithopterB
{
    public class OrnithopterB : AirVehicle
    {
        [Header("Ornithopter B Settings")]
        public Transform barrelPoint_A;
        public Transform barrelPoint_B;
        public Transform barrelTransform;

        [Header("Muzzle Flash Effects")]
        public ParticleSystem muzzleFlashEffect_A;
        public ParticleSystem muzzleFlashEffect_B;

        [Header("Rocket Settings")]
        [Tooltip("Sıralama: 0=SolA, 1=SolB, 2=SağA, 3=SağB")]
        public GameObject[] rocketObjects;
        
        [Header("Launch Points")]
        public Transform rocketLaunchLeftPoint_A;
        public Transform rocketLaunchLeftPoint_B;
        [Space]
        public Transform rocketLaunchRightPoint_A;
        public Transform rocketLaunchRightPoint_B;

        private bool isFiringSalvo = false;

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
                CreateBullet(barrelPoint_A.position, rotationA);
                if(muzzleFlashEffect_A != null) muzzleFlashEffect_A.Play();

                Vector3 directionB = (targetObject.transform.position - barrelPoint_B.position).normalized;
                Quaternion rotationB = Quaternion.LookRotation(directionB);
                CreateBullet(barrelPoint_B.position, rotationB);
                if(muzzleFlashEffect_B != null) muzzleFlashEffect_B.Play();

                currentAmmunition_bullet -= 2;
            }
        }

        private void CreateBullet(Vector3 pos, Quaternion rot)
        {
            GameObject bullet = Instantiate(ammunition_bullet.ammunitionPrefab, pos, rot);
            var bulletScript = bullet.GetComponent<Ammunition>();
            if(bulletScript != null) bulletScript.ownerVehicle = this;
        }

        protected override void LaunchRocket()
        {
            if (isFiringSalvo || ammunition_rocket == null) return;

            StartCoroutine(FireRocketSalvoRoutine());
        }

        private IEnumerator FireRocketSalvoRoutine()
        {
            isFiringSalvo = true;

            if (currentAmmunition_rocket >= 2 && targetObject != null)
            {
                FireSingleRocket(rocketLaunchLeftPoint_A);
                FireSingleRocket(rocketLaunchLeftPoint_B);

                SetRocketVisualInternal(0, false);
                SetRocketVisualInternal(1, false);

                currentAmmunition_rocket -= 2;
            }

            yield return new WaitForSeconds(1.0f);

            if (currentAmmunition_rocket >= 2 && targetObject != null)
            {
                FireSingleRocket(rocketLaunchRightPoint_A);
                FireSingleRocket(rocketLaunchRightPoint_B);

                SetRocketVisualInternal(2, false);
                SetRocketVisualInternal(3, false);

                currentAmmunition_rocket -= 2;
            }
            
            isFiringSalvo = false;
        }

        private void FireSingleRocket(Transform spawnPoint)
        {
            if (targetObject == null) return;

            GameObject rocket = Instantiate(ammunition_rocket.ammunitionPrefab, spawnPoint.position, spawnPoint.rotation);
            var script = rocket.GetComponent<Ammunition>();
            if (script != null)
            {
                script.ownerVehicle = this;
                script.SetRocket(targetObject.transform);
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
            Debug.Log("Ornithopter B: Rockets Reloaded");
        }

        private void RocketObjectVisibility(bool isVisible)
        {
            if (rocketObjects != null)
            {
                foreach (GameObject rocket in rocketObjects)
                {
                    if (rocket != null) rocket.SetActive(isVisible);
                }
            }
        }

        private void SetRocketVisualInternal(int index, bool active)
        {
            if (rocketObjects != null && index < rocketObjects.Length && rocketObjects[index] != null)
            {
                rocketObjects[index].SetActive(active);
            }
        }
    }
}