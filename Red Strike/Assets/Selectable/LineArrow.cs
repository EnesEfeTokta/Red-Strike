using UnityEngine;
using System.Collections.Generic;

public class LineArrow : MonoBehaviour
{
    public GameObject arrowPrefab;
    public float arrowSpacing = 2f;
    public float arrowSpeed = 5f;

    private List<GameObject> arrowPool = new();
    private List<ArrowMover> activeArrows = new();
    private SelectableVehicle selectableVehicle;

    private void Start()
    {
        selectableVehicle = GetComponent<SelectableVehicle>();
    }

    private void Update()
    {
        if (!selectableVehicle.Selected)
        {
            ClearArrows();
            return;
        }

        UpdateArrows();
        MoveArrows(Time.deltaTime);
    }

    /// <summary>
    /// Okların konumunu güncelleyecek olan fonksiyon.
    /// Bu fonksiyon, hedeflerin konumunu alacak ve okları bu konumlara göre yerleştirecek.
    /// </summary>
    private void UpdateArrows()
    {
        List<Vector3> pathPoints = new();
        pathPoints.Add(transform.position + Vector3.up * 0.1f);

        foreach (var target in selectableVehicle.PermanentTargets)
        {
            pathPoints.Add(target.transform.position + Vector3.up * 0.1f);
        }

        float totalPathLength = 0f;
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            totalPathLength += Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
        }

        int arrowCount = Mathf.FloorToInt(totalPathLength / arrowSpacing);

        if (activeArrows.Count == arrowCount) return;

        ClearArrows();

        for (int i = 0; i < arrowCount; i++)
        {
            float offset = i * arrowSpacing;
            GameObject arrow = GetArrowFromPool();
            arrow.SetActive(true);

            ArrowMover mover = new ArrowMover(arrow.transform, pathPoints, arrowSpeed, offset);
            activeArrows.Add(mover);
        }
    }

    /// <summary>
    /// Ok hareketlerini yapacak olan fonksiyon.
    /// Bu fonksiyon her frame'de çağrılacak ve okların hareket etmesini sağlayacak.
    /// </summary>
    /// <param name="deltaTime">Geçen süre.</param>
    private void MoveArrows(float deltaTime)
    {
        foreach (var mover in activeArrows)
        {
            mover.Move(deltaTime);
        }
    }

    /// <summary>
    /// Okları temizleyecek olan fonksiyon.
    /// </summary>
    private void ClearArrows()
    {
        foreach (var mover in activeArrows)
        {
            mover.Target.gameObject.SetActive(false);
        }
        activeArrows.Clear();
    }

    /// <summary>
    /// Ok havuzundan kullanılmayan bir ok alacak olan fonksiyon.
    /// Eğer havuzda kullanılmayan ok yoksa yeni bir ok oluşturacak.
    /// </summary>
    /// <returns></returns>
    private GameObject GetArrowFromPool()
    {
        foreach (var arrow in arrowPool)
        {
            if (!arrow.activeInHierarchy)
                return arrow;
        }

        GameObject newArrow = Instantiate(arrowPrefab, transform);
        newArrow.SetActive(false);
        arrowPool.Add(newArrow);
        return newArrow;
    }

    /// <summary>
    /// Okların hareketini yönetecek olan sınıf.
    /// Bu sınıf, okların hareketini ve yönünü hesaplayacak.
    /// </summary>
    private class ArrowMover
    {
        public Transform Target;
        private readonly List<Vector3> path;
        private readonly float speed;
        private readonly float totalLength;
        private float offset;

        private readonly List<float> segmentLengths = new();

        public ArrowMover(Transform target, List<Vector3> path, float speed, float offset)
        {
            Target = target;
            this.path = new List<Vector3>(path);
            this.speed = speed;
            this.offset = offset;

            totalLength = 0f;
            for (int i = 0; i < path.Count - 1; i++)
            {
                float segmentLength = Vector3.Distance(path[i], path[i + 1]);
                segmentLengths.Add(segmentLength);
                totalLength += segmentLength;
            }
        }

        public void Move(float deltaTime)
        {
            offset += speed * deltaTime;
            if (offset > totalLength)
                offset -= totalLength;

            float distCovered = 0f;
            for (int i = 0; i < path.Count - 1; i++)
            {
                float segLength = segmentLengths[i];
                if (offset <= distCovered + segLength)
                {
                    float t = (offset - distCovered) / segLength;
                    Vector3 pos = Vector3.Lerp(path[i], path[i + 1], t);
                    Target.position = pos;
                    Target.rotation = Quaternion.LookRotation(path[i + 1] - path[i]);
                    return;
                }
                distCovered += segLength;
            }
        }
    }
}
