using UnityEngine;

namespace VehicleSystem.Vehicles
{
    public class AirVehicle : Vehicle
    {
        public float cruisingAltitude = 60f;
        public float attackAltitude = 20f;
        public float attackLobeRadius = 70f;
        public float rocketRangeMultiplier = 3.0f;
        public float loiterRadius = 40f;
        public float maxBankAngle = 35f;
        public float bankSpeed = 4f;
        private enum AirState { TakingOff, Loitering, Engaging, Attacking }
        private AirState currentState = AirState.TakingOff;
        private Vector3 loiterCenterPoint;
        private float loiterTimer = 0f;
        private bool orbitingRightLobe = true;
        private Vector3 attackAxis;
        private float currentBankAngleZ = 0f;

        [Tooltip("Hız, dönüş hızı gibi değerlerdeki +/- sapma oranı. 0.1 = %10 (UNUTMA SAKIN)")]
        [Range(0, 0.5f)]
        public float parameterVariance = 0.1f;
        public float altitudeVariance = 5f;

        private float effectiveSpeed;
        private float effectiveTurnSpeed;
        private float effectiveAttackRadius;
        private float effectiveRocketRange;
        private float altitudeOffset;

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

        protected override void Update()
        {
            base.Update();

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

        private void HandleTakingOff()
        {
            Vector3 takeoffTarget = new Vector3(transform.position.x, cruisingAltitude, transform.position.z);
            MoveAndLook(takeoffTarget, 1.0f);
            if (transform.position.y >= cruisingAltitude - 1.0f)
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
            if (orbitingRightLobe && sideDot < 0) { orbitingRightLobe = false; }
            else if (!orbitingRightLobe && sideDot > 0) { orbitingRightLobe = true; }

            Vector3 directionToFocal = (currentFocalPoint - myGroundPos).normalized;
            Vector3 tangentOffset = Vector3.Cross(directionToFocal, Vector3.up);
            Vector3 orbitPoint = currentFocalPoint + tangentOffset * attackLobeRadius;

            float facingRatio = Vector3.Dot(transform.forward, (targetGroundPos - myGroundPos).normalized);

            float desiredAltitude = Mathf.Lerp(cruisingAltitude + altitudeOffset, attackAltitude + altitudeOffset, (facingRatio + 1) / 2f);
            orbitPoint.y = desiredAltitude;

            float dist = Vector3.Distance(transform.position, targetObject.transform.position);

            if (facingRatio > 0.7f)
            {
                // Mermi menzili: effectiveAttackRadius civarı
                // Roket menzili: effectiveRocketRange
                bool canFireBullets = dist < effectiveAttackRadius * 1.2f; // Biraz tolerans
                bool canFireRockets = dist < effectiveRocketRange;

                CheckAndFireWeapons(dist, canFireRockets, canFireBullets);
            }

            if (facingRatio > 0.7f)
            {
                bool canFireBullets = dist < effectiveAttackRadius * 1.2f; // Biraz tolerans
                bool canFireRockets = dist < effectiveRocketRange;

                CheckAndFireWeapons(dist, canFireRockets, canFireBullets);
            }

            MoveAndLook(orbitPoint);
        }

        private void CheckAndFireWeapons(float distance, bool fireRockets, bool fireBullets)
        {
            // Hedefe yeterince dönük müyüz?
            Vector3 dirToTarget = (targetObject.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToTarget);

            if (angle < 20f) // Eğer hedefe 20 derece içinde bakıyorsak
            {
                if (fireRockets) TryFireRockets();
                if (fireBullets) TryFireBullets();
            }
        }

        private void MoveAndLook(Vector3 targetPosition, float turnMultiplier = 0.25f)
        {
            Vector3 directionToTarget = (targetPosition - transform.position).normalized;
            if (directionToTarget.sqrMagnitude > 0.01f)
            {
                transform.position += transform.forward * effectiveSpeed * Time.deltaTime;

                Vector3 localTargetDir = transform.InverseTransformDirection(directionToTarget);

                float targetBankAngle = -localTargetDir.x * maxBankAngle;

                currentBankAngleZ = Mathf.Lerp(currentBankAngleZ, targetBankAngle, Time.deltaTime * bankSpeed);

                Quaternion targetLookRotation = Quaternion.LookRotation(directionToTarget);

                Vector3 euler = targetLookRotation.eulerAngles;
                euler.z = currentBankAngleZ;

                Quaternion finalRotation = Quaternion.Euler(euler);

                transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, Time.deltaTime * effectiveTurnSpeed * turnMultiplier);
            }
        }
    }
}