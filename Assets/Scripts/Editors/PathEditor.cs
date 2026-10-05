using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    private Path path;

    private bool isDrawing;

    private void OnEnable()
    {
        path =
            (Path)target;

        SceneView.duringSceneGui +=
            OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -=
            OnSceneGUI;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Draw Path"))
        {
            isDrawing = true;
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Stop Drawing"))
        {
            isDrawing = false;
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Generate Preview"))
        {
            Undo.RecordObject(
                path,
                "Generate Path"
            );

            path.GenerateMesh();

            EditorUtility.SetDirty(path);
        }

        if (GUILayout.Button("Finish Path"))
        {
            FinishPath();
        }
    }

    private void OnSceneGUI(
        SceneView sceneView)
    {
        if (path == null)
            return;

        DrawCurve();
        DrawPoints();

        if (isDrawing)
            HandleDrawing();
    }

    private void DrawCurve()
    {
        List<Vector3> points =
            path.GetCenterlinePoints();

        if (points.Count < 2)
            return;

        Handles.color =
            Color.yellow;

        for (int i = 0;
             i < points.Count - 1;
             i++)
        {
            Vector3 a =
                path.transform.TransformPoint(
                    points[i]
                );

            Vector3 b =
                path.transform.TransformPoint(
                    points[i + 1]
                );

            Handles.DrawLine(
                a,
                b
            );
        }
    }

    private void DrawPoints()
    {
        if (path.points == null)
            return;

        for (int i = 0;
             i < path.points.Length;
             i++)
        {
            Vector3 worldPoint =
                path.transform.TransformPoint(
                    path.points[i]
                );

            float size =
                HandleUtility.GetHandleSize(
                    worldPoint
                ) * 0.08f;

            Handles.color =
                Color.cyan;

            Handles.SphereHandleCap(
                0,
                worldPoint,
                Quaternion.identity,
                size,
                EventType.Repaint
            );
        }
    }

    private void HandleDrawing()
    {
        Event e =
            Event.current;

        if (e.type != EventType.MouseDown ||
            e.button != 0 ||
            e.alt)
        {
            return;
        }

        Ray ray =
            HandleUtility.GUIPointToWorldRay(
                e.mousePosition
            );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            Mathf.Infinity))
        {
            AddPoint(
                hit.point
            );

            e.Use();
        }
    }

    private void AddPoint(
        Vector3 worldPoint)
    {
        Vector3 localPoint =
            path.transform.InverseTransformPoint(
                worldPoint
            );

        Undo.RecordObject(
            path,
            "Add Path Point"
        );

        List<Vector3> points =
            new List<Vector3>(
                path.points
            );

        points.Add(
            localPoint
        );

        path.points =
            points.ToArray();

        path.GenerateMesh();

        EditorUtility.SetDirty(path);

        SceneView.RepaintAll();
    }

    private void FinishPath()
    {
        if (path.points == null ||
            path.points.Length < 2)
        {
            Debug.LogWarning(
                "Add at least 2 points."
            );

            return;
        }

        path.GenerateMesh();

        GameObject finished =
            new GameObject(
                "DirtPath"
            );

        finished.transform.position =
            path.transform.position;

        finished.transform.rotation =
            path.transform.rotation;

        finished.transform.localScale =
            path.transform.localScale;

        DirtPath dirtPath =
            finished.AddComponent<DirtPath>();

        dirtPath.points =
            (Vector3[])path.points.Clone();

        dirtPath.width =
            path.width;

        dirtPath.splineResolution =
            path.splineResolution;

        dirtPath.irregularEdges =
            path.irregularEdges;

        dirtPath.edgeVariation =
            path.edgeVariation;

        dirtPath.edgeNoiseScale =
            path.edgeNoiseScale;

        dirtPath.pathMaterial =
            path.pathMaterial;

        dirtPath.terrainLayer =
            path.terrainLayer;

        dirtPath.terrainOffset =
            path.terrainOffset;

        MeshFilter source =
            path.GetComponent<MeshFilter>();

        MeshFilter destination =
            finished.GetComponent<MeshFilter>();

        if (source.sharedMesh != null)
        {
            destination.sharedMesh =
                Instantiate(
                    source.sharedMesh
                );
        }

        MeshRenderer sourceRenderer =
            path.GetComponent<MeshRenderer>();

        MeshRenderer destinationRenderer =
            finished.GetComponent<MeshRenderer>();

        if (sourceRenderer != null)
        {
            destinationRenderer.sharedMaterial =
                sourceRenderer.sharedMaterial;
        }

        Undo.RegisterCreatedObjectUndo(
            finished,
            "Create Dirt Path"
        );

        SaveAsPrefab(
            finished
        );

        Selection.activeGameObject =
            finished;

        path.points =
            new Vector3[0];

        path.GenerateMesh();

        isDrawing = false;

        SceneView.RepaintAll();
    }

    private void SaveAsPrefab(
        GameObject finished)
    {
        string prefabFolder =
            "Assets/Prefabs";

        string meshFolder =
            "Assets/Prefabs/Meshes";

        if (!Directory.Exists(
            prefabFolder))
        {
            Directory.CreateDirectory(
                prefabFolder
            );
        }

        if (!Directory.Exists(
            meshFolder))
        {
            Directory.CreateDirectory(
                meshFolder
            );
        }

        AssetDatabase.Refresh();

        MeshFilter filter =
            finished.GetComponent<MeshFilter>();

        if (filter == null ||
            filter.sharedMesh == null)
        {
            Debug.LogError(
                "DirtPath has no mesh."
            );

            return;
        }

        Mesh mesh =
            filter.sharedMesh;

        if (!AssetDatabase.Contains(mesh))
        {
            string meshPath =
                AssetDatabase.GenerateUniqueAssetPath(
                    meshFolder +
                    "/DirtPathMesh.asset"
                );

            mesh.name =
                "DirtPathMesh";

            AssetDatabase.CreateAsset(
                mesh,
                meshPath
            );

            filter.sharedMesh =
                mesh;
        }

        string prefabPath =
            AssetDatabase.GenerateUniqueAssetPath(
                prefabFolder +
                "/DirtPath.prefab"
            );

        GameObject prefab =
            PrefabUtility.SaveAsPrefabAsset(
                finished,
                prefabPath
            );

        if (prefab == null)
        {
            Debug.LogError(
                "Could not save prefab."
            );

            return;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            "Saved DirtPath prefab: " +
            prefabPath
        );
    }
}