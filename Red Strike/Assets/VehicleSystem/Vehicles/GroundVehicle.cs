using UnityEngine;
using UnityEngine.AI;
using System.Linq;

namespace VehicleSystem.Vehicles
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class GroundVehicle : Vehicle
    {
        private NavMeshAgent agent;

        [Header("Ground Combat Settings")]
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

            if (fuelLevel <= 0 || isRefueling)
            {
                HandleRefuelingState();
                UpdateSmokeEffect();
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

            LookAtTarget(targetObject.transform.position);

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

        private void HandleRefuelingState()
        {
            isRefueling = true;

            if (nearestEnergyTower == null)
            {
                FindNearestEnergyTower();

                if (nearestEnergyTower == null)
                {
                    agent.isStopped = true;
                    isMoving = false;
                    return;
                }
            }

            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = false;

                if (Vector3.Distance(agent.destination, nearestEnergyTower.transform.position) > 1.0f)
                {
                    agent.SetDestination(nearestEnergyTower.transform.position);
                }

                isMoving = true;
                LookAtTarget(nearestEnergyTower.transform.position);

                float distToTower = Vector3.Distance(transform.position, nearestEnergyTower.transform.position);

                if (distToTower <= 10.0f)
                {
                    Refuel();
                }
            }
        }

        private void Refuel()
        {
            isMoving = false;
            agent.isStopped = true;

            fuelLevel += vehicleData.fuelCapacity * 0.2f * Time.deltaTime;

            if (fuelLevel >= vehicleData.fuelCapacity)
            {
                fuelLevel = vehicleData.fuelCapacity;
                isRefueling = false;
                nearestEnergyTower = null;
                agent.ResetPath();
            }
        }

        private void LookAtTarget(Vector3 targetPos)
        {
            Vector3 dirToTarget = (targetPos - transform.position).normalized;
            dirToTarget.y = 0;

            if (dirToTarget != Vector3.zero)
            {
                Quaternion lookRot = Quaternion.LookRotation(dirToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, vehicleData.turnSpeed * Time.deltaTime);
            }
        }

        private void HandleCombat()
        {
            if (targetObject == null) return;

            float distanceToTarget = Vector3.Distance(transform.position, targetObject.transform.position);

            Vector3 dirToTarget = targetObject.transform.position - transform.position;
            dirToTarget.y = 0;

            if (dirToTarget.sqrMagnitude < 0.001f) dirToTarget = transform.forward;

            float angle = Vector3.Angle(transform.forward, dirToTarget);

            if (angle < 20f)
            {
                if (distanceToTarget <= rocketAttackRange)
                {
                    TryFireRockets();
                }

                if (distanceToTarget <= agent.stoppingDistance + 5.0f)
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