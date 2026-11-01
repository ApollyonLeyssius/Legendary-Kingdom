
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Path : MonoBehaviour
{
    [Header("Path Settings")]
    public float width = 4f;

    [Range(2, 100)]
    public int splineResolution = 30;

    [Header("Terrain Wrapping")]
    [Range(2, 24)]
    public int widthSegments = 16;

    public LayerMask terrainLayer;
    public float terrainOffset = 0.02f;
    public float terrainRayHeight = 500f;
    public float terrainRayDistance = 1000f;

    [Header("Texture Blending")]
    public bool enableTextureBlending = true;

    [Min(0f)]
    public float edgeBlendWidth = 0.8f;

    [Min(0f)]
    public float startFadeLength = 2f;

    [Min(0f)]
    public float endFadeLength = 2f;

    [Header("Natural Edges")]
    public bool irregularEdges = true;

    [Range(0f, 1f)]
    public float edgeVariation = 0.15f;

    public float edgeNoiseScale = 0.8f;

    [Header("Material")]
    public Material pathMaterial;

    [Header("Control Points")]
    public Vector3[] points = new Vector3[0];

    private MeshFilter meshFilter;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    public void GenerateMesh()
    {
        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        if (pathMaterial != null && meshRenderer != null)
            meshRenderer.sharedMaterial = pathMaterial;

        List<Vector3> centerline = GetCenterlinePoints();

        if (centerline.Count < 2)
        {
            meshFilter.sharedMesh = null;
            return;
        }

        centerline = Densify(centerline, 0.5f);

        Mesh mesh = PathMeshBuilder.Build(
            transform,
            centerline,
            width,
            widthSegments,
            terrainLayer,
            terrainOffset,
            terrainRayHeight,
            terrainRayDistance,
            enableTextureBlending,
            edgeBlendWidth,
            startFadeLength,
            endFadeLength,
            irregularEdges,
            edgeVariation,
            edgeNoiseScale,
            "Path Mesh"
        );

        if (mesh != null)
            meshFilter.sharedMesh = mesh;
    }

    public List<Vector3> GetCenterlinePoints()
    {
        List<Vector3> result = new List<Vector3>();

        if (points == null || points.Length < 2)
            return result;

        int resolution = Mathf.Max(2, splineResolution);

        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 p0 = i == 0 ? points[i] : points[i - 1];
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 1];
            Vector3 p3 = i + 2 < points.Length
                ? points[i + 2]
                : p2;

            for (int j = 0; j < resolution; j++)
            {
                result.Add(PathUtility.GetPoint(
                    p0, p1, p2, p3, j / (float)resolution));
            }
        }

        result.Add(points[points.Length - 1]);

        return result;
    }

    private List<Vector3> Densify(
        List<Vector3> source,
        float spacing)
    {
        List<Vector3> result = new List<Vector3>();

        spacing = Mathf.Max(0.1f, spacing);

        for (int i = 0; i < source.Count - 1; i++)
        {
            Vector3 a = source[i];
            Vector3 b = source[i + 1];

            float distance = Vector3.Distance(
                transform.TransformPoint(a),
                transform.TransformPoint(b));

            int steps = Mathf.Max(
                1, Mathf.CeilToInt(distance / spacing));

            for (int j = 0; j < steps; j++)
            {
                result.Add(Vector3.Lerp(
                    a, b, j / (float)steps));
            }
        }

        result.Add(source[source.Count - 1]);

        return result;
    }
}
    