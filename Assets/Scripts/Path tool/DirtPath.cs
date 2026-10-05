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

    [Header("Natural Edges")]
    public bool irregularEdges = true;

    [Range(0f, 1f)]
    public float edgeVariation = 0.15f;

    public float edgeNoiseScale = 0.8f;

    [Header("Material")]
    public Material pathMaterial;

    [Header("Terrain")]
    public LayerMask terrainLayer;

    public float terrainOffset = 0.05f;

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
            return;

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

    public List<Vector3>
        GetCenterlinePoints()
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
            points[points.Length - 1]
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

        int count =
            centerline.Count;

        Vector3[] vertices =
            new Vector3[count * 2];

        Vector2[] uv =
            new Vector2[count * 2];

        int[] triangles =
            new int[(count - 1) * 6];

        float distance = 0f;

        for (int i = 0;
             i < count;
             i++)
        {
            Vector3 center =
                centerline[i];

            Vector3 direction;

            if (i == 0)
            {
                direction =
                    centerline[1] -
                    center;
            }
            else if (i == count - 1)
            {
                direction =
                    center -
                    centerline[count - 2];
            }
            else
            {
                direction =
                    centerline[i + 1] -
                    centerline[i - 1];
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
                    distance,
                    0
                );

            float rightVariation =
                GetEdgeVariation(
                    i,
                    distance,
                    1000
                );

            float leftWidth =
                width * 0.5f *
                (1f + leftVariation);

            float rightWidth =
                width * 0.5f *
                (1f + rightVariation);

            Vector3 left =
                center -
                side * leftWidth;

            Vector3 right =
                center +
                side * rightWidth;

            left =
                GetTerrainPoint(left);

            right =
                GetTerrainPoint(right);

            vertices[i * 2] =
                left;

            vertices[i * 2 + 1] =
                right;

            if (i > 0)
            {
                distance +=
                    Vector3.Distance(
                        centerline[i],
                        centerline[i - 1]
                    );
            }

            float v =
                distance / 4f;

            uv[i * 2] =
                new Vector2(0f, v);

            uv[i * 2 + 1] =
                new Vector2(1f, v);
        }

        int triangleIndex = 0;

        for (int i = 0;
             i < count - 1;
             i++)
        {
            int leftCurrent =
                i * 2;

            int rightCurrent =
                i * 2 + 1;

            int leftNext =
                (i + 1) * 2;

            int rightNext =
                (i + 1) * 2 + 1;

            triangles[triangleIndex++] =
                leftCurrent;

            triangles[triangleIndex++] =
                leftNext;

            triangles[triangleIndex++] =
                rightCurrent;

            triangles[triangleIndex++] =
                rightCurrent;

            triangles[triangleIndex++] =
                leftNext;

            triangles[triangleIndex++] =
                rightNext;
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

        Ray ray =
            new Ray(
                worldPoint +
                Vector3.up * 100f,
                Vector3.down
            );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            200f,
            terrainLayer))
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