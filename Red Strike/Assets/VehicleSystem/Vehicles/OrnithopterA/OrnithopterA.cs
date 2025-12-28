using UnityEngine;
using NetworkingSystem;
using Fusion;

namespace VehicleSystem.Vehicles.OrnithopterA
{
    public class OrnithopterA : AirVehicle
    {
        [Header("Ornithopter Weaponry")]
        public Transform barrelTransform; 
        public Transform[] bulletSpawnPoints; 
        
        [Header("Missiles")]
        public Transform[] rocketLaunchPoints; 
        public GameObject[] rocketVisuals; 

        private int _rocketIndex = 0;

        protected override void Update()
        {
            base.Update();
            
            HandleVisuals();
        }

        private void HandleVisuals()
        {
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

            foreach (var spawnPoint in bulletSpawnPoints)
            {
                if (currentAmmunition_bullet <= 0) break;

                Vector3 aimDir = transform.forward;
                if(targetObject != null)
                {
                    aimDir = (targetObject.transform.position - spawnPoint.position).normalized;
                }

                Quaternion fireRot = Quaternion.LookRotation(aimDir);

                CommanderData.LocalCommander.RPC_SpawnAmmunition(
                    ammunition_bullet.ammunitionName, 
                    spawnPoint.position, 
                    fireRot, 
                    Object
                );

                currentAmmunition_bullet--;
            }
        }

        protected override void LaunchRocket()
        {
            if (currentAmmunition_rocket <= 0) return;

            Transform currentPoint = rocketLaunchPoints[_rocketIndex % rocketLaunchPoints.Length];
            
            NetworkId targetId = default;
            if (targetObject != null)
                targetId = targetObject.GetComponent<NetworkObject>().Id;

            CommanderData.LocalCommander.RPC_SpawnAmmunition(
                ammunition_rocket.ammunitionName, 
                currentPoint.position, 
                currentPoint.rotation, 
                Object, 
                targetId
            );

            if (rocketVisuals != null && _rocketIndex < rocketVisuals.Length)
            {
                if(rocketVisuals[_rocketIndex] != null) 
                    rocketVisuals[_rocketIndex].SetActive(false);
            }

            currentAmmunition_rocket--;
            _rocketIndex++;
            if (_rocketIndex >= rocketLaunchPoints.Length) _rocketIndex = 0;
        }

        protected override void ReloadRocketAmmunition()
        {
            base.ReloadRocketAmmunition();
            if (rocketVisuals != null)
            {
                foreach (var vis in rocketVisuals)
                {
                    if (vis != null) vis.SetActive(true);
                }
            }
            _rocketIndex = 0;
        }

        private void LookAtTarget(Transform target)
        {
            if (barrelTransform == null) return;
            Vector3 dir = target.position - barrelTransform.position;
            Quaternion look = Quaternion.LookRotation(dir);
            barrelTransform.rotation = Quaternion.Slerp(barrelTransform.rotation, look, Time.deltaTime * 5f);
        }
    }
}