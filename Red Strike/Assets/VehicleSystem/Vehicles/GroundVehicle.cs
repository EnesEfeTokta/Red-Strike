using UnityEngine;
using UnityEngine.AI;
using Fusion;

namespace VehicleSystem.Vehicles
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class GroundVehicle : Vehicle
    {
        private NavMeshAgent agent;

        [Header("Ground Combat Settings")]
        public float rocketAttackRange = 40f;
        private float bulletAttackBuffer = 5.0f; 

        private bool isExitingHangar = true; 
        private float lifeTime = 0f;
        private Vector3 startPosition;

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

        public override void Spawned()
        {
            base.Spawned();

            if (Object.HasStateAuthority)
            {
                isExitingHangar = true;
                lifeTime = 0f;
                startPosition = transform.position;
            }
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (!Object.HasStateAuthority) return;

            if (isExitingHangar)
            {
                lifeTime += Runner.DeltaTime;

                if (lifeTime < 2.5f)
                {
                    if(agent.enabled) agent.isStopped = true;
                    return; 
                }

                transform.position += transform.forward * vehicleData.speed * Runner.DeltaTime;
                
                if(agent.enabled) agent.nextPosition = transform.position;

                if (Vector3.Distance(startPosition, transform.position) >= 25.0f)
                {
                    isExitingHangar = false;
                    if(agent.enabled) agent.isStopped = true;
                }

                return;
            }

            if (fuelLevel <= 0 || isRefueling)
            {
                HandleRefuelingState();
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
                float dist = Vector3.Distance(transform.position, targetObject.transform.position);
                agent.SetDestination(targetObject.transform.position);

                if (dist > agent.stoppingDistance)
                {
                    agent.isStopped = false;
                    isMoving = true;
                    ConsumeFuel();
                }
                else
                {
                    agent.isStopped = true;
                    isMoving = false;
                    RotateTowardsTarget();
                }
            }

            HandleCombat();
        }

        protected override void Update()
        {
            base.Update();
            if (!Object.HasStateAuthority) { UpdateSmokeEffect(); return; }
            bool actuallyMoving = (isExitingHangar && lifeTime > 1.0f) || (agent != null && agent.enabled && agent.velocity.sqrMagnitude > 0.1f);
            engineSource.volume = Mathf.Lerp(engineSource.volume, actuallyMoving ? 0.3f : 0.05f, Time.deltaTime * 2f);
            UpdateSmokeEffect();
        }

        private void RotateTowardsTarget()
        {
            if (targetObject == null) return;
            Vector3 direction = (targetObject.transform.position - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Runner.DeltaTime * vehicleData.turnSpeed * 0.1f);
            }
        }

        private void HandleRefuelingState()
        {
            isRefueling = true;
            if (nearestEnergyTower == null)
            {
                FindNearestEnergyTower();
                if (nearestEnergyTower == null) { if (agent.enabled) agent.isStopped = true; isMoving = false; return; }
            }
            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(nearestEnergyTower.transform.position);
                isMoving = true;
                if (Vector3.Distance(transform.position, nearestEnergyTower.transform.position) <= 15.0f)
                {
                    agent.isStopped = true; isMoving = false; Refuel();
                }
            }
            vehicleUI.SetVehicleStatusIconToRefueling();
        }

        private void Refuel()
        {
            float requestedAmount = vehicleData.fuelCapacity * 0.2f * Runner.DeltaTime;
            float receivedAmount = 0f;
            if (targetTowerScript != null) receivedAmount = targetTowerScript.GiveEnergy(requestedAmount);
            fuelLevel += receivedAmount;
            if (fuelLevel >= vehicleData.fuelCapacity || (requestedAmount > 0 && receivedAmount <= 0))
            {
                fuelLevel = Mathf.Min(fuelLevel, vehicleData.fuelCapacity);
                isRefueling = false; DisconnectFromTower(); if (agent.enabled) agent.ResetPath();
            }
            if (fuelLevel > vehicleData.fuelCapacity / 4) vehicleUI.ClearVehicleStatusIcon();
        }

        private void HandleCombat()
        {
            if (targetObject == null) return;
            float distanceToTarget = Vector3.Distance(transform.position, targetObject.transform.position);
            Vector3 dirToTarget = (targetObject.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToTarget);
            if (angle < 30f)
            {
                if (distanceToTarget <= rocketAttackRange) TryFireRockets();
                if (distanceToTarget <= agent.stoppingDistance + bulletAttackBuffer) TryFireBullets();
            }
            else { if (!isMoving) RotateTowardsTarget(); }
        }
    }
}