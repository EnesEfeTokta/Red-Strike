using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using System.Collections.Generic;

public class Tank : BaseUnit
{
    public TankState tankState = TankState.Idle;
    public TankType tankType;
    public float resilience = 1.0f;

    [Header("Transforms")]
    public Transform BarrelTransform;
    public Transform CannonTransform;
    public Transform TurretTransform;
    public Transform TpsCameraTransform;

    [Header("Targeting & Movement (Free Look)")]
    public Transform CurrentFreeLookTarget { get; private set; }
    public List<Transform> PermanentFreeLookTargets { get; private set; } = new List<Transform>();
    public float turretRotationSpeed = 5f;
    public float barrelRotationSpeed = 3f;
    public float minBarrelAngle = -10f;
    public float maxBarrelAngle = 25f;
    private float distanceToTarget;
    public float stoppingDistance = 20f;
    public float engagementRange = 30.0f;

    [Header("Attack (References handled by TankAttack)")]

    [Header("Effects")]
    public ParticleSystem BarrelParticle;
    public ParticleSystem EngineParticle;

    [Header("UI")]
    public VehicleControlUI VehicleControlUI;

    private TankAttack attack;
    private SelectableVehicle selectableVehicle;

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
                if (VehicleControlUI != null) VehicleControlUI.OnSelect();
                if (CameraController.Instance != null)
                {
                    CameraController.Instance.selectedVehicle = transform;
                    CameraController.Instance.tpsCameraPosition = TpsCameraTransform;
                }
            }
            else
            {
                if (VehicleControlUI != null) VehicleControlUI.OnDeselect();
                if (CameraController.Instance != null && CameraController.Instance.selectedVehicle == transform)
                {
                    CameraController.Instance.selectedVehicle = null;
                    CameraController.Instance.tpsCameraPosition = null;
                }
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
                if (VehicleControlUI != null) VehicleControlUI.OnSelect();
                if (CameraController.Instance != null)
                {
                    CameraController.Instance.selectedVehicle = transform;
                    CameraController.Instance.tpsCameraPosition = TpsCameraTransform;
                }
            }
            else
            {
                if (VehicleControlUI != null) VehicleControlUI.OnDeselect();
                if (CameraController.Instance != null && CameraController.Instance.selectedVehicle == transform)
                {
                    CameraController.Instance.selectedVehicle = null;
                    CameraController.Instance.tpsCameraPosition = null;
                }
            }
        }
    }

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        attack = GetComponent<TankAttack>();
        selectableVehicle = GetComponent<SelectableVehicle>();

        if (attack == null)
        {
            Debug.LogError("TankAttack component'i bulunamadı!", this);
        }
        if (selectableVehicle == null)
        {
            Debug.LogError("SelectableVehicle component'i bulunamadı!", this);
        }

        switch (tankType)
        {
            case TankType.A:
                resilience = 0.8f; Health = maxHealth = 300.0f; Speed = 5.0f; AttackCooldown = maxAttackCooldown = 300.0f; Energy = maxEnergy = 500.0f; engagementRange = 30.0f;
                break;
            case TankType.B:
                resilience = 1.2f; Health = maxHealth = 220.0f; Speed = 9.0f; AttackCooldown = maxAttackCooldown = 200.0f; Energy = maxEnergy = 400.0f; engagementRange = 22.0f;
                break;
            case TankType.Combat:
                resilience = 1.5f; Health = maxHealth = 180.0f; Speed = 15.0f; AttackCooldown = maxAttackCooldown = 150.0f; Energy = maxEnergy = 300.0f; engagementRange = 20.0f;
                break;
            default:
                resilience = 1.0f; Health = maxHealth = 200.0f; Speed = 7.0f; AttackCooldown = maxAttackCooldown = 250.0f; Energy = maxEnergy = 450.0f; engagementRange = 25.0f;
                break;
        }

        tankState = TankState.Idle;
    }

    private void Update()
    {
        if (tankState == TankState.Destroyed) return;

        if (CameraController.Instance != null && CameraController.Instance.currentMode == CameraMode.ThirdPerson)
        {
            TpsMove();
            TpsTowerBarrelRotate();
            if (attack != null)
            {
                attack.HandleTPSAimingInput();
            }
        }
        else
        {
            HandleFreeLookBehavior();
        }
    }

    private void HandleFreeLookBehavior()
    {
        Transform currentTarget = GetPrioritizedFreeLookTarget();

        if (currentTarget != null)
        {
            distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

            RotateTurret(currentTarget.position);
            AdjustBarrel(currentTarget.position);

            if (distanceToTarget > engagementRange)
            {
                RotateTank(currentTarget.position);
                Move(currentTarget.position);
                tankState = TankState.Moving;
                if (EngineParticle != null && !EngineParticle.isPlaying) EngineParticle.Play();
            }
            else if (distanceToTarget > stoppingDistance)
            {
                Move(currentTarget.position);
                tankState = TankState.Moving;
                if (EngineParticle != null && !EngineParticle.isPlaying) EngineParticle.Play();
                EngageTarget(currentTarget);
            }
            else
            {
                tankState = TankState.Attacking;
                if (EngineParticle != null && EngineParticle.isPlaying) EngineParticle.Stop();
                EngageTarget(currentTarget);
            }
        }
        else
        {
            if (tankState == TankState.Moving || tankState == TankState.Attacking)
            {
                tankState = TankState.Idle;
                if (EngineParticle != null && EngineParticle.isPlaying) EngineParticle.Stop();
            }
            if (TurretTransform != null) RotateTurret(transform.position + transform.forward * 10f);
            if (BarrelTransform != null) AdjustBarrel(transform.position + transform.forward * 10f + Vector3.up * 2f);
        }
    }

    private void EngageTarget(Transform target)
    {
        if (attack == null || target == null) return;

        if (PermanentFreeLookTargets.Count > 0)
        {
            List<Transform> validTargets = new List<Transform>();
            foreach (var t in PermanentFreeLookTargets)
            {
                if (t != null) validTargets.Add(t);
            }
            if (validTargets.Count > 0)
                attack.HandleMultiTargetInput(validTargets.ToArray());
        }
        else if (CurrentFreeLookTarget != null)
        {
            attack.HandleSingleTargetInput(CurrentFreeLookTarget);
        }
    }

    public void SetFreeLookTarget(Transform target)
    {
        CurrentFreeLookTarget = target;
    }

    public void AddFreeLookTarget(Transform target)
    {
        if (target != null && !PermanentFreeLookTargets.Contains(target))
        {
            PermanentFreeLookTargets.Add(target);
            CurrentFreeLookTarget = null;
        }
    }

    public void RemoveFreeLookTarget(Transform target)
    {
        if (target == CurrentFreeLookTarget)
        {
            CurrentFreeLookTarget = null;
        }
        if (PermanentFreeLookTargets.Contains(target))
        {
            PermanentFreeLookTargets.Remove(target);
        }
    }

    public void ClearFreeLookTarget()
    {
        CurrentFreeLookTarget = null;
    }

    public void ClearAllFreeLookTargets()
    {
        CurrentFreeLookTarget = null;
        PermanentFreeLookTargets.Clear();
    }

    private Transform GetPrioritizedFreeLookTarget()
    {
        if (CurrentFreeLookTarget != null)
        {
            return CurrentFreeLookTarget;
        }
        else if (PermanentFreeLookTargets.Count > 0)
        {
            return GetClosestPermanentTarget();
        }
        return null;
    }

    private Transform GetClosestPermanentTarget()
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        List<Transform> targetsToRemove = new List<Transform>();
        foreach (Transform potentialTarget in PermanentFreeLookTargets)
        {
            if (potentialTarget == null)
            {
                targetsToRemove.Add(potentialTarget);
                continue;
            }
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        foreach (var targetToRemove in targetsToRemove)
        {
            PermanentFreeLookTargets.Remove(targetToRemove);
        }

        return bestTarget;
    }

    public override void TakeDamage(float damageAmount)
    {
        if (tankState == TankState.Destroyed) return;

        float adjustedDamage = damageAmount * resilience;
        float effectiveDamage = 0;

        if (AttackCooldown > 0)
        {
            if (AttackCooldown >= adjustedDamage) { AttackCooldown -= adjustedDamage; effectiveDamage = 0; } else { effectiveDamage = adjustedDamage - AttackCooldown; AttackCooldown = 0; }
        }
        else
        {
            effectiveDamage = adjustedDamage;
        }

        Health -= effectiveDamage;
        Health = Mathf.Max(Health, 0);

        if (VehicleControlUI != null)
            VehicleControlUI.UpdateUIProperties(Health, Energy, AttackCooldown, maxHealth, maxEnergy, maxAttackCooldown);

        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        tankState = TankState.Destroyed;
        if (EngineParticle != null) EngineParticle.Stop();
        if (BarrelParticle != null) BarrelParticle.Stop();

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // Optionally disable other components or start destroy sequence
        // Destroy(gameObject, 5f);
    }

    public override void Move(Vector3 targetPosition)
    {
        if (tankState == TankState.Destroyed) return;

        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        if (distanceToTarget > stoppingDistance)
        {
            transform.position += direction * Speed * Time.deltaTime;
        }
    }

    private void RotateTank(Vector3 targetPosition)
    {
        if (tankState == TankState.Destroyed) return;

        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turretRotationSpeed * 0.8f);
        }
    }

    private void RotateTurret(Vector3 targetPosition)
    {
        if (tankState == TankState.Destroyed || TurretTransform == null) return;

        Vector3 targetDirection = targetPosition - TurretTransform.position;
        targetDirection.y = 0;

        if (targetDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
            TurretTransform.rotation = Quaternion.Slerp(TurretTransform.rotation, lookRotation, Time.deltaTime * turretRotationSpeed);
        }
    }

    private void AdjustBarrel(Vector3 targetPosition)
    {
        if (tankState == TankState.Destroyed || BarrelTransform == null || TurretTransform == null) return;

        Vector3 worldDirectionToTarget = targetPosition - BarrelTransform.position;

        if (worldDirectionToTarget.sqrMagnitude < 0.01f) return;

        Vector3 localDirectionInTurret = TurretTransform.InverseTransformDirection(worldDirectionToTarget);

        float targetAngleRad = Mathf.Atan2(localDirectionInTurret.y, localDirectionInTurret.z);
        float targetAngleDeg = targetAngleRad * Mathf.Rad2Deg;

        float clampedAngle = Mathf.Clamp(targetAngleDeg, minBarrelAngle, maxBarrelAngle);

        Quaternion finalLocalRotation = Quaternion.Euler(-clampedAngle, 0, 0);

        BarrelTransform.localRotation = Quaternion.Slerp(BarrelTransform.localRotation, finalLocalRotation, Time.deltaTime * barrelRotationSpeed);
    }

    private void TpsTowerBarrelRotate()
    {
        if (tankState == TankState.Destroyed || mainCamera == null || TurretTransform == null || BarrelTransform == null || CameraController.Instance == null) return;

        float mouseX = Input.GetAxis("Mouse X") * CameraController.Instance.tpsSensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * CameraController.Instance.tpsSensitivityY;

        // Turret Rotation based on camera forward (horizontal only)
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 towerDirection = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
        if (towerDirection != Vector3.zero)
        {
            Quaternion towerRot = Quaternion.LookRotation(towerDirection);
            // Using world rotation, ensure Taret is not a child or handle local conversion if it is
            TurretTransform.rotation = Quaternion.Slerp(TurretTransform.rotation, towerRot, Time.deltaTime * turretRotationSpeed);
        }

        // Barrel Rotation based on mouse Y (vertical only)
        float currentLocalXAngle = BarrelTransform.localRotation.eulerAngles.x;
        if (currentLocalXAngle > 180) currentLocalXAngle -= 360;

        // Adjust angle based on mouse input (invert Y if needed)
        float targetAngle = currentLocalXAngle - mouseY; // Or + mouseY depending on desired control

        targetAngle = Mathf.Clamp(targetAngle, minBarrelAngle, maxBarrelAngle);

        Quaternion targetLocalRotation = Quaternion.Euler(targetAngle, 0, 0);
        BarrelTransform.localRotation = Quaternion.Slerp(BarrelTransform.localRotation, targetLocalRotation, Time.deltaTime * barrelRotationSpeed * 2f);
    }

    private void TpsMove()
    {
        if (tankState == TankState.Destroyed) return;

        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");
        bool isMoving = false;

        if (moveInput != 0)
        {
            float currentSpeed = Speed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed *= 1.7f;
            }
            transform.position += transform.forward * moveInput * currentSpeed * Time.deltaTime;
            isMoving = true;
        }

        if (turnInput != 0)
        {
            transform.Rotate(Vector3.up, turnInput * 50 * Time.deltaTime);
            isMoving = true;
        }

        if (isMoving)
        {
            // Only change state if not already attacking
            if (tankState != TankState.Attacking)
                tankState = TankState.Moving;

            if (EngineParticle != null && !EngineParticle.isPlaying)
            {
                EngineParticle.Play();
            }
        }
        else
        {
            // Only change state if not attacking
            if (tankState == TankState.Moving)
            {
                tankState = TankState.Idle;
            }

            if (EngineParticle != null && EngineParticle.isPlaying)
            {
                EngineParticle.Stop();
            }
        }
    }

    public enum TankState { Idle, Moving, Attacking, Destroyed }
    public enum TankType { A, B, Combat }
}