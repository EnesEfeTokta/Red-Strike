using UnityEngine;
using DG.Tweening; // DOTween kütüphanesini kullanmak için gerekli

// Bu script'in çalıştığı GameObject'e bir AudioSource component'i eklemeyi unutma.
// IAttackStyle arayüzünün projenizde tanımlı olduğunu varsayıyoruz.
public class TankAttack : MonoBehaviour, IAttackStyle
{
    [Header("References")]
    public GameObject BulletPrefab;
    public Transform FirePoint;
    public ParticleSystem BarrelParticle;
    public Transform BarrelTransform; // Namlu geri tepme animasyonu için
    public AudioClip FireSound;

    [Header("Attack Stats")]
    public float FireRate = 1f;
    public float BulletSpeed = 20f;
    public float BulletDamage = 10f;
    public float BulletLifetime = 5f;

    private float nextFireTime = 0f;
    private Tank tank;
    private AudioSource audioSource;

    private void Awake()
    {
        tank = GetComponent<Tank>();
        audioSource = GetComponent<AudioSource>(); // AudioSource component'ini al

        if (tank != null)
        {
            // Referanslar atanmadıysa Tank'tan almayı dene
            if (FirePoint == null) FirePoint = tank.CannonTransform;
            if (BarrelParticle == null) BarrelParticle = tank.BarrelParticle;
            if (BarrelTransform == null) BarrelTransform = tank.BarrelTransform;
        }
        else
        {
            Debug.LogError("TankAttack script'i bir Tank component'i olan GameObject üzerinde olmalı!", this);
        }

        // Null check'ler
        if (audioSource == null)
        {
             Debug.LogWarning("TankAttack için AudioSource component'i bulunamadı. Ses çalınmayacak.", this);
        }
        if (BulletPrefab == null) Debug.LogError("BulletPrefab atanmamış!", this);
        if (FirePoint == null) Debug.LogError("FirePoint atanmamış veya Tank üzerinde CannonTransform bulunamadı!", this);
        // BarrelTransform null olabilir, animasyon istenmiyorsa sorun değil.
    }

    public void HandleSingleTargetInput(Transform target)
    {
        // Hedef geçerliyse ve ateş etme zamanı geldiyse ateş et
        if (target != null && Time.time >= nextFireTime)
        {
            // Tank.cs zaten tareti/namluyu hedefe çeviriyor olmalı
            Fire();
        }
    }

    public void HandleMultiTargetInput(Transform[] targets)
    {
        // Tank.cs'in hedef önceliğini belirlediğini varsayıyoruz.
        // Hedef listesi boş değilse ve ateş etme zamanı geldiyse ateş et.
        if (targets != null && targets.Length > 0 && Time.time >= nextFireTime)
        {
            // Not: Bu mantık her zaman listedeki ilk hedefe (veya Tank.cs'in yöneldiği hedefe)
            // ateş edecektir. İleride farklı hedefleme stratejileri (örn: en yakına) eklenebilir.
            Fire();
        }
    }

    public void HandleTPSAimingInput()
    {
        // FireRate'e göre GetMouseButton veya GetMouseButtonDown kullanılabilir
        // Sürekli ateş için GetMouseButton daha uygun.
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Fire();
        }
    }

    private void Fire()
    {
        // Gerekli bileşenler veya tank durumu uygun değilse ateş etme
        if (BulletPrefab == null || FirePoint == null || tank == null || tank.tankState == Tank.TankState.Destroyed)
            return;

        // Bir sonraki ateş zamanını hesapla
        nextFireTime = Time.time + 1f / FireRate;

        // Mermiyi oluştur ve fırlat
        GameObject projectile = Instantiate(BulletPrefab, FirePoint.position, FirePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = FirePoint.forward * BulletSpeed;
        }

        // Mermiye hasar bilgisi aktarma (Mermi script'i bunu almalı)
        // Örnek: projectile.GetComponent<Bullet>()?.Initialize(BulletDamage);

        // Mermiyi belirli bir süre sonra yok et
        Destroy(projectile, BulletLifetime);

        // Efektleri oynat
        if (BarrelParticle != null)
        {
            BarrelParticle.Play();
        }

        // Sesi çal
        if (FireSound != null && audioSource != null)
        {
             audioSource.PlayOneShot(FireSound);
        }

        // Namlu geri tepme animasyonu (DOTween kuruluysa)
        if (BarrelTransform != null)
        {
             // Önceki animasyonu durdur (varsa)
             BarrelTransform.DOKill();
             // Geri tepme animasyonu
             BarrelTransform.DOLocalMoveZ(-0.5f, 0.1f).SetRelative().SetEase(Ease.OutQuad)
                  .OnComplete(() => BarrelTransform.DOLocalMoveZ(0.5f, 0.2f).SetRelative().SetEase(Ease.OutBounce));
        }
    }
}