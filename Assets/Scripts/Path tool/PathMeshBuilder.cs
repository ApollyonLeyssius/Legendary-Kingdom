
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class PathMeshBuilder
{
    public static Mesh Build(
        Transform pathTransform,
        List<Vector3> centerline,
        float width,
        int widthSegments,
        LayerMask terrainLayer,
        float terrainOffset,
        float rayHeight,
        float rayDistance,
        bool enableTextureBlending,
        float edgeBlendWidth,
        float startFadeLength,
        float endFadeLength,
        bool irregularEdges,
        float edgeVariation,
        float edgeNoiseScale,
        string meshName)
    {
        if (centerline == null || centerline.Count < 2)
            return null;

        int rows = centerline.Count;
        int segments = Mathf.Max(2, widthSegments);
        int verticesPerRow = segments + 1;

        Vector3[] vertices = new Vector3[rows * verticesPerRow];
        Vector3[] normals = new Vector3[vertices.Length];
        Vector2[] uvs = new Vector2[vertices.Length];
        Color[] colors = new Color[vertices.Length];

        int[] triangles = new int[(rows - 1) * segments * 6];

        float halfWidth = Mathf.Max(0.01f, width * 0.5f);
        edgeBlendWidth = Mathf.Max(0f, edgeBlendWidth);

        float[] distances = new float[rows];
        float totalDistance = 0f;

        for (int i = 1; i < rows; i++)
        {
            Vector3 previous =
                pathTransform.TransformPoint(centerline[i - 1]);
            Vector3 current =
                pathTransform.TransformPoint(centerline[i]);

            totalDistance += Vector3.Distance(previous, current);
            distances[i] = totalDistance;
        }

        int failedSamples = 0;

        for (int i = 0; i < rows; i++)
        {
            Vector3 center = pathTransform.TransformPoint(centerline[i]);

            Vector3 direction;

            if (i == 0)
            {
                direction =
                    pathTransform.TransformPoint(centerline[1]) - center;
            }
            else if (i == rows - 1)
            {
                direction =
                    center - pathTransform.TransformPoint(centerline[i - 1]);
            }
            else
            {
                direction =
                    pathTransform.TransformPoint(centerline[i + 1]) -
                    pathTransform.TransformPoint(centerline[i - 1]);
            }

            direction.y = 0f;

            if (direction.sqrMagnitude < 0.0001f)
                direction = Vector3.forward;

            direction.Normalize();

            Vector3 side =
                Vector3.Cross(Vector3.up, direction).normalized;

            float leftVariation = GetVariation(
                irregularEdges, edgeVariation, edgeNoiseScale,
                i, distances[i], 0f);

            float rightVariation = GetVariation(
                irregularEdges, edgeVariation, edgeNoiseScale,
                i, distances[i], 1000f);

            float leftCore = halfWidth * (1f + leftVariation);
            float rightCore = halfWidth * (1f + rightVariation);

            float outerLeft = enableTextureBlending
                ? leftCore + edgeBlendWidth
                : leftCore;

            float outerRight = enableTextureBlending
                ? rightCore + edgeBlendWidth
                : rightCore;

            float startAlpha = !enableTextureBlending ||
                               startFadeLength <= 0.001f
                ? 1f
                : Mathf.SmoothStep(
                    0f, 1f, distances[i] / startFadeLength);

            float endAlpha = !enableTextureBlending ||
                             endFadeLength <= 0.001f
                ? 1f
                : Mathf.SmoothStep(
                    0f, 1f,
                    (totalDistance - distances[i]) / endFadeLength);

            float endFade = Mathf.Min(startAlpha, endAlpha);

            for (int j = 0; j <= segments; j++)
            {
                float t = j / (float)segments;

                // When blending is enabled, include extra mesh width
                // outside the solid dirt area for the feathered edges.
                float lateral = Mathf.Lerp(-outerLeft, outerRight, t);
                Vector3 sample = center + side * lateral;

                Vector3 rayOrigin = sample + Vector3.up * rayHeight;

                int index = i * verticesPerRow + j;

                if (Physics.Raycast(
                    rayOrigin,
                    Vector3.down,
                    out RaycastHit hit,
                    rayDistance,
                    terrainLayer,
                    QueryTriggerInteraction.Ignore))
                {
                    sample = hit.point + hit.normal * terrainOffset;
                    normals[index] = pathTransform.InverseTransformDirection(
                        hit.normal).normalized;
                }
                else
                {
                    failedSamples++;
                    normals[index] = Vector3.up;
                }

                vertices[index] =
                    pathTransform.InverseTransformPoint(sample);

                uvs[index] = new Vector2(t, distances[i] / 4f);

                float coreWidth = Mathf.Lerp(leftCore, rightCore, t);
                float absoluteLateral = Mathf.Abs(lateral);

                float sideAlpha;

                if (!enableTextureBlending ||
                    absoluteLateral <= coreWidth)
                {
                    sideAlpha = 1f;
                }
                else if (edgeBlendWidth <= 0.001f)
                {
                    sideAlpha = 0f;
                }
                else
                {
                    float blendT = Mathf.InverseLerp(
                        coreWidth,
                        coreWidth + edgeBlendWidth,
                        absoluteLateral);

                    sideAlpha = 1f -
                        Mathf.SmoothStep(0f, 1f, blendT);
                }

                float alpha = sideAlpha * endFade;

                colors[index] = new Color(1f, 1f, 1f, alpha);
            }
        }

        int tri = 0;

        for (int i = 0; i < rows - 1; i++)
        {
            for (int j = 0; j < segments; j++)
            {
                int a = i * verticesPerRow + j;
                int b = (i + 1) * verticesPerRow + j;
                int c = a + 1;
                int d = b + 1;

                triangles[tri++] = a;
                triangles[tri++] = b;
                triangles[tri++] = c;

                triangles[tri++] = c;
                triangles[tri++] = b;
                triangles[tri++] = d;
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = meshName;

        if (vertices.Length > 65535)
            mesh.indexFormat = IndexFormat.UInt32;

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.colors = colors;
        mesh.RecalculateBounds();

        if (failedSamples > 0)
        {
            Debug.LogWarning(
                meshName + ": " + failedSamples +
                " vertices missed the terrain. Check the terrain layer, " +
                "TerrainCollider, ray height and ray distance.");
        }

        return mesh;
    }

    private static float GetVariation(
        bool enabled,
        float variation,
        float noiseScale,
        int index,
        float distance,
        float offset)
    {
        if (!enabled)
            return 0f;

        float noise = Mathf.PerlinNoise(
            distance * noiseScale * 0.1f,
            offset + index * 0.037f);

        return (noise - 0.5f) * 2f * variation;
    }
}
