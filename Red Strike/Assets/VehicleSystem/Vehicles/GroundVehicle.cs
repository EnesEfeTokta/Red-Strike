using UnityEngine;
using VehicleSystem;
using UnityEngine.AI;

namespace VehicleSystem.Vehicles
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class GroundVehicle : Vehicle
    {
        private NavMeshAgent agent;

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
            if (fuelLevel <= 0)
            {
                fuelLevel = 0;
                if (agent.enabled) agent.isStopped = true;
                StopAttacking();
                isMoving = false;
                return;
            }

            if (targetObject == null)
            {
                if (agent.enabled && agent.hasPath)
                {
                    agent.isStopped = true;
                    agent.ResetPath();
                }
                StopAttacking();
                isMoving = false;
                return;
            }

            isMoving = true;
            LookAtTarget();

            if (agent.enabled)
            {
                agent.isStopped = false;
                agent.SetDestination(targetObject.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                {
                    if (!isAttacking) StartAttacking();
                }
                else
                {
                    StopAttacking();
                    ConsumeFuel();
                }
            }

            UpdateSmokeEffect();
        }

        protected override void UpdateSmokeEffect()
        {
            if (smokeEffect == null) return;

            bool actuallyMoving = agent.velocity.sqrMagnitude > 0.1f && isMoving;

            if (actuallyMoving && !smokeEffect.isPlaying) smokeEffect.Play();
            else if (!actuallyMoving && smokeEffect.isPlaying) smokeEffect.Stop();
        }
    }
}
