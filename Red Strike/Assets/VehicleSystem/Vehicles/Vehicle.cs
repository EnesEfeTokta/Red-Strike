using UnityEngine;
using System.Collections;
using UnityEngine.AI;

namespace VehicleSystem.Vehicles
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Vehicle : MonoBehaviour
    {
        public VehicleSystem.Vehicle vehicleData;
        protected float speed;
        protected float coverageAreaRadius;
        private float fuelLevel;
        private float fuelConsumptionRate;
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
        private bool isAttacking = false;
        private bool isRepairing = false;

        private Coroutine attackCoroutine;

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();

            agent.speed = vehicleData.speed;
            agent.stoppingDistance = vehicleData.stoppingDistance;
            agent.angularSpeed = vehicleData.turnSpeed;
            Setup();
        }

        private void Setup()
        {
            speed = vehicleData.speed;
            coverageAreaRadius = vehicleData.coverageAreaRadius;
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

        public (string, float, int, int) GetVehicleStatus()
        {
            return (vehicleData.vehicleName, fuelLevel, currentAmmunition, maxAmmunition);
        }

        private void Update()
        {
            if (targetObject == null)
            {
                if (agent.hasPath)
                {
                    agent.isStopped = true;
                    agent.ResetPath();
                }
                StopAttacking();
                isMoving = false;
                return;
            }

            if (fuelLevel <= 0)
            {
                fuelLevel = 0;
                agent.isStopped = true;
                StopAttacking();
                isMoving = false;
                return;
            }

            agent.SetDestination(targetObject.transform.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                isMoving = false;
                LookAtTarget();

                if (!isAttacking)
                {
                    StartAttacking();
                }
            }
            else
            {
                agent.isStopped = false;
                isMoving = true;
                StopAttacking();

                ConsumeFuel();
            }

            UpdateSmokeEffect();
        }

        private void ConsumeFuel()
        {
            if (isMoving && fuelLevel > 0)
            {
                fuelLevel -= fuelConsumptionRate * Time.deltaTime;
                fuelLevel = Mathf.Max(0, fuelLevel);
            }
        }

        private void LookAtTarget()
        {
            Vector3 direction = (targetObject.transform.position - transform.position).normalized;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, agent.angularSpeed * Time.deltaTime);
        }

        private void UpdateSmokeEffect()
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

        private void StartAttacking()
        {
            isAttacking = true;
            if (attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(AttackRoutine());
            }
        }

        private void StopAttacking()
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
