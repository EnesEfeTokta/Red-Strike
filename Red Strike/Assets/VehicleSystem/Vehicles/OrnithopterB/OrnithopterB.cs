using UnityEngine;
using System.Collections;
using AmmunitionSystem.Ammunitions;
using NetworkingSystem;
using Fusion;

namespace VehicleSystem.Vehicles.OrnithopterB
{
    public class OrnithopterB : AirVehicle
    {
        [Header("Ornithopter B Settings")]
        [Header("Bullet Settings")]
        public Transform[] barrelPoints;
        public Transform barrelTransform;

        [Header("Rocket Settings")]
        [Tooltip("Sıralama: 0=SolA, 1=SolB, 2=SağA, 3=SağB")]
        public GameObject[] rocketObjects;

        [Header("Launch Points")]
        public Transform[] rocketLaunchLeftPoints;
        [Space]
        public Transform[] rocketLaunchRightPoints;

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
            Vector3 direction = (targetObject.transform.position - barrelPoints[0].position).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction);

            for (int i = 0; i < barrelPoints.Length; i++)
            {
                Transform barrelPoint = barrelPoints[i];
                CommanderData.LocalCommander.RPC_SpawnAmmunition(ammunition_bullet.ammunitionName, barrelPoint.position, rotation, Object);
                currentAmmunition_bullet--;
            }
        }

        protected override void LaunchRocket()
        {
            if (isFiringSalvo || ammunition_rocket == null) return;

            StartCoroutine(FireRocketSalvoRoutine());
        }

        private IEnumerator FireRocketSalvoRoutine()
        {
            isFiringSalvo = true;

            NetworkId targetId = default;
            if (targetObject != null)
                targetId = targetObject.GetComponent<NetworkObject>().Id;

            if (currentAmmunition_rocket >= 2 && targetObject != null)
            {
                FireSingleRocket(rocketLaunchLeftPoints[0], targetId);
                FireSingleRocket(rocketLaunchLeftPoints[1], targetId);

                SetRocketVisualInternal(0, false);
                SetRocketVisualInternal(1, false);

                currentAmmunition_rocket -= 2;
            }

            yield return new WaitForSeconds(1.0f);

            if (currentAmmunition_rocket >= 2 && targetObject != null)
            {
                FireSingleRocket(rocketLaunchRightPoints[0], targetId);
                FireSingleRocket(rocketLaunchRightPoints[1], targetId);

                SetRocketVisualInternal(2, false);
                SetRocketVisualInternal(3, false);

                currentAmmunition_rocket -= 2;
            }

            isFiringSalvo = false;
        }

        private void FireSingleRocket(Transform spawnPoint, NetworkId targetId)
        {
            //Debug.LogWarning("Ornithopter B: FireSingleRocket called, BUT: Not Spawning Yet");
            if (targetObject == null) return;

            CommanderData.LocalCommander.RPC_SpawnAmmunition(ammunition_rocket.ammunitionName, spawnPoint.position, spawnPoint.rotation, Object, targetId);
            currentAmmunition_rocket--;
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