using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class Tank : BaseUnit
{
    public TankState tankState = TankState.Idle;
    public TankType tankType;
    public float resilience = 1.0f;

    public Transform BarrelTransform; // Ball barrel (up-down moves)
    public Transform CannonTransform; // The center of the ball (only up-down moves)
    public Transform TurretTransform; // Tower (Right-Sola Returns)
    public Transform TpsCameraTransform; // TPS camera position (for example, behind the tank)

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

    public VehicleControlUI VehicleControlUI;

    private Attack attack;

    private float maxHealth;
    private float maxEnergy;
    private float maxAttackCooldown;

    private bool isSingleSelection { get; set; } = false;
    public bool IsSingleSelection
    {
        get { return isSingleSelection; }
        set
        {
            isSingleSelection = value;
            isDoubleSelection = false;
            if (value)
            {
                VehicleControlUI.OnSelect();
                CameraController.Instance.selectedVehicle = transform;
                CameraController.Instance.tpsCameraPosition = TpsCameraTransform;
            }
            else
            {
                VehicleControlUI.OnDeselect();
                CameraController.Instance.selectedVehicle = null;
                CameraController.Instance.tpsCameraPosition = null;
            }
        }
    }

    private bool isDoubleSelection { get; set; } = false;
    public bool IsDoubleSelection
    {
        get { return isDoubleSelection; }
        set
        {
            isDoubleSelection = value;
            isSingleSelection = false;
            if (value)
            {
                VehicleControlUI.OnSelect();
                CameraController.Instance.selectedVehicle = transform;
                CameraController.Instance.tpsCameraPosition = TpsCameraTransform;
            }
            else
            {
                VehicleControlUI.OnDeselect();
                CameraController.Instance.selectedVehicle = null;
                CameraController.Instance.tpsCameraPosition = null;
            }
        }
    }

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        attack = GetComponent<Attack>();

        switch (tankType)
        {
            case TankType.A:
                resilience = 0.8f;
                Health = maxHealth = 300.0f;
                Speed = 5.0f;
                AttackCooldown = maxAttackCooldown = 300.0f;
                Energy = maxEnergy = 500.0f;
                Range = 30.0f;
                RepeatShot = 1.5f;
                break;
            case TankType.B:
                resilience = 1.2f;
                Health = maxHealth = 220.0f;
                Speed = 9.0f;
                AttackCooldown = maxAttackCooldown = 200.0f;
                Energy = maxEnergy = 400.0f;
                Range = 22.0f;
                RepeatShot = 0.1f;
                break;
            case TankType.Combat:
                resilience = 1.5f;
                Health = maxHealth = 180.0f;
                Speed = 15.0f;
                AttackCooldown = maxAttackCooldown = 150.0f;
                Energy = maxEnergy = 300.0f;
                Range = 20.0f;
                RepeatShot = 0.5f;
                break;
            default:
                resilience = 1.0f;
                Health = maxHealth = 200.0f;
                Speed = 7.0f;
                AttackCooldown = maxAttackCooldown = 250.0f;
                Energy = maxEnergy = 450.0f;
                Range = 25.0f;
                RepeatShot = 1.0f;
                break;
        }

        tankState = TankState.Idle;
    }

    private void Update()
    {
        if (CameraController.Instance.currentMode != CameraMode.FreeLook)
        {
            TpsMove();
            TpsTowerBarrelRotate();
        }
        else if (CameraController.Instance.currentMode == CameraMode.FreeLook)
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

            if (Input.GetKeyDown(KeyCode.Space))
            {
                TakeDamage(20);
            }
        }
    }

    public override void TakeDamage(float damageAmount)
    {
        float adjustedDamage = damageAmount * resilience;
        float effectiveDamage = 0;

        if (AttackCooldown > 0)
        {
            if (AttackCooldown >= adjustedDamage)
            {
                AttackCooldown -= adjustedDamage;
                effectiveDamage = 0;
            }
            else
            {
                effectiveDamage = adjustedDamage - AttackCooldown;
                AttackCooldown = 0;
            }
        }
        else
        {
            effectiveDamage = adjustedDamage;
        }

        Health -= effectiveDamage;
        Health = Mathf.Max(Health, 0);

        VehicleControlUI.UpdateUIProperties(Health, Energy, AttackCooldown, maxHealth, maxEnergy, maxAttackCooldown);

        if (Health <= 0)
        {
            tankState = TankState.Destroyed;
            EngineParticle.Stop();
            BarrelParticle.Stop();
        }
    }

    private void Attack()
    {/*
        attackTimer += Time.deltaTime;
        if (attackTimer >= RepeatShot)
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
        */
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
        if (distanceTarget >= Range)
        {
            Vector3 direction = position - transform.position;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turretRotationSpeed);
        }
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

    private void TpsTowerBarrelRotate()
    {
        Vector3 cameraForward = mainCamera.transform.forward;

        Vector3 towerDirection = new Vector3(cameraForward.x, 0, cameraForward.z);
        Quaternion towerRot = Quaternion.LookRotation(towerDirection);

        TurretTransform.rotation = Quaternion.Slerp(TurretTransform.rotation, towerRot, Time.deltaTime * turretRotationSpeed);

        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

        Vector2 crosshairPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        float normalizedCrosshairY = (crosshairPosition.y - screenCenter.y) / (Screen.height / 2);

        float targetAngle = normalizedCrosshairY * maxBarrelAngle;

        targetAngle = Mathf.Clamp(targetAngle, -40f, 10f);

        Quaternion targetRotation = Quaternion.Euler(-targetAngle, BarrelTransform.localRotation.eulerAngles.y, BarrelTransform.localRotation.eulerAngles.z);
        BarrelTransform.localRotation = Quaternion.Slerp(BarrelTransform.localRotation, targetRotation, Time.deltaTime * 10f);

    }

    private void TpsMove()
    {
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        if (moveInput != 0)
        {
            transform.position += transform.forward * moveInput * Speed * Time.deltaTime;
            tankState = TankState.Moving;
            if (!EngineParticle.isPlaying)
            {
                EngineParticle.Play();
            }
        }

        if (turnInput != 0)
        {
            transform.Rotate(Vector3.up, turnInput * 50 * Time.deltaTime);
            tankState = TankState.Moving;
            if (!EngineParticle.isPlaying)
            {
                EngineParticle.Play();
            }
        }

        if (moveInput == 0 && turnInput == 0)
        {
            tankState = TankState.Idle;
            if (EngineParticle.isPlaying)
            {
                EngineParticle.Stop();
            }
        }
    }

    public enum TankState { Idle, Moving, Attacking, Destroyed }
    public enum TankType { A, B, Combat }
}
