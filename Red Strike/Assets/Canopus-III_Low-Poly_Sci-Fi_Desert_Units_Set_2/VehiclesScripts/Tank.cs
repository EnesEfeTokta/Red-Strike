using UnityEngine;
using DG.Tweening;

public class Tank : BaseUnit
{
    public TankState tankState;
    public TankType tankType;
    public float resilience = 1.0f;

    public Transform BarrelTransform; // Ball barrel (up-down moves)
    public Transform CannonTransform; // The center of the ball (only up-down moves)
    public Transform TurretTransform; // Tower (Right-Sola Returns)

    public Transform TargetTransform; // Aim
    public float turretRotationSpeed = 5f;
    public float barrelRotationSpeed = 3f;
    public float minBarrelAngle = -10f; // Down limit
    public float maxBarrelAngle = 25f; // Upstream

    private float distanceTarget; // Distance to the target.
    public float stoppingDistance = 20f; // Distance to stop from the target.

    public GameObject projectilePrefab; // Mermi prefabı
    public float projectileSpeed = 20f; // Mermi hızı
    private float attackTimer = 0f;

    public ParticleSystem BarrelParticle; // Barrel particle system (for example, smoke or fire effect)
    public ParticleSystem EngineParticle; // Cannon particle system (for example, smoke or fire effect)

    private void Start()
    {
        switch (tankType)
        {
            case TankType.A:
                resilience = 0.8f;
                break;
            case TankType.B:
                resilience = 1.2f;
                break;
            case TankType.Combat:
                resilience = 1.5f;
                break;
            default:
                resilience = 1.0f;
                break;
        }

        tankState = TankState.Idle;

        Speed = 5f;
        AttackCooldown = 1.5f;
        Range = 30f;
    }

    private void Update()
    {
        if (TargetTransform != null)
        {
            distanceTarget = Vector3.Distance(transform.position, TargetTransform.position);

            RotateTurret(TargetTransform.position);
            AdjustBarrel(TargetTransform.position);
            RotateTank(TargetTransform.position);

            if (distanceTarget <= stoppingDistance)
            {
                tankState = TankState.Idle;
                EngineParticle.Stop(); // Engine particle effect
            }
            else if (distanceTarget <= Range)
            {
                tankState = TankState.Attacking;
                Attack();
            }
            else
            {
                tankState = TankState.Moving;
                Move(TargetTransform.position);
                EngineParticle.Play(); // Engine particle effect
            }
        }
    }

    public override void TakeDamage(float damageAmount)
    {
        damageAmount *= resilience;
        base.TakeDamage(damageAmount);
    }

    private void Attack()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= AttackCooldown)
        {
            if (projectilePrefab != null)
            {
                BarrelParticle.Play();
                EngineParticle.Stop();

                GameObject projectile = Instantiate(projectilePrefab, CannonTransform.position, CannonTransform.rotation);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.linearVelocity = CannonTransform.forward * projectileSpeed;
                }
                Destroy(projectile, 5f);
                attackTimer = 0f;

                BarrelTransform.DOLocalMoveZ(-0.5f, 0.1f).SetRelative().SetEase(Ease.OutQuad)
                        .OnComplete(() => BarrelTransform.DOLocalMoveZ(0.5f, 0.2f).SetRelative().SetEase(Ease.OutBounce));
            }
        }
    }

    public override void Move(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;
        if (distanceTarget > stoppingDistance)
        {
            transform.position += direction * Speed * Time.deltaTime;
        }
    }

    private void RotateTank(Vector3 position)
    {
        Vector3 direction = position - transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turretRotationSpeed);
    }

    private void RotateTurret(Vector3 targetPosition)
    {
        Vector3 targetDirection = targetPosition - TurretTransform.position;
        targetDirection.y = 0; // Y eksenini sıfırla, sadece yatay dönüş sağla.

        if (targetDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
            TurretTransform.rotation = Quaternion.Slerp(TurretTransform.rotation, lookRotation, Time.deltaTime * turretRotationSpeed);
        }
    }

    private void AdjustBarrel(Vector3 targetPosition)
    {
        Vector3 directionToTarget = targetPosition - BarrelTransform.position;

        // Sadece namlunun dikey açısını değiştirmek için Y eksenini sıfırla
        directionToTarget.x = 0;

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        float angle = targetRotation.eulerAngles.x;
        if (angle > 180) angle -= 360;
        angle = Mathf.Clamp(angle, minBarrelAngle, maxBarrelAngle);

        Quaternion limitedRotation = Quaternion.Euler(angle, BarrelTransform.localRotation.eulerAngles.y, BarrelTransform.localRotation.eulerAngles.z);
        BarrelTransform.localRotation = Quaternion.Slerp(BarrelTransform.localRotation, limitedRotation, Time.deltaTime * barrelRotationSpeed);
    }

    public enum TankState { Idle, Moving, Attacking, Destroyed }
    public enum TankType { A, B, Combat }
}
