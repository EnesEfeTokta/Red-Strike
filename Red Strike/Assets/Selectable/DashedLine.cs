/*
using UnityEngine;

public class DashedLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Material dashedLineMaterial;

    private Tank tank;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        tank = GetComponent<Tank>();

        if (dashedLineMaterial != null)
        {
            lineRenderer.material = dashedLineMaterial;
        }
        else
        {
            Debug.LogWarning("DashedLine: dashedLineMaterial atanmamış!", this);

            lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply")); // Geçici fallback
        }

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.alignment = LineAlignment.View;
    }

    private void Update()
    {
        UpdateTargetLine();
    }

    private void UpdateTargetLine()
    {
        if (tank != null && tank.CurrentFreeLookTarget != null)
        {
            Vector3 startPos = transform.position + Vector3.up * 0.1f;
            Vector3 targetPos = tank.CurrentFreeLookTarget.position + Vector3.up * 0.1f;

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, targetPos);

            float distance = Vector3.Distance(startPos, targetPos);
            lineRenderer.material.mainTextureScale = new Vector2(distance * 2f, 1f);
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }
}
*/
