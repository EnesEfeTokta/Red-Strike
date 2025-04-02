using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class Outline : MonoBehaviour
{
    private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();

    public enum Mode
    {
        OutlineAll,
        OutlineVisible,
        OutlineHidden,
        OutlineAndSilhouette,
        SilhouetteOnly
    }

    public Mode OutlineMode
    {
        get { return outlineMode; }
        set
        {
            if (outlineMode == value) return;
            outlineMode = value;
            needsUpdate = true;
        }
    }

    public Color OutlineColor
    {
        get { return outlineColor; }
        set
        {
            if (outlineColor == value) return;
            outlineColor = value;
            needsUpdate = true;
        }
    }

    public float OutlineWidth
    {
        get { return outlineWidth; }
        set
        {
            if (outlineWidth == value) return;
            outlineWidth = value;
            needsUpdate = true;
        }
    }

    [Serializable]
    private class ListVector3
    {
        public List<Vector3> data;
    }

    [SerializeField]
    private Mode outlineMode;

    [SerializeField]
    private Color outlineColor = Color.white;

    [SerializeField, Range(0f, 10f)]
    private float outlineWidth = 2f;

    [Header("Optional")]

    [SerializeField]
    private bool precomputeOutline;

    [SerializeField, HideInInspector]
    private List<Mesh> bakeKeys = new List<Mesh>();

    [SerializeField, HideInInspector]
    private List<ListVector3> bakeValues = new List<ListVector3>();

    private Renderer[] renderers;
    private Material outlineMaskMaterial;
    private Material outlineFillMaterial;

    private bool needsUpdate;
    private bool isInitialized = false;

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        if (isInitialized) return;

        var allRenderers = GetComponentsInChildren<Renderer>();
        renderers = allRenderers.Where(r => !(r is ParticleSystemRenderer)).ToArray(); // ParticleSystemRenderer'ları filtrele

        if (outlineMaskMaterial == null)
        {
            outlineMaskMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineMask"));
            if (outlineMaskMaterial != null) outlineMaskMaterial.name = "OutlineMask (Instance)";
        }
        if (outlineFillMaterial == null)
        {
            outlineFillMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineFill"));
            if (outlineFillMaterial != null) outlineFillMaterial.name = "OutlineFill (Instance)";
        }

        if (outlineMaskMaterial == null || outlineFillMaterial == null) {
            Debug.LogError("Failed to load Outline materials. Make sure 'Resources/Materials/OutlineMask' and 'Resources/Materials/OutlineFill' exist.", this);
            isInitialized = false; // Başlatma başarısız oldu
            return;
        }

        LoadSmoothNormals();

        needsUpdate = true;
        isInitialized = true;
    }

    void OnEnable()
    {
        if (!isInitialized) Initialize(); // Ensure initialized
        if (!isInitialized) return; // Still not initialized? Bail.


        if (renderers == null || outlineMaskMaterial == null || outlineFillMaterial == null)
        {
             Debug.LogError("Outline cannot enable. Initialization failed or components missing.", this);
             return;
        }

        foreach (var renderer in renderers)
        {
            if (renderer == null) continue;

            var materials = renderer.sharedMaterials.ToList();
            bool maskAdded = materials.Contains(outlineMaskMaterial);
            bool fillAdded = materials.Contains(outlineFillMaterial);

            if (!maskAdded) materials.Add(outlineMaskMaterial);
            if (!fillAdded) materials.Add(outlineFillMaterial);

            if (!maskAdded || !fillAdded)
            {
                renderer.materials = materials.ToArray();
            }
        }
        needsUpdate = true;
    }

    void OnValidate()
    {
        needsUpdate = true;

        if (!precomputeOutline && bakeKeys.Count != 0 || bakeKeys.Count != bakeValues.Count)
        {
            bakeKeys.Clear();
            bakeValues.Clear();
        }

        if (precomputeOutline && bakeKeys.Count == 0 && Application.isEditor && !Application.isPlaying)
        {
        }
    }

    void Update()
    {
        if (this.enabled && isInitialized && needsUpdate)
        {
            UpdateMaterialProperties();
            needsUpdate = false;
        }
    }

    void OnDisable()
    {
        if (!isInitialized || renderers == null || outlineMaskMaterial == null || outlineFillMaterial == null)
        {
             return;
        }

        foreach (var renderer in renderers)
        {
             if (renderer == null) continue;

            try {
                var materials = renderer.sharedMaterials.ToList();
                bool changed = false;
                if (materials.Remove(outlineMaskMaterial)) changed = true;
                if (materials.Remove(outlineFillMaterial)) changed = true;

                if(changed)
                    renderer.materials = materials.ToArray();
            } catch (Exception ex) {
                Debug.LogWarning($"Could not modify materials on disable for {renderer.gameObject.name}: {ex.Message}", renderer.gameObject);
            }
        }
    }

    void OnDestroy()
    {
        if (outlineMaskMaterial != null)
        {
            Destroy(outlineMaskMaterial);
        }
        if (outlineFillMaterial != null)
        {
            Destroy(outlineFillMaterial);
        }
        isInitialized = false; // Reset initialization state on destroy
    }

    void Bake()
    {
        var bakedMeshes = new HashSet<Mesh>();
        bakeKeys.Clear();
        bakeValues.Clear();

        foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
        {
            if (meshFilter.sharedMesh == null) continue;
            if (!bakedMeshes.Add(meshFilter.sharedMesh)) continue;

            var smoothNormals = SmoothNormals(meshFilter.sharedMesh);
            bakeKeys.Add(meshFilter.sharedMesh);
            bakeValues.Add(new ListVector3() { data = smoothNormals });
        }
         Debug.Log($"Baked outline data for {bakedMeshes.Count} unique meshes.", this);
    }


    void LoadSmoothNormals()
    {
        var meshFilters = GetComponentsInChildren<MeshFilter>();
        foreach (var meshFilter in meshFilters)
        {
            if (meshFilter.sharedMesh == null) continue;
            if (registeredMeshes.Contains(meshFilter.sharedMesh)) continue;

            var index = bakeKeys.IndexOf(meshFilter.sharedMesh);
            var smoothNormals = (index >= 0 && bakeValues.Count > index && bakeValues[index].data != null)
                                ? bakeValues[index].data
                                : SmoothNormals(meshFilter.sharedMesh);

            if (smoothNormals.Count == meshFilter.sharedMesh.vertexCount)
            {
                try {
                    meshFilter.sharedMesh.SetUVs(3, smoothNormals);
                    registeredMeshes.Add(meshFilter.sharedMesh);
                } catch (Exception ex) {
                    Debug.LogError($"Failed to set UVs for mesh {meshFilter.sharedMesh.name}: {ex.Message}", meshFilter.gameObject);
                }
            }
            else if (meshFilter.sharedMesh.vertexCount > 0) // Only warn if vertex count > 0
            {
                 Debug.LogWarning($"Smooth normal count mismatch for mesh {meshFilter.sharedMesh.name}. Expected {meshFilter.sharedMesh.vertexCount}, got {smoothNormals.Count}", meshFilter.gameObject);
            }

        }

        var skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
        {
             if (skinnedMeshRenderer.sharedMesh == null) continue;
             if (registeredMeshes.Contains(skinnedMeshRenderer.sharedMesh)) continue;

            if (skinnedMeshRenderer.sharedMesh.vertexCount > 0)
            {
                try {
                    List<Vector3> emptyNormals = new List<Vector3>(new Vector3[skinnedMeshRenderer.sharedMesh.vertexCount]);
                    skinnedMeshRenderer.sharedMesh.SetUVs(3, emptyNormals);
                    registeredMeshes.Add(skinnedMeshRenderer.sharedMesh);
                } catch (Exception ex) {
                    Debug.LogError($"Failed to clear UV3 for skinned mesh {skinnedMeshRenderer.sharedMesh.name}: {ex.Message}", skinnedMeshRenderer.gameObject);
                }
            }
        }
    }

    List<Vector3> SmoothNormals(Mesh mesh)
    {
        if (mesh == null || mesh.vertexCount == 0) return new List<Vector3>();

        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        if (vertices == null || normals == null || vertices.Length == 0 || normals.Length == 0) {
             //Debug.LogWarning($"Mesh {mesh.name} has no vertices or normals.", mesh);
             return new List<Vector3>();
        }

        if (normals.Length != vertices.Length) {
             Debug.LogError($"Normal count ({normals.Length}) does not match vertex count ({vertices.Length}) for mesh {mesh.name}. Cannot smooth normals.", mesh);
             return new List<Vector3>(normals); // Return original normals as fallback
        }


        List<Vector3> smoothNormals = new List<Vector3>(normals);
        var groups = vertices.Select((vertex, index) => new { vertex, index })
                             .GroupBy(pair => pair.vertex);

        try {
            foreach (var group in groups)
            {
                if (group.Count() <= 1) continue;

                var smoothNormal = Vector3.zero;
                int validCount = 0;
                foreach (var pair in group)
                {
                    if(pair.index < smoothNormals.Count) { // Boundary check
                        smoothNormal += smoothNormals[pair.index];
                        validCount++;
                    }
                }

                if (validCount > 0) {
                    smoothNormal.Normalize();
                } else {
                    smoothNormal = Vector3.up; // Fallback if no valid normals found in group
                }


                foreach (var pair in group)
                {
                    if(pair.index < smoothNormals.Count) { // Boundary check
                       smoothNormals[pair.index] = smoothNormal;
                    }
                }
            }
        } catch (Exception ex) {
            Debug.LogError($"Error during normal smoothing for mesh {mesh.name}: {ex.Message}", mesh);
            return new List<Vector3>(normals); // Return original on error
        }

        return smoothNormals;
    }


    void UpdateMaterialProperties()
    {
        if (!isInitialized || outlineFillMaterial == null || outlineMaskMaterial == null)
        {
            return;
        }

        outlineFillMaterial.SetColor("_OutlineColor", outlineColor);

        float maskZTest;
        float fillZTest;
        float currentOutlineWidth = outlineWidth;

        switch (outlineMode)
        {
            case Mode.OutlineAll:
                maskZTest = (float)UnityEngine.Rendering.CompareFunction.Always;
                fillZTest = (float)UnityEngine.Rendering.CompareFunction.Always;
                break;

            case Mode.OutlineVisible:
                maskZTest = (float)UnityEngine.Rendering.CompareFunction.Always;
                fillZTest = (float)UnityEngine.Rendering.CompareFunction.LessEqual;
                break;

            case Mode.OutlineHidden:
                maskZTest = (float)UnityEngine.Rendering.CompareFunction.Always;
                fillZTest = (float)UnityEngine.Rendering.CompareFunction.Greater;
                break;

            case Mode.OutlineAndSilhouette:
                maskZTest = (float)UnityEngine.Rendering.CompareFunction.LessEqual;
                fillZTest = (float)UnityEngine.Rendering.CompareFunction.Always;
                break;

            case Mode.SilhouetteOnly:
                maskZTest = (float)UnityEngine.Rendering.CompareFunction.LessEqual;
                fillZTest = (float)UnityEngine.Rendering.CompareFunction.Greater;
                currentOutlineWidth = 0f;
                break;

             default: // Fallback or handle unexpected enum value
                maskZTest = (float)UnityEngine.Rendering.CompareFunction.Always;
                fillZTest = (float)UnityEngine.Rendering.CompareFunction.LessEqual;
                break;
        }

         outlineMaskMaterial.SetFloat("_ZTest", maskZTest);
         outlineFillMaterial.SetFloat("_ZTest", fillZTest);
         outlineFillMaterial.SetFloat("_OutlineWidth", currentOutlineWidth);
    }

    public void On()
    {
        this.enabled = true;
    }

    public void Off()
    {
        this.enabled = false;
    }

    public bool IsOutlineEnabled()
    {
        return this.enabled && isInitialized;
    }
}