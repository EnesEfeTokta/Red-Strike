using UnityEngine;

public class DashedLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Material dashedLineMaterial;

    private SelectableVehicle selectableVehicle;

    public LineRenderer LineRenderer
    {
        get { return lineRenderer; }
        set { lineRenderer = value; }
    }

    private void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        selectableVehicle = GetComponent<SelectableVehicle>();

        lineRenderer.material = dashedLineMaterial;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;
        lineRenderer.textureMode = LineTextureMode.Tile; // Doku tekrar etsin
        lineRenderer.alignment = LineAlignment.View;     // Kamera ile hizalanma
    }

    private void Update()
    {
        UpdateTargetLine();
    }

    private void UpdateTargetLine()
    {
        if (selectableVehicle.Selected)
        {
            if (selectableVehicle.CurrentlySelectedTarget != null)
            {
                Vector3 startPos = transform.position + Vector3.up * 0.1f;
                Vector3 targetPos = selectableVehicle.CurrentlySelectedTarget.transform.position + Vector3.up * 0.1f;

                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, startPos);
                lineRenderer.SetPosition(1, targetPos);
            }
            else
            {
                Vector3 startPos = transform.position + Vector3.up * 0.1f;

                lineRenderer.positionCount = selectableVehicle.PermanentTargets.Count+1;
                lineRenderer.SetPosition(0, startPos);
                for (int i = 0; i < selectableVehicle.PermanentTargets.Count; i++)
                {
                    lineRenderer.SetPosition(i+1, selectableVehicle.PermanentTargets[i].transform.position + Vector3.up * 0.1f);
                }
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }
}
