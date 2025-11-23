using UnityEngine;
using System.Collections;
using UnityEngine.AI;

namespace VehicleSystem.Vehicles
{
    [RequireComponent(typeof(BoxCollider))]
    public class Vehicle : MonoBehaviour
    {
        public VehicleSystem.Vehicle vehicleData;

        public GameObject targetObject;
        public ParticleSystem smokeEffect;

        protected float speed;
        protected float turnSpeed;
        protected float fuelLevel;
        protected float fuelConsumptionRate;
        protected float health;
        protected float maxHealth;
        protected float stoppingDistance;

        protected int maxAmmunition = 30;
        protected int currentAmmunition = 30;
        protected int reloadCounter = 0;
        protected float bulletDamage = 10f;
        protected float bulletSpeed = 20f;
        protected float reloadTime = 1.5f;
        protected GameObject bulletPrefab;

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
            fuelLevel = vehicleData.fuelCapacity;
            fuelConsumptionRate = vehicleData.fuelConsumptionRate;
            maxAmmunition = vehicleData.maxAmmunition;
            currentAmmunition = maxAmmunition;
            bulletDamage = vehicleData.bulletDamage;
            bulletSpeed = vehicleData.bulletSpeed;
            reloadTime = vehicleData.reloadTime;
            bulletPrefab = vehicleData.bulletPrefab;
            maxHealth = vehicleData.maxHealth;
            health = maxHealth;
            stoppingDistance = vehicleData.stoppingDistance;
        }

        public (string, float, int, int, float) GetVehicleStatus()
        {
            return (vehicleData.vehicleName, fuelLevel, currentAmmunition, maxAmmunition, health);
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
                yield return new WaitForSeconds(reloadTime);
            }
        }

        protected virtual void FireShot()
        {
            // Implement firing logic in derived classes
        }

        protected virtual void ReloadAmmunition()
        {
            reloadCounter++;
            currentAmmunition = maxAmmunition;
        }
    }
}
