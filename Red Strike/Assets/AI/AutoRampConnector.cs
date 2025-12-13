using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections.Generic;

public class AutoRampConnector : EditorWindow
{
    [Header("Zorunlu Alan")]
    GameObject linkPrefab;

    [Header("Tarama Ayarları")]
    float minHeight = 0.5f;
    float maxHeight = 8.0f;
    float density = 2.0f;
    LayerMask groundLayer = ~0;

    [Header("Eğim Ayarı (Rampa Uzunluğu)")]
    float slopeFactor = 1.5f;

    [MenuItem("Tools/Auto Ramp Connector (High-to-Low)")]
    public static void ShowWindow()
    {
        GetWindow(typeof(AutoRampConnector));
    }

    void OnGUI()
    {
        GUILayout.Label("Otomatik Rampa Bağlayıcı", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Bu araç, yüksek zeminlerden alçak zeminlere otomatik olarak 'Döndürülmüş' (Rotated) prefablar yerleştirir.", MessageType.Info);

        EditorGUILayout.Space();
        
        linkPrefab = (GameObject)EditorGUILayout.ObjectField("Link Prefab", linkPrefab, typeof(GameObject), false);
        
        if (linkPrefab == null)
        {
            EditorGUILayout.HelpBox("Lütfen Rampa/Link Prefab'ını buraya sürükleyin!", MessageType.Error);
        }

        EditorGUILayout.Space();
        GUILayout.Label("Ayarlar", EditorStyles.label);
        minHeight = EditorGUILayout.FloatField("Min Yükseklik", minHeight);
        maxHeight = EditorGUILayout.FloatField("Max Yükseklik", maxHeight);
        slopeFactor = EditorGUILayout.FloatField("Eğim Çarpanı (Slope)", slopeFactor);
        density = EditorGUILayout.FloatField("Sıklık (Density)", density);

        EditorGUILayout.Space();

        GUI.enabled = linkPrefab != null;
        if (GUILayout.Button("Rampaları Otomatik Yerleştir", GUILayout.Height(40)))
        {
            GenerateConnections();
        }
        GUI.enabled = true;

        if (GUILayout.Button("Sahnedeki Rampaları Temizle"))
        {
            ClearConnections();
        }
    }

    void GenerateConnections()
    {
        ClearConnections();

        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
        
        GameObject container = new GameObject("Auto_Connected_Ramps");
        Undo.RegisterCreatedObjectUndo(container, "Auto Ramp Gen");

        int count = 0;

        Dictionary<string, int> edgeCounts = new Dictionary<string, int>();
        List<Vector3[]> edges = new List<Vector3[]>();

        for (int i = 0; i < triangulation.indices.Length; i += 3)
        {
            int i1 = triangulation.indices[i];
            int i2 = triangulation.indices[i + 1];
            int i3 = triangulation.indices[i + 2];
            AddEdge(triangulation.vertices[i1], triangulation.vertices[i2], edgeCounts, edges);
            AddEdge(triangulation.vertices[i2], triangulation.vertices[i3], edgeCounts, edges);
            AddEdge(triangulation.vertices[i3], triangulation.vertices[i1], edgeCounts, edges);
        }

        foreach (var edge in edges)
        {
            if (edgeCounts[GetEdgeKey(edge[0], edge[1])] == 1)
            {
                ProcessHighGroundEdge(edge[0], edge[1], container.transform, ref count);
            }
        }

        Debug.Log($"İşlem Tamamlandı! {count} adet rampa bağlantısı kuruldu.");
    }

    void ProcessHighGroundEdge(Vector3 v1, Vector3 v2, Transform parent, ref int count)
    {
        float edgeLen = Vector3.Distance(v1, v2);
        int segments = Mathf.Max(1, Mathf.FloorToInt(edgeLen / density));
        Vector3 edgeDir = (v2 - v1).normalized;

        Vector3 sideA = Vector3.Cross(edgeDir, Vector3.up);
        Vector3 sideB = -sideA;

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            Vector3 startPos = Vector3.Lerp(v1, v2, t);

            Vector3 cliffDir = Vector3.zero;
            if (IsCliff(startPos, sideA)) cliffDir = sideA;
            else if (IsCliff(startPos, sideB)) cliffDir = sideB;
            else continue;

            RaycastHit hit;
            Vector3 rayStart = startPos + (cliffDir * 0.2f) + Vector3.up * 0.1f;

            if (Physics.Raycast(rayStart, Vector3.down, out hit, maxHeight, groundLayer))
            {
                float dropHeight = hit.distance;
                if (dropHeight < minHeight) continue;

                float horizontalDistance = dropHeight * slopeFactor;

                Vector3 targetPos = startPos + (cliffDir * horizontalDistance);
                targetPos.y = hit.point.y;

                NavMeshHit navHit;
                Vector3 finalEndPos;

                if (NavMesh.SamplePosition(targetPos, out navHit, 2.0f, NavMesh.AllAreas))
                {
                    finalEndPos = navHit.position;
                }
                else
                {
                    if (NavMesh.SamplePosition(hit.point, out navHit, horizontalDistance, NavMesh.AllAreas))
                    {
                         if(Vector3.Distance(startPos, navHit.position) > minHeight)
                             finalEndPos = navHit.position;
                         else continue;
                    }
                    else continue;
                }

                CreateRotatedRamp(startPos, finalEndPos, parent);
                count++;
            }
        }
    }

    void CreateRotatedRamp(Vector3 start, Vector3 end, Transform parent)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        GameObject rampObj = (GameObject)PrefabUtility.InstantiatePrefab(linkPrefab);
        rampObj.transform.position = start;
        rampObj.transform.SetParent(parent);

        if (direction != Vector3.zero)
        {
            rampObj.transform.rotation = Quaternion.LookRotation(direction);
        }

        NavMeshLink link = rampObj.GetComponent<NavMeshLink>();
        if (link == null) link = rampObj.AddComponent<NavMeshLink>();

        link.startPoint = Vector3.zero;
        link.endPoint = new Vector3(0, 0, distance);
        
        link.width = 1.0f; 
        link.bidirectional = true;
        link.autoUpdate = true;
    }

    bool IsCliff(Vector3 origin, Vector3 dir)
    {
        Vector3 checkPos = origin + (dir * 0.5f) + Vector3.up * 0.5f;
        return !Physics.Raycast(checkPos, Vector3.down, 2.0f, groundLayer);
    }

    void AddEdge(Vector3 v1, Vector3 v2, Dictionary<string, int> counts, List<Vector3[]> edges)
    {
        string key = GetEdgeKey(v1, v2);
        if (counts.ContainsKey(key)) counts[key]++;
        else { counts[key] = 1; edges.Add(new Vector3[] { v1, v2 }); }
    }

    string GetEdgeKey(Vector3 v1, Vector3 v2)
    {
        if (v1.x < v2.x || (v1.x == v2.x && v1.z < v2.z)) return $"{v1}_{v2}";
        else return $"{v2}_{v1}";
    }

    void ClearConnections()
    {
        GameObject existing = GameObject.Find("Auto_Connected_Ramps");
        if (existing != null) Undo.DestroyObjectImmediate(existing);
    }
}