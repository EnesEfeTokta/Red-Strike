using UnityEngine;
using Fusion;

namespace VehicleSystem.Vehicles
{
    public class AirVehicle : Vehicle
    {
        [Header("Flight Settings")]
        public float patrolAltitude = 40f;
        public float patrolRadius = 60f;

        [Header("Runway / Landing Settings")]
        public float approachDistance = 100f;
        public float approachAltitude = 40f;
        public float landingSpeed = 15f;

        [Header("Combat Settings (45 Degree Attack)")]
        public float attackAltitude = 50f;      // Saldırı başlangıç yüksekliği
        public float orbitRadius = 50f;         // Hedef etrafında dönme yarıçapı (Yükseklik ile aynı olursa 45 derece olur)
        public float orbitDuration = 3.0f;      // Saldırmadan önce en az kaç saniye dönsün
        public float pullUpAltitude = 20f;      // Yere ne kadar yaklaşınca saldırıyı kessin
        public float firingAngle = 30f;         // Saldırı konisi açısı
        public float rocketChance = 0.5f;

        [Header("Visuals")]
        public float bankAngle = 50f;
        public float turnSpeedSmooth = 2f;

        protected enum VehicleState { Spawning, Idle, Combat, Refueling }
        protected enum CombatPhase { PullingUp, Orbiting, Diving }
        protected enum RefuelPhase { Approach, FinalApproach, Landed, Takeoff }

        [Networked] protected VehicleState CurrentState { get; set; }
        [Networked] protected CombatPhase CurrentCombatPhase { get; set; }
        [Networked] protected RefuelPhase CurrentRefuelPhase { get; set; }

        [Networked] private Vector3 NetworkedPosition { get; set; }
        [Networked] private Quaternion NetworkedRotation { get; set; }

        private float _idleTimer;
        private float _combatOrbitTimer;
        private Vector3 _patrolCenter;
        private bool _hasFiredRocketInPass;
        private Vector3 _landingStartPoint;
        private Vector3 _landingEndPoint;

        public override void Spawned()
        {
            base.Spawned();
            if (Object.HasStateAuthority)
            {
                _patrolCenter = transform.position;
                CurrentState = VehicleState.Refueling;
                CurrentRefuelPhase = RefuelPhase.Takeoff;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (!Object.HasStateAuthority) return;

            if (fuelLevel <= 0 || isRefueling)
            {
                if (CurrentState != VehicleState.Refueling) StartRefuelingProcess();
                HandleRunwayRefueling();
            }
            else if (targetObject != null)
            {
                if (CurrentState != VehicleState.Combat)
                {
                    CurrentState = VehicleState.Combat;
                    CurrentCombatPhase = CombatPhase.Orbiting;
                    _combatOrbitTimer = 0f;
                }
                HandleCombatLogic();
                ConsumeFuel();
            }
            else
            {
                if (CurrentState != VehicleState.Idle)
                {
                    CurrentState = VehicleState.Idle;
                    _patrolCenter = new Vector3(transform.position.x, 0, transform.position.z);
                }
                HandleIdleLogic();
                ConsumeFuel();
            }

            NetworkedPosition = transform.position;
            NetworkedRotation = transform.rotation;
        }

        public override void Render()
        {
            if (!Object.HasStateAuthority)
            {
                float lerpSpeed = Time.deltaTime * 10f;
                transform.position = Vector3.Lerp(transform.position, NetworkedPosition, lerpSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, NetworkedRotation, lerpSpeed);
                UpdateEffectsAndSound();
            }
            else
            {
                UpdateEffectsAndSound();
            }
        }

        private void UpdateEffectsAndSound()
        {
            bool isEngineIdle = (CurrentState == VehicleState.Refueling && CurrentRefuelPhase == RefuelPhase.Landed);
            engineSource.volume = Mathf.Lerp(engineSource.volume, isEngineIdle ? 0.05f : 0.4f, Time.deltaTime * 2f);
            if (!isEngineIdle) UpdateSmokeEffect();
            else if (smokeEffect != null && smokeEffect.isPlaying) smokeEffect.Stop();
        }

        #region Flight Physics
        protected void FlyTowards(Vector3 targetPos, float speedMultiplier = 1.0f, bool limitPitch = false, float forcedBank = 0f)
        {
            Vector3 direction = (targetPos - transform.position).normalized;
            if (direction == Vector3.zero) direction = transform.forward;

            transform.position += transform.forward * speed * speedMultiplier * Runner.DeltaTime;

            Quaternion targetLook = Quaternion.LookRotation(direction);

            Vector3 localDir = transform.InverseTransformDirection(direction);
            float targetBank = (Mathf.Abs(forcedBank) > 0.1f) ? forcedBank : (-localDir.x * bankAngle);

            if (limitPitch)
            {
                Vector3 euler = targetLook.eulerAngles;
                if (euler.x > 180) euler.x -= 360;
                euler.x = Mathf.Clamp(euler.x, -30f, 30f);
                targetLook = Quaternion.Euler(euler);
            }

            Quaternion bankRotation = Quaternion.AngleAxis(targetBank, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetLook * bankRotation, Runner.DeltaTime * turnSpeedSmooth);
        }
        #endregion

        #region Combat Logic (Orbit & 45 Degree Dive)
        private void HandleCombatLogic()
        {
            if (targetObject == null) return;

            Vector3 targetPos = targetObject.transform.position;
            Vector3 myPos = transform.position;
            float distToTargetH = Vector3.Distance(new Vector3(myPos.x, 0, myPos.z), new Vector3(targetPos.x, 0, targetPos.z));
            float distToTargetTotal = Vector3.Distance(myPos, targetPos);

            switch (CurrentCombatPhase)
            {
                case CombatPhase.Orbiting:
                    _combatOrbitTimer += Runner.DeltaTime;

                    Vector3 dirToTarget = (targetPos - myPos).normalized;
                    Vector3 tangent = Vector3.Cross(dirToTarget, Vector3.up);

                    Vector3 orbitPoint = myPos + (tangent * 20f) + ((targetPos - myPos).normalized * (distToTargetH - orbitRadius));
                    orbitPoint.y = attackAltitude;

                    FlyTowards(orbitPoint, 1.0f, false, -30f);

                    bool atAltitude = myPos.y >= attackAltitude - 5f;
                    bool inPosition = Mathf.Abs(distToTargetH - orbitRadius) < 20f;

                    if (_combatOrbitTimer > orbitDuration && atAltitude && inPosition)
                    {
                        float angle = Vector3.Angle(transform.forward, targetPos - myPos);
                        if (angle < 90f)
                        {
                            CurrentCombatPhase = CombatPhase.Diving;
                            _hasFiredRocketInPass = false;
                        }
                    }
                    break;

                case CombatPhase.Diving:
                    FlyTowards(targetPos, 1.3f);
                    float attackAngle = Vector3.Angle(transform.forward, (targetPos - myPos).normalized);

                    if (attackAngle < firingAngle)
                    {
                        TryFireBullets();

                        if (!_hasFiredRocketInPass && distToTargetTotal < orbitRadius * 1.2f && Random.value < rocketChance)
                        {
                            TryFireRockets();
                            _hasFiredRocketInPass = true;
                        }
                    }

                    if (myPos.y < pullUpAltitude || distToTargetH < 10f)
                    {
                        CurrentCombatPhase = CombatPhase.PullingUp;
                    }
                    break;

                case CombatPhase.PullingUp:
                    Vector3 climbDir = transform.forward;
                    climbDir.y = 0;
                    Vector3 pullUpPoint = myPos + (climbDir * 40f) + (Vector3.up * 20f);
                    FlyTowards(pullUpPoint, 1.0f);

                    if (myPos.y > attackAltitude * 0.6f)
                    {
                        CurrentCombatPhase = CombatPhase.Orbiting;
                        _combatOrbitTimer = 0f;
                    }
                    break;
            }
        }
        #endregion

        #region Refueling & Idle Logic
        private void StartRefuelingProcess()
        {
            CurrentState = VehicleState.Refueling;
            CurrentRefuelPhase = RefuelPhase.Approach;
            FindNearestEnergyTower();
            CalculateLandingPath();
        }

        private void CalculateLandingPath()
        {
            if (nearestEnergyTower == null) return;
            Vector3 towerPos = nearestEnergyTower.transform.position;
            Vector3 dirFromTowerToMe = (transform.position - towerPos).normalized;
            dirFromTowerToMe.y = 0;
            _landingEndPoint = towerPos;
            _landingStartPoint = towerPos + (dirFromTowerToMe * approachDistance);
            _landingStartPoint.y = towerPos.y + approachAltitude;
        }

        private void HandleRunwayRefueling()
        {
            if (nearestEnergyTower == null && CurrentRefuelPhase != RefuelPhase.Takeoff)
                CurrentRefuelPhase = RefuelPhase.Takeoff;

            switch (CurrentRefuelPhase)
            {
                case RefuelPhase.Approach:
                    FlyTowards(_landingStartPoint, 1.0f);
                    if (Vector3.Distance(transform.position, _landingStartPoint) < 15f)
                        CurrentRefuelPhase = RefuelPhase.FinalApproach;
                    break;

                case RefuelPhase.FinalApproach:
                    float glideSpeedRatio = landingSpeed / speed;
                    Vector3 glideTarget = _landingEndPoint + Vector3.up * 2f;
                    FlyTowards(glideTarget, glideSpeedRatio, true);
                    if (Vector3.Distance(transform.position, glideTarget) < 4f)
                        CurrentRefuelPhase = RefuelPhase.Landed;
                    break;

                case RefuelPhase.Landed:
                    isRefueling = true;
                    Quaternion flatRot = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                    transform.rotation = Quaternion.Slerp(transform.rotation, flatRot, Runner.DeltaTime * 2f);
                    float requested = vehicleData.fuelCapacity * 0.25f * Runner.DeltaTime;
                    float received = (targetTowerScript != null) ? targetTowerScript.GiveEnergy(requested) : 0f;
                    fuelLevel += received;
                    if (fuelLevel >= vehicleData.fuelCapacity * 0.99f)
                    {
                        fuelLevel = vehicleData.fuelCapacity;
                        isRefueling = false;
                        DisconnectFromTower();
                        CurrentRefuelPhase = RefuelPhase.Takeoff;
                    }
                    break;

                case RefuelPhase.Takeoff:
                    Vector3 takeoffPoint = transform.position + (transform.forward * 50f) + (Vector3.up * 30f);
                    if (transform.position.y >= patrolAltitude)
                    {
                        CurrentState = VehicleState.Idle;
                        _patrolCenter = transform.position;
                    }
                    else FlyTowards(takeoffPoint, 1.0f, true);
                    break;
            }
        }

        private void HandleIdleLogic()
        {
            _idleTimer += Runner.DeltaTime * 0.4f;
            float scale = patrolRadius;
            float sinT = Mathf.Sin(_idleTimer);
            float cosT = Mathf.Cos(_idleTimer);
            float denom = 1 + (sinT * sinT);
            Vector3 targetPoint = _patrolCenter + new Vector3((scale * cosT) / denom, patrolAltitude, (scale * sinT * cosT) / denom);
            FlyTowards(targetPoint, 0.7f, true);
        }
        #endregion
    }
}