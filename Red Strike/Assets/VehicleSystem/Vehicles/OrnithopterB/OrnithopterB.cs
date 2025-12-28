using UnityEngine;
using System.Collections;
using NetworkingSystem;
using Fusion;

namespace VehicleSystem.Vehicles.OrnithopterB
{
    public class OrnithopterB : AirVehicle
    {
        [Header("Ornithopter B Weaponry")]
        public Transform barrelTransform;
        public Transform[] barrelPoints;

        [Header("Salvo Missile Settings")]
        [Tooltip("Görsel Roket Objeleri Sırası: 0=Sol, 1=Sol, 2=Sağ, 3=Sağ")]
        public GameObject[] rocketObjects;

        [Header("Launch Points")]
        public Transform[] rocketLaunchLeftPoints;
        public Transform[] rocketLaunchRightPoints;

        private bool isFiringSalvo = false;

        protected override void Update()
        {
            base.Update();

            if (targetObject != null && CurrentState == VehicleState.Combat && CurrentCombatPhase == CombatPhase.Diving)
            {
                LookAtTarget(targetObject.transform);
            }
            else
            {
                if (barrelTransform != null)
                {
                    barrelTransform.localRotation = Quaternion.Slerp(
                        barrelTransform.localRotation, 
                        Quaternion.identity, 
                        Time.deltaTime * 3f
                    );
                }
            }
        }

        protected override void FireShot()
        {
            if (currentAmmunition_bullet <= 0) return;

            foreach (var point in barrelPoints)
            {
                if (currentAmmunition_bullet <= 0) break;

                Vector3 targetDir = transform.forward;
                if (targetObject != null)
                    targetDir = (targetObject.transform.position - point.position).normalized;

                Quaternion rotation = Quaternion.LookRotation(targetDir);

                CommanderData.LocalCommander.RPC_SpawnAmmunition(
                    ammunition_bullet.ammunitionName, 
                    point.position, 
                    rotation, 
                    Object
                );

                currentAmmunition_bullet--;
            }
        }

        protected override void LaunchRocket()
        {
            if (isFiringSalvo || currentAmmunition_rocket < 2) return;

            StartCoroutine(FireRocketSalvoRoutine());
        }

        private IEnumerator FireRocketSalvoRoutine()
        {
            isFiringSalvo = true;

            NetworkId targetId = default;
            if (targetObject != null)
                targetId = targetObject.GetComponent<NetworkObject>().Id;

            if (currentAmmunition_rocket >= 2)
            {
                FireSingleRocket(rocketLaunchLeftPoints[0], 0, targetId);
                if(rocketLaunchLeftPoints.Length > 1) 
                    FireSingleRocket(rocketLaunchLeftPoints[1], 1, targetId);
            }

            yield return new WaitForSeconds(0.8f); 

            if (currentAmmunition_rocket >= 2)
            {
                if (targetObject != null) targetId = targetObject.GetComponent<NetworkObject>().Id;

                FireSingleRocket(rocketLaunchRightPoints[0], 2, targetId);
                if(rocketLaunchRightPoints.Length > 1)
                    FireSingleRocket(rocketLaunchRightPoints[1], 3, targetId);
            }

            isFiringSalvo = false;
        }

        private void FireSingleRocket(Transform spawnPoint, int visualIndex, NetworkId targetId)
        {
            if (currentAmmunition_rocket <= 0) return;

            CommanderData.LocalCommander.RPC_SpawnAmmunition(
                ammunition_rocket.ammunitionName, 
                spawnPoint.position, 
                spawnPoint.rotation, 
                Object, 
                targetId
            );

            SetRocketVisualInternal(visualIndex, false);

            currentAmmunition_rocket--;
        }

        private void LookAtTarget(Transform target)
        {
            if (barrelTransform == null) return;
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