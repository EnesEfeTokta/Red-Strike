using UnityEngine;
using NetworkingSystem;

namespace VehicleSystem.Vehicles.OrnithopterA
{
    public class OrnithopterA : AirVehicle
    {
        [Header("Ornithopter A Settings")]
        [Header("Bullet Settings")]
        public Transform[] barrelPoints;
        public Transform barrelTransform;

        [Header("Rocket Settings")]
        public GameObject[] rocketObjects;

        [Header("Launch Points")]
        public Transform[] rocketLaunchPoints;

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
            }

            currentAmmunition_bullet -= 2;
        }

        protected override void LaunchRocket()
        {

            Debug.LogWarning("Ornithopter A: LaunchRocket called, BUT: Not Spawning Yet");
            /*
            for (int i = 0; i < rocketLaunchPoints.Length; i++)
            {
                Transform rocketLaunchPoint = rocketLaunchPoints[i];

                GameObject rocket = Instantiate(ammunition_rocket.ammunitionPrefab, rocketLaunchPoint.position, rocketLaunchPoint.rotation);
                rocket.GetComponent<Ammunition>().ownerVehicle = this;
                rocket.GetComponent<Ammunition>().SetRocket(targetObject.transform);

                currentAmmunition_rocket--;
            }
            */
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
            if (rocketObjects != null)
            {
                foreach (GameObject rocket in rocketObjects)
                {
                    if (rocket != null) rocket.SetActive(isVisible);
                }
            }
        }
    }
}
