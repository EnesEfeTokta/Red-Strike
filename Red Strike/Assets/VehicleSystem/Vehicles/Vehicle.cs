using UnityEngine;
using AmmunitionSystem;
using System.Linq;
using BuildingPlacement.Buildings;
using Fusion;
using GameStateSystem;
using NetworkingSystem;

namespace VehicleSystem.Vehicles
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(VehicleUI))]
    public class Vehicle : Unit.Unit
    {
        [Header("Vehicle Data")]
        public VehicleSystem.Vehicle vehicleData;

        [Header("Runtime Settings")]
        public GameObject targetObject;
        public ParticleSystem smokeEffect;

        [Header("Muzzle Flash Settings")]
        public ParticleSystem[] muzzleFlashEffects;
        public Light[] muzzleFlashLights;

        [Header("Energy Warning Settings")]
        [Range(0f, 1f)]
        public float lowEnergyThreshold = 0.3f;

        [Networked] public NetworkId TargetNetworkId { get; set; }

        [Networked] public Vector3 TargetMovePosition { get; set; }
        [Networked] public bool IsMovingToPosition { get; set; }

        [Networked] protected int currentAmmunition_bullet { get; set; }
        [Networked] protected int currentAmmunition_rocket { get; set; }

        protected float bulletCooldownTimer = 0f;
        protected float rocketCooldownTimer = 0f;
        protected int reloadCounter_bullet = 0;
        protected int reloadCounter_rocket = 0;

        protected Ammunition ammunition_bullet;
        protected VehicleAmmunition bulletAmmunitionSettings;
        protected Ammunition ammunition_rocket;
        protected VehicleAmmunition rocketAmmunitionSettings;

        private ChangeDetector _changes;

        protected float speed;
        protected float turnSpeed;
        protected float fuelLevel;
        protected float maxFuel;
        protected float fuelConsumptionRate;
        protected float health;
        protected float maxHealth;
        protected float stoppingDistance;

        protected bool isMoving = false;
        protected GameObject nearestEnergyTower;
        protected bool isRefueling = false;
        protected EnergyTower targetTowerScript;
        protected VehicleUI vehicleUI;

        protected AudioClip engineSound;
        protected AudioClip bulletFireSound;
        protected AudioClip rocketFireSound;
        protected AudioSource soundEffectSource;
        public AudioSource engineSource;

        public Animator animator;

        protected virtual void Start()
        {
            vehicleUI = GetComponent<VehicleUI>();
            soundEffectSource = GetComponent<AudioSource>();

            engineSource.clip = vehicleData.engineSound;
            engineSource.Play();
        }

        public override void Spawned()
        {
            base.Spawned();
            _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);

            Setup();
            UpdateTargetObject();
            UpdateVehicleStatusIcon();
        }

        public override void Render()
        {
            foreach (var change in _changes.DetectChanges(this))
            {
                if (change == nameof(TargetNetworkId))
                {
                    UpdateTargetObject();
                }
            }

            if (bulletCooldownTimer > 0) bulletCooldownTimer -= Time.deltaTime;
            if (rocketCooldownTimer > 0) rocketCooldownTimer -= Time.deltaTime;
        }

        public void SetMovePosition(Vector3 position)
        {
            if (fuelLevel <= 0) return;

            if (Object.HasStateAuthority)
            {
                // Düşman hedefini iptal et
                TargetNetworkId = default;
                targetObject = null;

                // Pozisyon hedefini ayarla
                IsMovingToPosition = true;
                TargetMovePosition = position;
                isMoving = true;
            }
            else if (Object.HasInputAuthority)
            {
                RPC_SetMovePosition(position);
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_SetMovePosition(Vector3 position)
        {
            TargetNetworkId = default;
            targetObject = null;
            IsMovingToPosition = true;
            TargetMovePosition = position;
            isMoving = true;
        }

        public virtual void SetTargetEnemy(GameObject enemy)
        {
            if (fuelLevel <= 0) return;

            var enemyNetObj = enemy.GetComponent<NetworkObject>();
            if (enemyNetObj == null) return;

            if (Object.HasStateAuthority)
            {
                IsMovingToPosition = false;
                TargetNetworkId = enemyNetObj.Id;
                UpdateTargetObject();
            }
            else if (Object.HasInputAuthority)
            {
                RPC_SetTarget(enemyNetObj.Id);
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_SetTarget(NetworkId enemyId)
        {
            IsMovingToPosition = false;
            TargetNetworkId = enemyId;
            UpdateTargetObject();
        }

        private void UpdateTargetObject()
        {
            if (TargetNetworkId.IsValid)
            {
                var netObj = Runner.FindObject(TargetNetworkId);
                if (netObj != null)
                {
                    targetObject = netObj.gameObject;
                    isMoving = true;
                }
            }
            else
            {
                targetObject = null;
                isMoving = false;
            }
        }

        private void Setup()
        {
            speed = vehicleData.speed;
            turnSpeed = vehicleData.turnSpeed;
            stoppingDistance = vehicleData.stoppingDistance;
            maxHealth = vehicleData.maxHealth;
            health = maxHealth;
            maxFuel = vehicleData.fuelCapacity;
            fuelLevel = maxFuel;
            fuelConsumptionRate = vehicleData.fuelConsumptionRate;

            bulletFireSound = vehicleData.ammunitionSettings?.Where(ammo => ammo.ammunitionType == AmmunitionType.Bullet).FirstOrDefault()?.sound;
            rocketFireSound = vehicleData.ammunitionSettings?.Where(ammo => ammo.ammunitionType == AmmunitionType.Rocket).FirstOrDefault()?.sound;

            if (vehicleData.ammunitionSettings != null)
            {
                foreach (var ammoSetting in vehicleData.ammunitionSettings)
                {
                    if (ammoSetting.isEnabled)
                    {
                        switch (ammoSetting.ammunitionType)
                        {
                            case AmmunitionType.Bullet:
                                bulletAmmunitionSettings = ammoSetting;
                                ammunition_bullet = ammoSetting.ammunition;
                                currentAmmunition_bullet = ammoSetting.maxAmmunition;
                                break;
                            case AmmunitionType.Rocket:
                                rocketAmmunitionSettings = ammoSetting;
                                ammunition_rocket = ammoSetting.ammunition;
                                currentAmmunition_rocket = ammoSetting.maxAmmunition;
                                break;
                        }
                    }
                }
            }
        }

        public (string vehicleName,
                float currentHealth, float maxHealth,
                float currentFuel, float maxFuel,
                int bulletCurrent, int bulletMax, int bulletReloadCount,
                int rocketCurrent, int rocketMax, int rocketReloadCount
                ) GetVehicleStatus()
        {
            return (
                vehicleData.vehicleName, // vehicle name

                health, maxHealth, // health current and max

                fuelLevel, maxFuel, // fuel current and max

                currentAmmunition_bullet, // bullet current and max
                bulletAmmunitionSettings != null ? bulletAmmunitionSettings.maxAmmunition : 0,
                reloadCounter_bullet,

                currentAmmunition_rocket, // rocket current and max
                rocketAmmunitionSettings != null ? rocketAmmunitionSettings.maxAmmunition : 0,
                reloadCounter_rocket
                );
        }

        public override void FixedUpdateNetwork()
        {
            if (bulletCooldownTimer > 0) bulletCooldownTimer -= Runner.DeltaTime;
            if (rocketCooldownTimer > 0) rocketCooldownTimer -= Runner.DeltaTime;

            UpdateVehicleStatusIcon();
        }

        protected virtual void Update()
        {
            if (bulletCooldownTimer > 0) bulletCooldownTimer -= Time.deltaTime;
            if (rocketCooldownTimer > 0) rocketCooldownTimer -= Time.deltaTime;

            UpdateVehicleStatusIcon();
        }

        protected virtual void ConsumeFuel()
        {
            if (isMoving && fuelLevel > 0)
            {
                fuelLevel -= fuelConsumptionRate * Time.deltaTime;
                fuelLevel = Mathf.Max(0, fuelLevel);
            }
        }

        protected virtual void UpdateVehicleStatusIcon()
        {
            if (vehicleUI == null) return;

            if (isRefueling)
            {
                vehicleUI.SetVehicleStatusIconToRefueling();
            }
            else if (fuelLevel <= maxFuel * lowEnergyThreshold)
            {
                vehicleUI.SetVehicleStatusIconToWarning();
            }
            else
            {
                vehicleUI.ClearVehicleStatusIcon();
            }
        }

        protected virtual void FindNearestEnergyTower()
        {
            GameObject[] towers = GameObject.FindGameObjectsWithTag("Build");

            if (towers.Length == 0)
            {
                nearestEnergyTower = null;
                return;
            }

            var bestTower = Enumerable.FirstOrDefault(
                Enumerable.OrderBy(
                    Enumerable.Where(
                        Enumerable.Select(towers, t => t.GetComponent<EnergyTower>()),
                        script => script != null && script.IsAvailable()
                    ),
                    script => Vector3.Distance(transform.position, script.transform.position)
                )
            );

            if (bestTower != null)
            {
                if (bestTower.NewVehicleConnected(this))
                {
                    nearestEnergyTower = bestTower.gameObject;
                    targetTowerScript = bestTower;
                }
                else
                {
                    nearestEnergyTower = null;
                    targetTowerScript = null;
                }
            }
        }

        protected void DisconnectFromTower()
        {
            if (targetTowerScript != null)
            {
                targetTowerScript.VehicleDisconnected(this);
                targetTowerScript = null;
                nearestEnergyTower = null;
            }
        }

        protected virtual void UpdateSmokeEffect()
        {
            if (smokeEffect == null) return;

            if (isMoving)
            {
                smokeEffect.Play();
            }
            else if (!isMoving)
            {
                smokeEffect.Stop();
            }
        }

        public virtual void TryFireBullets()
        {
            if (!Object.HasStateAuthority) return;
            if (bulletAmmunitionSettings == null || !bulletAmmunitionSettings.isEnabled) return;

            if (bulletCooldownTimer <= 0)
            {
                if (currentAmmunition_bullet > 0)
                {
                    FireShot();
                    RPC_PlayMuzzleEffects();

                    bulletCooldownTimer = bulletAmmunitionSettings.fireRate;
                }
                else
                {
                    bulletCooldownTimer = bulletAmmunitionSettings.reloadTime;
                    ReloadAmmunition();
                }
            }
        }

        public virtual void TryFireRockets()
        {
            if (!Object.HasStateAuthority) return;
            if (rocketAmmunitionSettings == null || !rocketAmmunitionSettings.isEnabled) return;

            if (rocketCooldownTimer <= 0)
            {
                if (currentAmmunition_rocket > 0)
                {
                    LaunchRocket();
                    RPC_PlayMuzzleEffects();
                    rocketCooldownTimer = rocketAmmunitionSettings.fireRate;
                }
                else
                {
                    rocketCooldownTimer = rocketAmmunitionSettings.reloadTime;
                    ReloadRocketAmmunition();
                }
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        protected void RPC_PlayMuzzleEffects()
        {
            foreach (var effect in muzzleFlashEffects) if (effect != null) effect.Play();
            foreach (var light in muzzleFlashLights) if (light != null) light.enabled = true;
            Invoke(nameof(DisableMuzzleFlashLights), 0.1f);
        }

        private void DisableMuzzleFlashLights()
        {
            foreach (var light in muzzleFlashLights)
            {
                if (light != null)
                {
                    light.enabled = false;
                }
            }
        }

        protected virtual void FireShot()
        {
            soundEffectSource.PlayOneShot(bulletFireSound);
            animator.SetTrigger("isBulletTypeAttacking");
        }

        protected virtual void LaunchRocket()
        {
            soundEffectSource.PlayOneShot(rocketFireSound);
            animator.SetTrigger("isRocketTypeAttacking");
        }

        protected virtual void ReloadAmmunition()
        {
            reloadCounter_bullet++;
            currentAmmunition_bullet = bulletAmmunitionSettings.maxAmmunition;
        }

        protected virtual void ReloadRocketAmmunition()
        {
            if (rocketAmmunitionSettings != null)
            {
                reloadCounter_rocket++;
                currentAmmunition_rocket = rocketAmmunitionSettings.maxAmmunition;
            }
        }

        public override void TakeDamage(float damage)
        {
            if (!Object.HasStateAuthority) return;

            health -= damage;
            health = Mathf.Max(0, health);

            if (health <= 0)
            {
                GameStateManager.Instance.ReportUnitDestroyed(this);
                if (vehicleData.explosionEffect != null)
                    CommanderData.LocalCommander.RPC_SpawnExplosionEffect(transform.position);

                Runner.Despawn(Object);
            }
        }
    }
}
