using UnityEngine;
using System.Collections;
using UnityEngine.Video;

namespace VehicleSystem.Vehicles
{
    public class Vehicle : MonoBehaviour
    {
        public VehicleSystem.Vehicle vehicleData;
        protected float speed;
        protected float coverageAreaRadius;
        private float fuelLevel;
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

        protected bool isMoving = false;
        private bool isDestroyed = false;
        private bool isRefueling = false;
        private bool isAttacking = false;
        private bool isRepairing = false;

        private Coroutine attackCoroutine;

        private void Start()
        {
            Setup();
        }

        private void Setup()
        {
            speed = vehicleData.speed;
            coverageAreaRadius = vehicleData.coverageAreaRadius;
            fuelLevel = vehicleData.fuelCapacity;
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

        private void Update()
        {
            if (targetObject == null)
            {
                StopAttacking();
                isMoving = false;
            }

            float distanceToTarget = Vector3.Distance(transform.position, targetObject.transform.position);

            if (distanceToTarget > stoppingDistance && distanceToTarget < coverageAreaRadius)
            {
                isMoving = true;
                StopAttacking();
                MoveTo(targetObject.transform.position);


                fuelLevel -= Time.deltaTime * vehicleData.fuelConsumptionRate;
                if (fuelLevel <= 0)
                {
                    fuelLevel = 0;
                    isMoving = false;
                }
            }
            else if (distanceToTarget <= stoppingDistance && distanceToTarget < coverageAreaRadius)
            {
                isMoving = false;

                if (!isAttacking)
                {
                    StartAttacking();
                }
            }
            else
            {
                isMoving = false;
                StopAttacking();
            }

            UpdateSmokeEffect();
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

        protected virtual void MoveTo(Vector3 destination)
        {
            Vector3 lookPosition = destination;
            lookPosition.y = transform.position.y;
            transform.LookAt(lookPosition);

            destination.y = 0;
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
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
