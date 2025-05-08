using UnityEngine;

public class Ornithopter : BaseUnit
{
    public OrnithopterState currentState;
    public OrnithopterType ornithopterType;

    public float resilience = 1.0f;
    public float speed = 5.0f;
    public float takeoffSpeed = 2.0f;
    public float attackRange = 5.0f;
    public float fireDistance = 1.0f;
    public float attackAltitude = 2.0f;
    public float cruiseAltitude = 10.0f;
    public float fireRate = 1.0f;
    public float rotationSpeed = 2.0f;
    public float pitchAngle = 15.0f;
    public float rollAngle = 30.0f;
    public float lookAheadWeight = 0.5f;
    public Transform[] targets;

    public Transform BarrelTransform;
    public GameObject bulletPrefab;
    public float bulletSpeed = 50.0f;
    public float maxBarrelAngleX = 20.0f;
    public float inaccuracyRadius = 1.0f;

    private int currentTargetIndex = 0;
    private Vector3[] bezierPath;
    private int currentPathPoint = 0;
    private bool isTakingOff = false;
    private float nextFireTime = 0.0f;
    private Vector3 initialPosition;
    private float currentRollAngle = 0.0f;
    private float targetRollAngle = 0.0f;

    private void Start()
    {
        switch (ornithopterType)
        {
            case OrnithopterType.A:
                resilience = 0.8f;
                speed = 30.0f;
                break;
            case OrnithopterType.B:
                resilience = 1.2f;
                speed = 15.0f;
                break;
            default:
                resilience = 1.0f;
                speed = 6.0f;
                break;
        }

        currentState = OrnithopterState.Idle;
        initialPosition = transform.position;
        isTakingOff = true;

        if (targets.Length > 0)
        {
            GenerateBezierPath();
        }

        if (BarrelTransform != null)
        {
            BarrelTransform.localRotation = Quaternion.identity;
        }
    }

    private void Update()
    {
        switch (currentState)
        {
            case OrnithopterState.Idle:
                if (isTakingOff)
                {
                    TakeOff();
                }
                break;

            case OrnithopterState.Flying:
                FollowBezierPath();
                CheckTargetProximity();
                break;

            case OrnithopterState.Attacking:
                AttackTarget();
                break;

            case OrnithopterState.Landing:
                Land();
                break;
        }
    }

    private void TakeOff()
    {
        Vector3 takeoffTarget = initialPosition + (Vector3.up * cruiseAltitude + Vector3.forward * 20f);
        transform.position = Vector3.MoveTowards(transform.position, takeoffTarget, takeoffSpeed * Time.deltaTime);

        Vector3 direction = (takeoffTarget - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation *= Quaternion.Euler(pitchAngle, 0, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        if (Vector3.Distance(transform.position, takeoffTarget) < 0.1f)
        {
            isTakingOff = false;
            currentState = OrnithopterState.Flying;
        }
    }

    private void GenerateBezierPath()
    {
        if (targets.Length == 0) return;

        Vector3 startPoint = transform.position;
        Vector3 endPoint = targets[currentTargetIndex].position + Vector3.up * cruiseAltitude;
        float distance = Vector3.Distance(startPoint, endPoint);

        Vector3 controlPoint1 = startPoint + transform.forward.normalized * (distance * 0.3f) + Vector3.up * (cruiseAltitude * 0.2f);
        Vector3 directionToEnd = (endPoint - startPoint).normalized;
        Vector3 controlPoint2 = endPoint - directionToEnd * (distance * 0.3f) + Vector3.up * (cruiseAltitude * 0.2f);

        const int pointCount = 40;
        bezierPath = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            float t = i / (float)(pointCount - 1);
            bezierPath[i] = CalculateBezierPoint(t, startPoint, controlPoint1, controlPoint2, endPoint);
        }
        currentPathPoint = 0;
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * p0;
        point += 3 * uu * t * p1;
        point += 3 * u * tt * p2;
        point += ttt * p3;

        return point;
    }

    private void FollowBezierPath()
    {
        if (bezierPath == null || currentPathPoint >= bezierPath.Length) return;

        Vector3 targetPoint = bezierPath[currentPathPoint];
        Vector3 moveDirection = (targetPoint - transform.position).normalized;

        transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            targetRotation *= Quaternion.Euler(pitchAngle * (targetPoint.y - transform.position.y) / cruiseAltitude, 0, currentRollAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        {
            currentPathPoint++;
            if (currentPathPoint >= bezierPath.Length)
            {
                MoveToNextTarget();
            }
        }

        currentRollAngle = Mathf.Lerp(currentRollAngle, 0, Time.deltaTime * rotationSpeed);
    }

    private void CheckTargetProximity()
    {
        if (targets.Length == 0) return;

        Vector3 targetPos = targets[currentTargetIndex].position;
        float distanceToTarget = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                                                   new Vector3(targetPos.x, 0, targetPos.z));

        if (distanceToTarget <= attackRange)
        {
            currentState = OrnithopterState.Attacking;

            Vector3 targetDirection = (targetPos - transform.position).normalized;
            Vector3 cross = Vector3.Cross(transform.forward, targetDirection);
            targetRollAngle = cross.y > 0 ? rollAngle : -rollAngle;
        }
    }

    private void AttackTarget()
    {
        if (currentTargetIndex >= targets.Length) return;

        Vector3 targetPos = targets[currentTargetIndex].position;
        Vector3 attackPoint = new Vector3(targetPos.x, attackAltitude, targetPos.z);

        Vector3 attackDirection = (attackPoint - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, attackPoint, speed * Time.deltaTime);

        if (BarrelTransform != null)
        {
            Vector3 directionToTarget = (targetPos - BarrelTransform.position).normalized;
            Quaternion targetRotationBarrel = Quaternion.LookRotation(directionToTarget);

            float angleX = targetRotationBarrel.eulerAngles.x;
            if (angleX > 180) angleX -= 360;
            angleX = Mathf.Clamp(angleX, -maxBarrelAngleX, maxBarrelAngleX);

            float angleY = targetRotationBarrel.eulerAngles.y;

            Quaternion limitedRotation = Quaternion.Euler(angleX, angleY, 0);
            BarrelTransform.rotation = Quaternion.Slerp(BarrelTransform.rotation, limitedRotation, Time.deltaTime * 3);
        }

        Vector3 lookDirectionToTarget = (targetPos - transform.position).normalized;
        Vector3 lookDirectionToPath = attackDirection;

        Vector3 averagedDirection = Vector3.Lerp(lookDirectionToPath, lookDirectionToTarget, lookAheadWeight);
        if (averagedDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(averagedDirection);
            targetRotation *= Quaternion.Euler(-pitchAngle, 0, targetRollAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        float distanceToAttackPoint = Vector3.Distance(transform.position, attackPoint);
        float distanceToTarget = Vector3.Distance(BarrelTransform.position, targetPos);
        if (Time.time >= nextFireTime && distanceToAttackPoint < fireDistance && distanceToTarget < attackRange * 1.5f)
        {
            Fire(targetPos);
            nextFireTime = Time.time + 1f / fireRate;
        }

        float horizontalDistance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                                                     new Vector3(targetPos.x, 0, targetPos.z));
        if (horizontalDistance < 1.0f)
        {
            currentState = OrnithopterState.Flying;
            MoveToNextTarget();
        }

        currentRollAngle = Mathf.Lerp(currentRollAngle, 0, Time.deltaTime * rotationSpeed);

        Debug.DrawRay(transform.position, lookDirectionToTarget * 5f, Color.red, 0.1f);
        Debug.DrawRay(transform.position, lookDirectionToPath * 5f, Color.green, 0.1f);
        Debug.DrawRay(transform.position, averagedDirection * 5f, Color.blue, 0.1f);
        if (BarrelTransform != null)
        {
            Debug.DrawRay(BarrelTransform.position, BarrelTransform.forward * 5f, Color.yellow, 0.1f);
        }
    }

    private void MoveToNextTarget()
    {
        currentTargetIndex++;
        if (currentTargetIndex >= targets.Length)
        {
            currentTargetIndex = 0;
        }
        currentPathPoint = 0;
        GenerateBezierPath();
    }

    private void Fire(Vector3 targetPos)
    {
        Debug.Log("Firing at target " + currentTargetIndex);

        if (bulletPrefab != null && BarrelTransform != null)
        {
            Vector3 randomOffset = Random.insideUnitSphere * inaccuracyRadius;
            Vector3 adjustedTargetPos = targetPos + randomOffset;

            GameObject bullet = Instantiate(bulletPrefab, BarrelTransform.position, BarrelTransform.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                Vector3 bulletDirection = (adjustedTargetPos - BarrelTransform.position).normalized;
                bulletRb.linearVelocity = bulletDirection * bulletSpeed;

                Debug.DrawRay(BarrelTransform.position, bulletDirection * 5f, Color.cyan, 1f);
            }
        }
    }

    private void Land()
    {
        Vector3 landingTarget = initialPosition;
        transform.position = Vector3.MoveTowards(transform.position, landingTarget, speed * Time.deltaTime);

        Vector3 direction = (landingTarget - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation *= Quaternion.Euler(-pitchAngle, 0, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        if (Vector3.Distance(transform.position, landingTarget) < 0.1f)
        {
            currentState = OrnithopterState.Idle;
        }
    }

    public override void TakeDamage(float damageAmount)
    {
        damageAmount = damageAmount * resilience;
        base.TakeDamage(damageAmount);

        if (Health <= 0)
        {
            currentState = OrnithopterState.Landing;
        }
    }

    public enum OrnithopterState { Idle, Flying, Attacking, Landing }
    public enum OrnithopterType { A, B }
}