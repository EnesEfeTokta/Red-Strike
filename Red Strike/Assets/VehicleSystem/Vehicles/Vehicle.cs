using UnityEngine;
using System.Collections;
using UnityEngine.AI;

namespace VehicleSystem.Vehicles
{
    [RequireComponent(typeof(BoxCollider))]
    public class Vehicle : MonoBehaviour
    {
        public VehicleSystem.Vehicle vehicleData;
        protected float speed;
        protected float coverageAreaRadius;
        protected float turnSpeed;
        protected float fuelLevel;
        protected float fuelConsumptionRate;
        protected float health;
        private float maxHealth = 100f;
        private float stoppingDistance = 1.5f;
        protected int maxAmmunition = 30;
        protected int currentAmmunition = 30;
        protected int reloadCounter = 0;
        protected float bulletDamage = 10f;
        protected float bulletSpeed = 20f;
        protected float reloadTime = 1.5f;

        protected GameObject bulletPrefab;

        public GameObject targetObject;

        public ParticleSystem smokeEffect;

        private NavMeshAgent agent;

        protected bool isMoving = false;
        private bool isDestroyed = false;
        private bool isRefueling = false;
        protected bool isAttacking = false;
        private bool isRepairing = false;

        private Coroutine attackCoroutine;

        protected virtual void Start()
        {
            Setup();
            agent = GetComponent<NavMeshAgent>();
            agent.speed = speed;
            agent.stoppingDistance = stoppingDistance;
        }

        private void Setup()
        {
            speed = vehicleData.speed;
            coverageAreaRadius = vehicleData.coverageAreaRadius;
            turnSpeed = vehicleData.turnSpeed;
            fuelLevel = vehicleData.fuelCapacity;
            fuelConsumptionRate = vehicleData.fuelConsumptionRate;
            stoppingDistance = vehicleData.stoppingDistance;
            maxAmmunition = vehicleData.maxAmmunition;
            currentAmmunition = maxAmmunition;
            bulletDamage = vehicleData.bulletDamage;
            bulletSpeed = vehicleData.bulletSpeed;
            reloadTime = vehicleData.reloadTime;
            bulletPrefab = vehicleData.bulletPrefab;
            maxHealth = vehicleData.maxHealth;
            health = maxHealth;
        }

        public (string, float, int, int, float) GetVehicleStatus()
        {
            return (vehicleData.vehicleName, fuelLevel, currentAmmunition, maxAmmunition, health);
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
            Vector3 direction = (targetObject.transform.position - transform.position).normalized;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, agent.angularSpeed * Time.deltaTime);
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
