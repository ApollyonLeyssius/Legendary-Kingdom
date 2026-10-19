using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class DirtPath : MonoBehaviour
{
    [Header("Path")]
    public Vector3[] points =
        new Vector3[0];

    public float width = 4f;

    [Range(2, 100)]
    public int splineResolution = 20;

    [Header("Terrain Edge Blend")]
    public bool terrainBlend = true;

    [Min(0f)]
    public float blendWidth = 0.75f;

    [Range(2, 20)]
    public int widthSegments = 10;

    [Header("Terrain")]
    public LayerMask terrainLayer;

    public float terrainOffset = 0.03f;

    [Header("Natural Edges")]
    public bool irregularEdges = true;

    [Range(0f, 1f)]
    public float edgeVariation = 0.15f;

    public float edgeNoiseScale = 0.8f;

    [Header("Material")]
    public Material pathMaterial;

    private MeshFilter meshFilter;

    private void Awake()
    {
        meshFilter =
            GetComponent<MeshFilter>();
    }

    public void GenerateMesh()
    {
        if (meshFilter == null)
            meshFilter =
                GetComponent<MeshFilter>();

        List<Vector3> centerline =
            GetCenterlinePoints();

        if (centerline.Count < 2)
        {
            meshFilter.sharedMesh = null;
            return;
        }

        Mesh mesh =
            BuildMesh(centerline);

        meshFilter.sharedMesh =
            mesh;

        MeshRenderer renderer =
            GetComponent<MeshRenderer>();

        if (pathMaterial != null)
            renderer.sharedMaterial =
                pathMaterial;
    }

    public List<Vector3> GetCenterlinePoints()
    {
        List<Vector3> result =
            new List<Vector3>();

        if (points == null ||
            points.Length < 2)
        {
            return result;
        }

        for (int i = 0;
             i < points.Length - 1;
             i++)
        {
            Vector3 p0 =
                i == 0
                    ? points[i]
                    : points[i - 1];

            Vector3 p1 =
                points[i];

            Vector3 p2 =
                points[i + 1];

            Vector3 p3 =
                i + 2 >= points.Length
                    ? p2
                    : points[i + 2];

            for (int j = 0;
                 j < splineResolution;
                 j++)
            {
                float t =
                    j /
                    (float)splineResolution;

                result.Add(
                    PathUtility.GetPoint(
                        p0,
                        p1,
                        p2,
                        p3,
                        t
                    )
                );
            }
        }

        result.Add(
            points[
                points.Length - 1
            ]
        );

        return result;
    }

    private Mesh BuildMesh(
        List<Vector3> centerline)
    {
        Mesh mesh =
            new Mesh();

        mesh.name =
            gameObject.name +
            " Mesh";

        int lengthCount =
            centerline.Count;

        int segmentsAcross =
            terrainBlend
                ? Mathf.Max(
                    6,
                    widthSegments
                )
                : 2;

        int widthCount =
            segmentsAcross + 1;

        Vector3[] vertices =
            new Vector3[
                lengthCount *
                widthCount
            ];

        Vector2[] uv =
            new Vector2[
                lengthCount *
                widthCount
            ];

        int[] triangles =
            new int[
                (lengthCount - 1) *
                segmentsAcross *
                6
            ];

        float[] distances =
            new float[lengthCount];

        float totalDistance = 0f;

        for (int i = 1;
             i < lengthCount;
             i++)
        {
            totalDistance +=
                Vector3.Distance(
                    centerline[i - 1],
                    centerline[i]
                );

            distances[i] =
                totalDistance;
        }

        for (int i = 0;
             i < lengthCount;
             i++)
        {
            Vector3 center =
                centerline[i];

            Vector3 direction;

            if (i == 0)
            {
                direction =
                    centerline[1] -
                    centerline[0];
            }
            else if (i ==
                     lengthCount - 1)
            {
                direction =
                    centerline[i] -
                    centerline[i - 1];
            }
            else
            {
                direction =
                    centerline[i + 1] -
                    centerline[i - 1];
            }

            direction.y = 0f;

            if (direction.sqrMagnitude <
                0.001f)
            {
                direction =
                    Vector3.forward;
            }

            direction.Normalize();

            Vector3 side =
                Vector3.Cross(
                    Vector3.up,
                    direction
                ).normalized;

            float leftVariation =
                GetEdgeVariation(
                    i,
                    distances[i],
                    0f
                );

            float rightVariation =
                GetEdgeVariation(
                    i,
                    distances[i],
                    1000f
                );

            float leftWidth =
                width *
                0.5f *
                (1f + leftVariation);

            float rightWidth =
                width *
                0.5f *
                (1f + rightVariation);

            float outerLeft =
                -leftWidth;

            float outerRight =
                rightWidth;

            if (terrainBlend)
            {
                outerLeft -=
                    blendWidth;

                outerRight +=
                    blendWidth;
            }

            for (int x = 0;
                 x < widthCount;
                 x++)
            {
                float t =
                    x /
                    (float)segmentsAcross;

                float offset =
                    Mathf.Lerp(
                        outerLeft,
                        outerRight,
                        t
                    );

                Vector3 localPoint =
                    center +
                    side * offset;

                Vector3 terrainPoint =
                    GetTerrainPoint(
                        localPoint
                    );

                vertices[
                    i * widthCount + x
                ] =
                    terrainPoint;

                float pathU =
                    Mathf.InverseLerp(
                        -leftWidth,
                        rightWidth,
                        offset
                    );

                uv[
                    i * widthCount + x
                ] =
                    new Vector2(
                        pathU,
                        distances[i] / 4f
                    );
            }
        }

        int triangleIndex = 0;

        for (int i = 0;
             i < lengthCount - 1;
             i++)
        {
            for (int x = 0;
                 x < segmentsAcross;
                 x++)
            {
                int current =
                    i * widthCount + x;

                int next =
                    (i + 1) *
                    widthCount + x;

                triangles[
                    triangleIndex++
                ] =
                    current;

                triangles[
                    triangleIndex++
                ] =
                    next;

                triangles[
                    triangleIndex++
                ] =
                    current + 1;

                triangles[
                    triangleIndex++
                ] =
                    current + 1;

                triangles[
                    triangleIndex++
                ] =
                    next;

                triangles[
                    triangleIndex++
                ] =
                    next + 1;
            }
        }

        mesh.vertices =
            vertices;

        mesh.triangles =
            triangles;

        mesh.uv =
            uv;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    private float GetEdgeVariation(
        int index,
        float distance,
        float offset)
    {
        if (!irregularEdges)
            return 0f;

        float noise =
            Mathf.PerlinNoise(
                distance *
                edgeNoiseScale *
                0.1f,

                offset +
                index * 0.037f
            );

        return
            (noise - 0.5f) *
            2f *
            edgeVariation;
    }

    private Vector3 GetTerrainPoint(
        Vector3 localPoint)
    {
        Vector3 worldPoint =
            transform.TransformPoint(
                localPoint
            );

        Vector3 rayStart =
            worldPoint +
            Vector3.up * 500f;

        Ray ray =
            new Ray(
                rayStart,
                Vector3.down
            );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            1000f,
            terrainLayer,
            QueryTriggerInteraction.Ignore))
        {
            Vector3 terrainPoint =
                hit.point +
                hit.normal *
                terrainOffset;

            return transform.InverseTransformPoint(
                terrainPoint
            );
        }

        return localPoint;
    }
}