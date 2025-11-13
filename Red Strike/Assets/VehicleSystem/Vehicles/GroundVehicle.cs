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
            base.Update();

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

            LookAtTarget();

            agent.SetDestination(targetObject.transform.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                isMoving = false;

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
    }
}
