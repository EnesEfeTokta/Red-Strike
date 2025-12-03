using UnityEngine;
using AmmunitionSystem;
using System.Linq;
using BuildingPlacement.Buildings;

namespace VehicleSystem.Vehicles
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(VehicleUI))]
    public class Vehicle : MonoBehaviour
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
        public float lowEnergyThreshold = 0.3f; // %30'un altında uyarı

        protected float speed;
        protected float turnSpeed;
        protected float fuelLevel;
        protected float maxFuel;
        protected float fuelConsumptionRate;
        protected float health;
        protected float maxHealth;
        protected float stoppingDistance;

        // Bullet ammunition
        protected int currentAmmunition_bullet = 30;
        protected int reloadCounter_bullet = 0;
        protected Ammunition ammunition_bullet;
        protected VehicleAmmunition bulletAmmunitionSettings;
        protected float bulletCooldownTimer = 0f;

        // Rocket ammunition
        protected int currentAmmunition_rocket = 10;
        protected int reloadCounter_rocket = 0;
        protected Ammunition ammunition_rocket;
        protected VehicleAmmunition rocketAmmunitionSettings;
        protected float rocketCooldownTimer = 0f;

        protected bool isMoving = false;

        protected GameObject nearestEnergyTower;
        protected bool isRefueling = false;

        protected EnergyTower targetTowerScript;

        protected VehicleUI vehicleUI;

        protected virtual void Start()
        {
            vehicleUI = GetComponent<VehicleUI>();
            Setup();
            UpdateVehicleStatusIcon();
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

        public (string, float, int, int, float) GetVehicleStatus()
        {
            return (vehicleData.vehicleName, fuelLevel, currentAmmunition_bullet, bulletAmmunitionSettings.maxAmmunition, health);
        }

        public (int currentRockets, int maxRockets) GetRocketStatus()
        {
            if (rocketAmmunitionSettings != null)
                return (currentAmmunition_rocket, rocketAmmunitionSettings.maxAmmunition);
            return (0, 0);
        }

        public virtual void SetTargetEnemy(GameObject enemy)
        {
            if (fuelLevel <= 0) return;

            targetObject = enemy;
            isMoving = true;
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

            if (isMoving && !smokeEffect.isPlaying)
            {
                smokeEffect.Play();
            }
            else if (!isMoving && smokeEffect.isPlaying)
            {
                smokeEffect.Stop();
            }
        }

        public virtual void TryFireBullets()
        {
            if (bulletAmmunitionSettings == null || !bulletAmmunitionSettings.isEnabled) return;

            if (bulletCooldownTimer <= 0)
            {
                if (currentAmmunition_bullet > 0)
                {
                    FireShot();

                    EnableMuzzleFlashEffects();
                    Invoke("DisableMuzzleFlashLights", 0.1f);

                    bulletCooldownTimer = bulletAmmunitionSettings.reloadTime;
                }
                else
                {
                    ReloadAmmunition();
                    bulletCooldownTimer = bulletAmmunitionSettings.reloadTime * 2;
                }
            }
        }

        public virtual void TryFireRockets()
        {
            if (rocketAmmunitionSettings == null || !rocketAmmunitionSettings.isEnabled) return;

            if (rocketCooldownTimer <= 0)
            {
                if (currentAmmunition_rocket > 0)
                {
                    LaunchRocket();

                    EnableMuzzleFlashEffects();
                    Invoke("DisableMuzzleFlashLights", 0.1f);

                    rocketCooldownTimer = rocketAmmunitionSettings.reloadTime;
                }
                else
                {
                    ReloadRocketAmmunition();
                    rocketCooldownTimer = rocketAmmunitionSettings.reloadTime * 2;
                }
            }
        }

        private void EnableMuzzleFlashEffects()
        {
            foreach (var effect in muzzleFlashEffects)
            {
                if (effect != null)
                {
                    effect.Play();
                }
            }

            foreach (var light in muzzleFlashLights)
            {
                if (light != null)
                {
                    light.enabled = true;
                }
            }
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
            // Implement firing logic in derived classes
        }

        protected virtual void LaunchRocket()
        {
            // Implement missile launching logic in derived classes
        }

        protected virtual void ReloadAmmunition()
        {
            currentAmmunition_bullet = bulletAmmunitionSettings.maxAmmunition;
        }

        protected virtual void ReloadRocketAmmunition()
        {
            if (rocketAmmunitionSettings != null)
            {
                currentAmmunition_rocket = rocketAmmunitionSettings.maxAmmunition;
            }
        }
    }
}
