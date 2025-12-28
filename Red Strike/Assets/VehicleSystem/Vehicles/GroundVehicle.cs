using UnityEngine;
using UnityEngine.AI;

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
            
            agent.updateRotation = true;
            agent.updatePosition = true;

            if (!Object.HasStateAuthority)
            {
                agent.enabled = false;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!Object.HasStateAuthority)
            {
                UpdateSmokeEffect();
                return;
            }

            engineSource.volume = Mathf.Lerp(engineSource.volume, isMoving ? 0.3f : 0.05f, Time.deltaTime * 2f);

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
                TargetNetworkId = default;
                return;
            }

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
                    if (agent.enabled) agent.isStopped = true;
                    isMoving = false;
                    return;
                }
            }

            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = false;

                if (Vector3.Distance(agent.destination, nearestEnergyTower.transform.position) > 2.0f)
                {
                    agent.SetDestination(nearestEnergyTower.transform.position);
                }

                isMoving = true;

                float distToTower = Vector3.Distance(transform.position, nearestEnergyTower.transform.position);

                if (distToTower <= 20.0f)
                {
                    Refuel();
                }
            }

            vehicleUI.SetVehicleStatusIconToRefueling();
        }

        private void Refuel()
        {
            isMoving = false;
            if (agent.enabled) agent.isStopped = true;

            float requestedAmount = vehicleData.fuelCapacity * 0.2f * Time.deltaTime;

            float receivedAmount = 0f;
            if (targetTowerScript != null)
            {
                receivedAmount = targetTowerScript.GiveEnergy(requestedAmount);
            }

            fuelLevel += receivedAmount;

            if (fuelLevel >= vehicleData.fuelCapacity || (requestedAmount > 0 && receivedAmount <= 0))
            {
                fuelLevel = Mathf.Min(fuelLevel, vehicleData.fuelCapacity);

                isRefueling = false;
                DisconnectFromTower();

                if (agent.enabled) agent.ResetPath();
            }

            if (fuelLevel > vehicleData.fuelCapacity / 4)
            {
                vehicleUI.ClearVehicleStatusIcon();
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

            bool actuallyMoving = false;
            
            if (Object.HasStateAuthority && agent != null && agent.enabled)
            {
                actuallyMoving = agent.velocity.sqrMagnitude > 0.1f;
            }
            else
            {
                actuallyMoving = isMoving;
            }

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