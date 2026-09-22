using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Path : MonoBehaviour
{
    [Header("Path Settings")]
    public float width = 4f;

    [Header("Terrain Settings")]
    public LayerMask terrainLayer;
    public float terrainOffset = 0.05f;

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
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        List<Vector3> centerline = GetCenterlinePoints();

        if (centerline.Count < 2)
        {
            Debug.LogWarning("Path needs at least 2 points.");
            return;
        }

        Mesh mesh = new Mesh();

        mesh.name = "Path Mesh";

        Vector3[] vertices =
            new Vector3[centerline.Count * 2];

        int[] triangles =
            new int[(centerline.Count - 1) * 6];

        float halfWidth = width * 0.5f;


        for (int i = 0; i < centerline.Count; i++)
        {
            Vector3 center = centerline[i];

            Vector3 direction;

            if (i == 0)
            {
                direction =
                    centerline[i + 1] - center;
            }

            else if (i == centerline.Count - 1)
            {
                direction =
                    center - centerline[i - 1];
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

            Vector3 left =
                center - side * halfWidth;

            Vector3 right =
                center + side * halfWidth;

            left = GetTerrainPoint(left);
            right = GetTerrainPoint(right);

            vertices[i * 2] = left;
            vertices[i * 2 + 1] = right;
        }


        int triangleIndex = 0;

        for (int i = 0; i < centerline.Count - 1; i++)
        {
            int leftCurrent = i * 2;
            int rightCurrent = i * 2 + 1;

            int leftNext = (i + 1) * 2;
            int rightNext = (i + 1) * 2 + 1;

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


        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        mesh.RecalculateBounds();

        meshFilter.sharedMesh = mesh;
    }


    private List<Vector3> GetCenterlinePoints()
    {
        List<Vector3> result =
            new List<Vector3>();

        if (points.Length < 2)
        {
            return result;
        }

        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector3 p0;
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 1];
            Vector3 p3;

            if (i == 0)
            {
                p0 = p1;
            }
            else
            {
                p0 = points[i - 1];
            }

            if (i + 2 >= points.Length)
            {
                p3 = p2;
            }
            else
            {
                p3 = points[i + 2];
            }

            int resolution = 20;

            for (int j = 0; j < resolution; j++)
            {
                float t =
                    j / (float)resolution;

                Vector3 point =
                    PathUtility.GetPoint(
                        p0,
                        p1,
                        p2,
                        p3,
                        t
                    );

                result.Add(point);
            }
        }

        result.Add(
            points[points.Length - 1]
        );

        return result;
    }


    private Vector3 GetTerrainPoint(Vector3 point)
    {
        Vector3 rayStart =
            point + Vector3.up * 100f;

        Ray ray = new Ray(
            rayStart,
            Vector3.down
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            200f,
            terrainLayer
        ))
        {
            return hit.point +
                   hit.normal * terrainOffset;
        }

        return point;
    }
}