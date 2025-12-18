using UnityEngine;
using Fusion;

namespace VehicleSystem.Vehicles
{
    public class AirVehicle : Vehicle
    {
        [Header("Air Combat Settings")]
        public float cruisingAltitude = 60f;
        public float attackAltitude = 20f;
        public float attackLobeRadius = 70f;
        public float rocketRangeMultiplier = 3.0f;
        public float loiterRadius = 40f;
        public float maxBankAngle = 35f;
        public float bankSpeed = 4f;

        private enum AirState { TakingOff, Loitering, Engaging, Attacking, Refueling }
        private AirState currentState = AirState.TakingOff;

        private enum RefuelStage { Calculating, Approaching, Gliding, Landed }
        private RefuelStage currentRefuelStage = RefuelStage.Calculating;

        private Vector3 approachPoint;
        private Vector3 landingPoint;

        private Vector3 loiterCenterPoint;
        private float loiterTimer = 0f;
        private bool orbitingRightLobe = true;
        private Vector3 attackAxis;
        private float currentBankAngleZ = 0f;

        [Range(0, 0.5f)]
        public float parameterVariance = 0.1f;
        public float altitudeVariance = 5f;

        private float effectiveSpeed;
        private float effectiveTurnSpeed;
        private float effectiveAttackRadius;
        private float effectiveRocketRange;
        private float altitudeOffset;

        [Networked] private Vector3 NetworkedPosition { get; set; }
        [Networked] private Quaternion NetworkedRotation { get; set; }

        protected override void Start()
        {
            base.Start();

            loiterCenterPoint = new Vector3(transform.position.x, cruisingAltitude, transform.position.z);
            loiterTimer = Random.Range(0f, 100f);

            effectiveSpeed = vehicleData.speed * Random.Range(1f - parameterVariance, 1f + parameterVariance);
            effectiveTurnSpeed = vehicleData.turnSpeed * Random.Range(1f - parameterVariance, 1f + parameterVariance);
            effectiveAttackRadius = attackLobeRadius * Random.Range(1f - parameterVariance, 1f + parameterVariance);
            effectiveRocketRange = effectiveAttackRadius * rocketRangeMultiplier;
            altitudeOffset = Random.Range(-altitudeVariance, altitudeVariance);
        }

        public override void FixedUpdateNetwork()
        {
            if (!Object.HasStateAuthority) return;

            if (fuelLevel <= 0 || isRefueling)
            {
                if (currentState != AirState.Refueling)
                {
                    currentState = AirState.Refueling;
                    currentRefuelStage = RefuelStage.Calculating;
                }
                HandleRefueling();
            }
            else
            {
                // Durum makinesi
                switch (currentState)
                {
                    case AirState.TakingOff: HandleTakingOff(); break;
                    case AirState.Loitering: HandleLoitering(); break;
                    case AirState.Engaging: HandleEngaging(); break;
                    case AirState.Attacking: HandleAttacking(); break;
                }

                isMoving = true;
                ConsumeFuel();
            }

            // Konumu Network'e yaz
            NetworkedPosition = transform.position;
            NetworkedRotation = transform.rotation;
        }

        protected override void Update()
        {
            base.Update();

            if (!Object.HasStateAuthority)
            {
                transform.position = Vector3.Lerp(transform.position, NetworkedPosition, Time.deltaTime * 5f);
                transform.rotation = Quaternion.Slerp(transform.rotation, NetworkedRotation, Time.deltaTime * 5f);
                return;
            }

            if (fuelLevel <= 0 || isRefueling)
            {
                if (currentState != AirState.Refueling)
                {
                    currentState = AirState.Refueling;
                    currentRefuelStage = RefuelStage.Calculating;
                }

                HandleRefueling();
                return;
            }

            switch (currentState)
            {
                case AirState.TakingOff: HandleTakingOff(); break;
                case AirState.Loitering: HandleLoitering(); break;
                case AirState.Engaging: HandleEngaging(); break;
                case AirState.Attacking: HandleAttacking(); break;
            }

            isMoving = true;
            ConsumeFuel();
        }

        private void MoveAndLook(Vector3 targetPosition, float turnMultiplier = 0.25f)
        {
            Vector3 directionToTarget = (targetPosition - transform.position).normalized;
            if (directionToTarget.sqrMagnitude > 0.01f)
            {
                float dt = Runner.DeltaTime;

                transform.position += transform.forward * effectiveSpeed * dt;

                Vector3 localTargetDir = transform.InverseTransformDirection(directionToTarget);
                float targetBankAngle = -localTargetDir.x * maxBankAngle;
                currentBankAngleZ = Mathf.Lerp(currentBankAngleZ, targetBankAngle, dt * bankSpeed);

                Quaternion targetLookRotation = Quaternion.LookRotation(directionToTarget);
                Vector3 euler = targetLookRotation.eulerAngles;
                euler.z = currentBankAngleZ;

                Quaternion finalRotation = Quaternion.Euler(euler);
                transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, dt * effectiveTurnSpeed * turnMultiplier);
            }
        }

        private void HandleRefueling()
        {
            isRefueling = true;

            if (nearestEnergyTower == null)
            {
                FindNearestEnergyTower();

                if (nearestEnergyTower == null)
                {
                    return;
                }
            }

            switch (currentRefuelStage)
            {
                case RefuelStage.Calculating:
                    CalculateLandingPath();
                    break;
                case RefuelStage.Approaching:
                    ExecuteApproach();
                    break;
                case RefuelStage.Gliding:
                    ExecuteGlide();
                    break;
                case RefuelStage.Landed:
                    ExecuteRefuel();
                    break;
            }
        }

        private void CalculateLandingPath()
        {
            Vector3 towerPos = nearestEnergyTower.transform.position;
            Vector3 myPos = transform.position;

            Vector3 directionFromTower = (myPos - towerPos).normalized;
            landingPoint = towerPos + (directionFromTower * 10f);
            landingPoint.y = towerPos.y;

            approachPoint = landingPoint + (directionFromTower * 80f);
            approachPoint.y = cruisingAltitude;

            currentRefuelStage = RefuelStage.Approaching;
        }

        private void ExecuteApproach()
        {
            MoveAndLook(approachPoint, 1.0f);

            if (Vector3.Distance(transform.position, approachPoint) < 5.0f)
            {
                currentRefuelStage = RefuelStage.Gliding;
            }
        }

        private void ExecuteGlide()
        {
            isMoving = true;
            float glideSpeed = effectiveSpeed * 0.6f;

            transform.position = Vector3.MoveTowards(transform.position, landingPoint, glideSpeed * Time.deltaTime);

            Vector3 vectorToLanding = landingPoint - transform.position;
            Vector3 flatDirection = vectorToLanding;
            flatDirection.y = 0;

            if (flatDirection != Vector3.zero)
            {
                // A) Hedefe doğrudan bakış (Burun aşağı)
                Quaternion diveRotation = Quaternion.LookRotation(vectorToLanding);

                // B) Ufka düz bakış (Yere paralel)
                Quaternion flatRotation = Quaternion.LookRotation(flatDirection);

                // C) Yükseklik farkı
                float altitude = transform.position.y - landingPoint.y;

                // D) Karışım Oranı: 
                float blendFactor = Mathf.Clamp01(altitude / 15.0f);

                // Rotasyonları karıştır: Yere yaklaştıkça burnunu kaldırır
                Quaternion targetRot = Quaternion.Lerp(flatRotation, diveRotation, blendFactor);

                // Yumuşak geçiş uygula
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 3f);
            }

            currentBankAngleZ = Mathf.Lerp(currentBankAngleZ, 0, Time.deltaTime * bankSpeed);

            // 3. Yere temas kontrolü
            if (Vector3.Distance(transform.position, landingPoint) < 0.5f)
            {
                currentRefuelStage = RefuelStage.Landed;
            }
        }

        private void ExecuteRefuel()
        {
            isMoving = false;

            Vector3 euler = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0, euler.y, 0);

            float requestedAmount = vehicleData.fuelCapacity * 0.2f * Time.deltaTime; // TODO: Ayarlanabilir, ScriptableObject'tan çekilebilir

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

                currentState = AirState.TakingOff;

                Vector3 takeoffDir = transform.forward;

                Vector3 takeoffPoint = transform.position + (takeoffDir * 60f);
                takeoffPoint.y = cruisingAltitude;

                loiterCenterPoint = new Vector3(transform.position.x, cruisingAltitude, transform.position.z);

                currentRefuelStage = RefuelStage.Calculating;
            }
        }

        private void HandleTakingOff()
        {
            Vector3 takeoffTarget = transform.position + (transform.forward * 50f) + (Vector3.up * 20f);

            if (takeoffTarget.y > cruisingAltitude) takeoffTarget.y = cruisingAltitude;

            MoveAndLook(takeoffTarget, 0.5f);

            if (transform.position.y >= cruisingAltitude - 5.0f)
            {
                currentState = AirState.Loitering;
                loiterCenterPoint = new Vector3(transform.position.x, cruisingAltitude, transform.position.z);
            }
        }

        private void HandleLoitering()
        {
            if (targetObject != null) { currentState = AirState.Engaging; return; }
            loiterTimer += Time.deltaTime;
            float xOffset = Mathf.Sin(loiterTimer * 0.4f) * loiterRadius;
            float zOffset = Mathf.Cos(loiterTimer * 0.8f) * loiterRadius;
            Vector3 targetPos = loiterCenterPoint + new Vector3(xOffset, 0, zOffset);
            MoveAndLook(targetPos);
        }

        private void HandleEngaging()
        {
            if (targetObject == null) { currentState = AirState.Loitering; return; }

            float distanceToTarget = Vector3.Distance(transform.position, targetObject.transform.position);

            if (distanceToTarget <= effectiveRocketRange)
            {
                CheckAndFireWeapons(distanceToTarget, fireRockets: true, fireBullets: false);
            }

            if (distanceToTarget <= effectiveAttackRadius * 1.5f)
            {
                Vector3 directionToTarget = (targetObject.transform.position - transform.position).normalized;
                attackAxis = Vector3.Cross(directionToTarget, Vector3.up).normalized;
                currentState = AirState.Attacking;
                return;
            }

            Vector3 targetPos = targetObject.transform.position;
            targetPos.y = cruisingAltitude + altitudeOffset;
            MoveAndLook(targetPos);
        }

        private void HandleAttacking()
        {
            if (targetObject == null) { currentState = AirState.Loitering; return; }

            Vector3 targetGroundPos = new Vector3(targetObject.transform.position.x, 0, targetObject.transform.position.z);
            Vector3 myGroundPos = new Vector3(transform.position.x, 0, transform.position.z);

            Vector3 focalPointRight = targetGroundPos + attackAxis * effectiveAttackRadius;
            Vector3 focalPointLeft = targetGroundPos - attackAxis * effectiveAttackRadius;
            Vector3 currentFocalPoint = orbitingRightLobe ? focalPointRight : focalPointLeft;

            Vector3 fromTargetToMe = myGroundPos - targetGroundPos;
            float sideDot = Vector3.Dot(fromTargetToMe, attackAxis);
            if (orbitingRightLobe && sideDot < 0) orbitingRightLobe = false;
            else if (!orbitingRightLobe && sideDot > 0) orbitingRightLobe = true;

            Vector3 directionToFocal = (currentFocalPoint - myGroundPos).normalized;
            Vector3 tangentOffset = Vector3.Cross(directionToFocal, Vector3.up);
            Vector3 orbitPoint = currentFocalPoint + tangentOffset * attackLobeRadius;

            float facingRatio = Vector3.Dot(transform.forward, (targetGroundPos - myGroundPos).normalized);
            float desiredAltitude = Mathf.Lerp(cruisingAltitude + altitudeOffset, attackAltitude + altitudeOffset, (facingRatio + 1) / 2f);
            orbitPoint.y = desiredAltitude;
            float dist = Vector3.Distance(transform.position, targetObject.transform.position);

            if (facingRatio > 0.7f)
            {
                bool canFireBullets = dist < effectiveAttackRadius * 1.2f;
                bool canFireRockets = dist < effectiveRocketRange;
                CheckAndFireWeapons(dist, canFireRockets, canFireBullets);
            }
            MoveAndLook(orbitPoint);
        }

        private void CheckAndFireWeapons(float distance, bool fireRockets, bool fireBullets)
        {
            Vector3 dirToTarget = (targetObject.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToTarget);

            if (angle < 20f)
            {
                if (fireRockets) TryFireRockets();
                if (fireBullets) TryFireBullets();
            }
        }
    }
}