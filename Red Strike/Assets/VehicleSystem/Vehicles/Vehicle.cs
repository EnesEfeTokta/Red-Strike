using UnityEngine;
using System.Collections;
using AmmunitionSystem;

namespace VehicleSystem.Vehicles
{
    [RequireComponent(typeof(BoxCollider))]
    public class Vehicle : MonoBehaviour
    {
        [Header("Vehicle Data")]
        public VehicleSystem.Vehicle vehicleData;

        [Header("Runtime Settings")]
        public GameObject targetObject;
        public ParticleSystem smokeEffect;

        protected float speed;
        protected float turnSpeed;
        protected float fuelLevel;
        protected float fuelConsumptionRate;
        protected float health;
        protected float maxHealth;
        protected float stoppingDistance;

        // Bullet ammunition
        protected int currentAmmunition_bullet = 30;
        protected int reloadCounter_bullet = 0;
        protected Ammunition ammunition_bullet;
        protected VehicleAmmunition bulletAmmunitionSettings;

        // Rocket ammunition
        protected int currentAmmunition_rocket = 10;
        protected int reloadCounter_rocket = 0;
        protected Ammunition ammunition_rocket;
        protected VehicleAmmunition rocketAmmunitionSettings;

        protected bool isMoving = false;
        protected bool isAttacking = false;
        private Coroutine attackCoroutine;

        protected virtual void Start()
        {
            Setup();
        }

        private void Setup()
        {
            speed = vehicleData.speed;
            turnSpeed = vehicleData.turnSpeed;
            stoppingDistance = vehicleData.stoppingDistance;
            maxHealth = vehicleData.maxHealth;
            health = maxHealth;
            fuelLevel = vehicleData.fuelCapacity;
            fuelConsumptionRate = vehicleData.fuelConsumptionRate;

            if (vehicleData.ammunitionSettings != null)
            {
                foreach (var ammoSetting in vehicleData.ammunitionSettings)
                {
                    if (ammoSetting.isEnabled)
                    {
                        switch (ammoSetting.ammunitionType)
                        {
                            case AmmunitionType.Bullet:
                                bulletAmmunitionSettings = ammoSetting;
                                ammunition_bullet = ammoSetting.ammunition;
                                currentAmmunition_bullet = ammoSetting.maxAmmunition;
                                break;
                            case AmmunitionType.Rocket:
                                rocketAmmunitionSettings = ammoSetting;
                                ammunition_rocket = ammoSetting.ammunition;
                                currentAmmunition_rocket = ammoSetting.maxAmmunition;
                                break;
                        }
                    }
                }
            }
        }

        public (string, float, int, int, float) GetVehicleStatus()
        {
            return (vehicleData.vehicleName, fuelLevel, currentAmmunition_bullet, bulletAmmunitionSettings.maxAmmunition, health);
        }

        public virtual void SetTargetEnemy(GameObject enemy)
        {
            if (fuelLevel <= 0) return;

            targetObject = enemy;
            isMoving = true;
        }

        protected virtual void Update()
        {
        }

        protected virtual void ConsumeFuel()
        {
            if (isMoving && fuelLevel > 0)
            {
                fuelLevel -= fuelConsumptionRate * Time.deltaTime;
                fuelLevel = Mathf.Max(0, fuelLevel);
            }
        }

        protected virtual void LookAtTarget()
        {
             if (targetObject == null) return;
             Vector3 direction = (targetObject.transform.position - transform.position).normalized;
             direction.y = 0;
             if (direction != Vector3.zero)
             {
                 Quaternion lookRotation = Quaternion.LookRotation(direction);
                 transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);
             }
        }

        protected virtual void UpdateSmokeEffect()
        {
            if (smokeEffect == null) return;

            if (isMoving && !smokeEffect.isPlaying)
            {
                smokeEffect.Play();
            }
            else if (!isMoving && smokeEffect.isPlaying)
            {
                smokeEffect.Stop();
            }
        }

        protected virtual void StartAttacking()
        {
            isAttacking = true;
            if (attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(AttackRoutine());
            }
        }

        protected virtual void StopAttacking()
        {
            isAttacking = false;
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }

        protected virtual IEnumerator AttackRoutine()
        {
            while (isAttacking)
            {
                FireShot();
                yield return new WaitForSeconds(bulletAmmunitionSettings.reloadTime);
            }
        }

        protected virtual void FireShot()
        {
            // Implement firing logic in derived classes
        }

        protected virtual void LaunchRocket()
        {
            // Implement missile launching logic in derived classes
        }

        protected virtual void ReloadAmmunition()
        {
            reloadCounter_bullet++;
            currentAmmunition_bullet = bulletAmmunitionSettings.maxAmmunition;
        }
    }
}
