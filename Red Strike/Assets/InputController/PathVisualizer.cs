using UnityEngine;
using UnityEngine.AI;

namespace InputController
{
    [RequireComponent(typeof(LineRenderer))]
    public class PathVisualizer : MonoBehaviour
    {
        private LineRenderer lineRenderer;
        private NavMeshAgent agent;
        private Material lineMaterial;

        private int baseMapID;

        [Header("Görsel Ayarlar")]
        public float animationSpeed = 3.0f;
        public float lineWidth = 1.0f;
        public float textureTilingMultiplier = 0.5f;

        public enum PathColor { Red, Green, Yellow }

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            agent = GetComponent<NavMeshAgent>();

            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.enabled = false;

            lineRenderer.numCornerVertices = 4;
            lineRenderer.numCapVertices = 4;

            lineMaterial = lineRenderer.material;
            baseMapID = Shader.PropertyToID("_BaseMap");

        }

        private void Update()
        {
            if (lineRenderer.enabled && lineMaterial != null)
            {
                float offset = Time.time * -animationSpeed;
                Vector2 offsetVec = new Vector2(offset, 0);

                lineMaterial.SetTextureOffset(baseMapID, offsetVec);
            }
        }

        public void ShowNavMeshPath(PathColor color)
        {
            if (agent == null || !agent.hasPath)
            {
                HidePath();
                return;
            }

            Vector3[] corners = agent.path.corners;
            if (corners.Length < 2)
            {
                HidePath();
                return;
            }

            DrawLineInternal(corners, color);
        }

        public void ShowDirectLine(Vector3 start, Vector3 end, PathColor color)
        {
            Vector3[] points = new Vector3[] { start, end };
            DrawLineInternal(points, color);
        }

        private void DrawLineInternal(Vector3[] points, PathColor color)
        {
            lineRenderer.enabled = true;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;

            switch (color)
            {
                case PathColor.Red: lineMaterial.color = Color.red; break;
                case PathColor.Green: lineMaterial.color = Color.green; break;
                case PathColor.Yellow: lineMaterial.color = Color.yellow; break;
            }

            for (int i = 0; i < points.Length; i++)
            {
                points[i].y += 0.5f;
            }

            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);

            float totalDist = 0;
            for (int i = 0; i < points.Length - 1; i++)
            {
                totalDist += Vector3.Distance(points[i], points[i + 1]);
            }

            float finalTiling = totalDist * textureTilingMultiplier;
            if (finalTiling < 1.0f) finalTiling = 1.0f;

            Vector2 scaleVec = new Vector2(finalTiling, 1);

            lineMaterial.SetTextureScale(baseMapID, scaleVec);
        }

        public void HidePath()
        {
            if (lineRenderer != null) lineRenderer.enabled = false;
        }
    }
}