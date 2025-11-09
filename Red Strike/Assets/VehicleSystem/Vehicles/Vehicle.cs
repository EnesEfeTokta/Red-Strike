using UnityEngine;

namespace VehicleSystem.Vehicles
{
    public class Vehicle : MonoBehaviour
    {
        public VehicleSystem.Vehicle vehicleData;
        protected float speed;
        private float fuelLevel;
        private float health;
        private float maxHealth = 100f;
        private float stoppingDistance = 1.5f;

        public GameObject targetObject;

        public ParticleSystem smokeEffect;

        protected bool isMoving = false;
        private bool isDestroyed = false;
        private bool isRefueling = false;
        private bool isAttacking = false;
        private bool isRepairing = false;

        private void Start()
        {
            speed = vehicleData.speed;
            fuelLevel = vehicleData.fuelCapacity;
            stoppingDistance = vehicleData.stoppingDistance;
            maxHealth = vehicleData.maxHealth;
            health = maxHealth;
        }

        private void Update()
        {
            if (targetObject == null)
            {
                isMoving = false;
            }
            else
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetObject.transform.position);

                if (distanceToTarget > stoppingDistance)
                {
                    isMoving = true;
                    MoveTo(targetObject.transform.position);
                    fuelLevel -= Time.deltaTime * vehicleData.fuelConsumptionRate;
                    if (fuelLevel <= 0)
                    {
                        fuelLevel = 0;
                        isMoving = false;
                    }
                }
                else
                {
                    isMoving = false;
                }
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
    }
}
