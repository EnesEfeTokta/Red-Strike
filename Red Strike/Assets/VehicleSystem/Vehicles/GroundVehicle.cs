using UnityEngine;
using VehicleSystem;
using UnityEngine.AI;

namespace VehicleSystem.Vehicles
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class GroundVehicle : Vehicle
    {
        private NavMeshAgent agent;

        [Header("Ground Combat Settings")]
        // Agent'ın durma mesafesinden bağımsız olarak roket atabileceği maksimum mesafe.
        public float rocketAttackRange = 40f; 

        protected override void Start()
        {
            base.Start();

            agent = GetComponent<NavMeshAgent>();
            
            agent.speed = vehicleData.speed;
            agent.stoppingDistance = vehicleData.stoppingDistance; 
            agent.angularSpeed = vehicleData.turnSpeed;
            agent.updateRotation = false; 
        }

        protected override void Update()
        {
            base.Update();

            if (fuelLevel <= 0)
            {
                fuelLevel = 0;
                if (agent.enabled && agent.isOnNavMesh) agent.isStopped = true;
                isMoving = false;
                return;
            }

            if (targetObject == null)
            {
                if (agent.enabled && agent.isOnNavMesh && agent.hasPath)
                {
                    agent.isStopped = true;
                    agent.ResetPath();
                }
                isMoving = false;
                return;
            }

            LookAtTarget();

            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(targetObject.transform.position);

                if (agent.remainingDistance > agent.stoppingDistance)
                {
                    isMoving = true;
                    ConsumeFuel();
                }
                else
                {
                    isMoving = false; 
                }
            }
            
            HandleCombat();

            UpdateSmokeEffect();
        }

        private void HandleCombat()
        {
            if (targetObject == null) return;

            float distanceToTarget = Vector3.Distance(transform.position, targetObject.transform.position);
            
            Vector3 dirToTarget = (targetObject.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToTarget);
            
            if (angle < 15f)
            {
                if (distanceToTarget <= rocketAttackRange)
                {
                    TryFireRockets();
                }

                if (distanceToTarget <= agent.stoppingDistance + 2.0f)
                {
                    TryFireBullets();
                }
            }
        }

        protected override void UpdateSmokeEffect()
        {
            if (smokeEffect == null) return;

            bool actuallyMoving = agent.velocity.sqrMagnitude > 0.1f;

            if (actuallyMoving && !smokeEffect.isPlaying)
            {
                smokeEffect.Play();
            }
            else if (!actuallyMoving && smokeEffect.isPlaying)
            {
                smokeEffect.Stop();
            }
        }
    }
}