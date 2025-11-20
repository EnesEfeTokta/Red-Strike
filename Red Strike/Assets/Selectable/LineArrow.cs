/*
using UnityEngine;
using System.Collections.Generic;

public class LineArrow : MonoBehaviour
{
    public GameObject arrowPrefab; 
    public float arrowSpacing = 2f;
    public float arrowSpeed = 5f;

    private List<GameObject> arrowPool = new List<GameObject>();
    private List<ArrowMover> activeArrows = new List<ArrowMover>();
    private Tank tank;

    private List<Vector3> currentPathPoints = new List<Vector3>();
    private int currentArrowCount = 0;

    private void Awake()
    {
        tank = GetComponent<Tank>();
        if (arrowPrefab == null)
        {
            Debug.LogError("LineArrow: arrowPrefab atanmamış!", this);
        }
    }

    private void Update()
    {
        if (tank == null || !(tank.IsSingleSelection || tank.IsDoubleSelection) || tank.PermanentFreeLookTargets.Count == 0)
        {
            if (activeArrows.Count > 0)
            {
                ClearArrows();
            }
            return;
        }

        // Koşullar sağlandıysa, okları güncelle ve hareket ettir
        bool pathChanged = UpdatePathPoints();
        UpdateArrows(pathChanged);
        MoveArrows(Time.deltaTime);
    }

    /// <summary>
    /// Hedeflere göre yol noktalarını günceller ve yolun değişip değişmediğini döndürür.
    /// </summary>
    /// <returns>Yol noktaları değiştiyse true, aksi takdirde false.</returns>
    private bool UpdatePathPoints()
    {
        List<Vector3> newPathPoints = new List<Vector3>();
        newPathPoints.Add(transform.position + Vector3.up * 0.1f); // Tankın pozisyonu

        // Kalıcı hedefleri yola ekle
        foreach (var targetTransform in tank.PermanentFreeLookTargets)
        {
            if (targetTransform != null)
            {
                newPathPoints.Add(targetTransform.position + Vector3.up * 0.1f);
            }
        }

        // Yeni yol ile eski yol aynı mı kontrol et
        if (newPathPoints.Count == currentPathPoints.Count)
        {
            bool same = true;
            for (int i = 0; i < newPathPoints.Count; i++)
            {
                // Küçük bir tolerans ile karşılaştır (pozisyonlar hafifçe değişebilir)
                if (Vector3.SqrMagnitude(newPathPoints[i] - currentPathPoints[i]) > 0.01f)
                {
                    same = false;
                    break;
                }
            }
            if (same) return false; // Yol değişmedi
        }

        // Yol değişti, güncelle
        currentPathPoints = newPathPoints;
        return true;
    }


    /// <summary>
    /// Gerekli ok sayısını hesaplar ve aktif okları buna göre ayarlar.
    /// Sadece yol değiştiyse veya ok sayısı değiştiyse yeniden oluşturma yapar.
    /// </summary>
    /// <param name="forceUpdate">Yol değişmese bile okları yeniden oluşturmaya zorlar mı?</param>
    private void UpdateArrows(bool pathChanged)
    {
        if (currentPathPoints.Count < 2)
        {
            if (activeArrows.Count > 0) ClearArrows();
            return;
        }

        // Toplam yol uzunluğunu hesapla
        float totalPathLength = 0f;
        for (int i = 0; i < currentPathPoints.Count - 1; i++)
        {
            totalPathLength += Vector3.Distance(currentPathPoints[i], currentPathPoints[i + 1]);
        }

        int requiredArrowCount = Mathf.FloorToInt(totalPathLength / arrowSpacing);

        if (requiredArrowCount == currentArrowCount && !pathChanged)
        {
            return;
        }

        // Ok sayısı veya yol değişti, okları yeniden ayarla
        ClearArrows(); // Önce mevcutları temizle
        currentArrowCount = requiredArrowCount; // Yeni sayıyı kaydet

        if (arrowPrefab == null) return; // Prefab yoksa devam etme

        for (int i = 0; i < currentArrowCount; i++)
        {
            float offset = i * arrowSpacing;
            GameObject arrow = GetArrowFromPool();
            arrow.SetActive(true);

            ArrowMover mover = new ArrowMover(arrow.transform, currentPathPoints, arrowSpeed, offset);
            activeArrows.Add(mover);

            mover.UpdatePosition(0);
        }
    }

    /// <summary>
    /// Aktif okları hareket ettirir.
    /// </summary>
    private void MoveArrows(float deltaTime)
    {
        foreach (var mover in activeArrows)
        {
            mover.Move(deltaTime);
        }
    }

    /// <summary>
    /// Tüm aktif okları havuza geri gönderir ve listeyi temizler.
    /// </summary>
    private void ClearArrows()
    {
        foreach (var mover in activeArrows)
        {
            if (mover.Target != null)
            {
                mover.Target.gameObject.SetActive(false);
            }
        }
        activeArrows.Clear();
        currentArrowCount = 0;
    }

    /// <summary>
    /// Ok havuzundan kullanılmayan bir ok alır veya yenisini oluşturur.
    /// </summary>
    private GameObject GetArrowFromPool()
    {
        // Havuzda deaktif ok ara
        foreach (var arrow in arrowPool)
        {
            if (arrow != null && !arrow.activeInHierarchy)
            {
                return arrow;
            }
        }

        // Havuzda yoksa veya hepsi aktifse yenisini oluştur
        GameObject newArrow = Instantiate(arrowPrefab, transform); // Parent'ı bu obje yapalım ki sahnede kaybolmasınlar
        newArrow.SetActive(false); // Başlangıçta deaktif
        arrowPool.Add(newArrow); // Havuza ekle
        return newArrow;
    }

    // ArrowMover sınıfı (İç içe sınıf olarak kalabilir)
    private class ArrowMover
    {
        public Transform Target;
        private readonly List<Vector3> path;
        private readonly float speed;
        private readonly float totalLength;
        private float currentDistance;

        private readonly List<float> segmentLengths;
        private readonly List<float> cumulativeLengths;

        public ArrowMover(Transform target, List<Vector3> path, float speed, float initialOffset)
        {
            Target = target;
            this.path = path;
            this.speed = speed;
            this.currentDistance = initialOffset;

            segmentLengths = new List<float>();
            cumulativeLengths = new List<float>();
            totalLength = 0f;
            cumulativeLengths.Add(0f);

            for (int i = 0; i < path.Count - 1; i++)
            {
                float segmentLength = Vector3.Distance(path[i], path[i + 1]);
                segmentLengths.Add(segmentLength);
                totalLength += segmentLength;
                cumulativeLengths.Add(totalLength);
            }
        }

        /// <summary>
        /// Oku zaman adımı kadar hareket ettirir ve pozisyonunu günceller.
        /// </summary>
        public void Move(float deltaTime)
        {
            if (totalLength <= 0) return;

            currentDistance += speed * deltaTime;

            if (currentDistance > totalLength)
            {
                currentDistance -= totalLength;
            }

            UpdatePosition(currentDistance);
        }

        /// <summary>
        /// Verilen mesafeye göre okun pozisyonunu ve rotasyonunu ayarlar.
        /// </summary>
        /// <param name="distanceAlongPath">Yol üzerindeki hedef mesafe.</param>
        public void UpdatePosition(float distanceAlongPath)
        {
            if (path.Count < 2 || Target == null) return;

            for (int i = 0; i < segmentLengths.Count; i++)
            {
                if (distanceAlongPath <= cumulativeLengths[i + 1])
                {
                    float distanceIntoSegment = distanceAlongPath - cumulativeLengths[i];

                    float segLength = segmentLengths[i];
                    if (segLength <= 0.001f)
                    {
                        Target.position = path[i + 1];
                        if (i + 2 < path.Count)
                            Target.rotation = Quaternion.LookRotation(path[i + 2] - path[i + 1]);
                        else
                            Target.rotation = Quaternion.LookRotation(path[i + 1] - path[i]);
                        return;
                    }

                    float t = distanceIntoSegment / segLength;

                    Vector3 pos = Vector3.Lerp(path[i], path[i + 1], t);
                    Target.position = pos;

                    Vector3 direction = (path[i + 1] - path[i]).normalized;
                    if (direction != Vector3.zero)
                        Target.rotation = Quaternion.LookRotation(direction);

                    return;
                }

                Target.position = path[path.Count - 1];
                if (segmentLengths.Count > 0)
                    Target.rotation = Quaternion.LookRotation(path[path.Count - 1] - path[path.Count - 2]);

            }
        }
    }
}
*/
