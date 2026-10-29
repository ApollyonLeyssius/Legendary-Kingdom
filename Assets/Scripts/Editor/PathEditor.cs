
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
        path = (Path)target;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(
            "Path Drawing",
            EditorStyles.boldLabel);

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
            Undo.RecordObject(path, "Generate Path");
            path.GenerateMesh();
            EditorUtility.SetDirty(path);
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Finish Path"))
            FinishPath();

        EditorGUILayout.HelpBox(
            "While drawing, left-click the terrain once to add each point. " +
            "Dragging will not add points. Use Enable Texture Blending " +
            "to toggle soft edges and end fades.",
            MessageType.Info);
    }

    private void OnSceneGUI(SceneView sceneView)
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
        List<Vector3> centerline = path.GetCenterlinePoints();

        if (centerline.Count < 2)
            return;

        Handles.color = Color.yellow;

        for (int i = 0; i < centerline.Count - 1; i++)
        {
            Vector3 a = path.transform.TransformPoint(centerline[i]);
            Vector3 b = path.transform.TransformPoint(centerline[i + 1]);

            Handles.DrawLine(a, b);
        }
    }

    private void DrawPoints()
    {
        if (path.points == null)
            return;

        for (int i = 0; i < path.points.Length; i++)
        {
            Vector3 worldPoint =
                path.transform.TransformPoint(path.points[i]);

            float size =
                HandleUtility.GetHandleSize(worldPoint) * 0.08f;

            Handles.color = Color.cyan;

            Handles.SphereHandleCap(
                0,
                worldPoint,
                Quaternion.identity,
                size,
                EventType.Repaint);

            Handles.Label(
                worldPoint + Vector3.up * size * 1.5f,
                i.ToString());
        }
    }

    private void HandleDrawing()
    {
        Event e = Event.current;

        if (e.type != EventType.MouseDown ||
            e.button != 0 ||
            e.alt)
            return;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            Mathf.Infinity,
            path.terrainLayer,
            QueryTriggerInteraction.Ignore))
        {
            AddPoint(hit.point);
            e.Use();
        }
    }

    private void AddPoint(Vector3 worldPoint)
    {
        Vector3 localPoint =
            path.transform.InverseTransformPoint(worldPoint);

        Undo.RecordObject(path, "Add Path Point");

        List<Vector3> points = new List<Vector3>(path.points);
        points.Add(localPoint);
        path.points = points.ToArray();

        path.GenerateMesh();

        EditorUtility.SetDirty(path);
        SceneView.RepaintAll();
    }

    private void FinishPath()
    {
        if (path.points == null || path.points.Length < 2)
        {
            EditorUtility.DisplayDialog(
                "Cannot Finish Path",
                "Add at least 2 path points first.",
                "OK");
            return;
        }

        path.GenerateMesh();

        GameObject finished = new GameObject("DirtPath");

        finished.transform.position = path.transform.position;
        finished.transform.rotation = path.transform.rotation;
        finished.transform.localScale = path.transform.localScale;

        DirtPath dirtPath = finished.AddComponent<DirtPath>();

        CopySettings(path, dirtPath);

        MeshRenderer sourceRenderer =
            path.GetComponent<MeshRenderer>();

        MeshRenderer destinationRenderer =
            finished.GetComponent<MeshRenderer>();

        if (sourceRenderer != null)
        {
            destinationRenderer.sharedMaterials =
                sourceRenderer.sharedMaterials;
        }

        dirtPath.GenerateMesh();

        MeshFilter destinationFilter =
            finished.GetComponent<MeshFilter>();

        if (destinationFilter.sharedMesh == null)
        {
            DestroyImmediate(finished);
            Debug.LogError("Finished path mesh generation failed.");
            return;
        }

        if (!SaveAsPrefab(finished))
        {
            Debug.LogError(
                "Prefab saving failed. The scene object was kept.");

            Undo.RegisterCreatedObjectUndo(
                finished, "Create Dirt Path");

            Selection.activeGameObject = finished;
            return;
        }

        Undo.RegisterCreatedObjectUndo(
            finished, "Create Dirt Path");

        Selection.activeGameObject = finished;

        Undo.RecordObject(path, "Clear Finished Path");
        path.points = new Vector3[0];
        path.GenerateMesh();

        EditorUtility.SetDirty(path);

        isDrawing = false;
        SceneView.RepaintAll();

        Debug.Log("Path finished and saved as a prefab.");
    }

    private void CopySettings(Path source, DirtPath destination)
    {
        destination.points = (Vector3[])source.points.Clone();
        destination.width = source.width;
        destination.splineResolution = source.splineResolution;

        destination.widthSegments = source.widthSegments;
        destination.terrainLayer = source.terrainLayer;
        destination.terrainOffset = source.terrainOffset;
        destination.terrainRayHeight = source.terrainRayHeight;
        destination.terrainRayDistance = source.terrainRayDistance;

        destination.enableTextureBlending =
            source.enableTextureBlending;

        destination.edgeBlendWidth = source.edgeBlendWidth;
        destination.startFadeLength = source.startFadeLength;
        destination.endFadeLength = source.endFadeLength;

        destination.irregularEdges = source.irregularEdges;
        destination.edgeVariation = source.edgeVariation;
        destination.edgeNoiseScale = source.edgeNoiseScale;

        destination.pathMaterial = source.pathMaterial;
    }

    private bool SaveAsPrefab(GameObject finished)
    {
        const string prefabFolder = "Assets/Prefabs";
        const string meshFolder = "Assets/Prefabs/Meshes";

        if (!Directory.Exists(prefabFolder))
            Directory.CreateDirectory(prefabFolder);

        if (!Directory.Exists(meshFolder))
            Directory.CreateDirectory(meshFolder);

        AssetDatabase.Refresh();

        MeshFilter filter = finished.GetComponent<MeshFilter>();

        if (filter == null || filter.sharedMesh == null)
        {
            Debug.LogError("DirtPath has no mesh to save.");
            return false;
        }

        Mesh mesh = filter.sharedMesh;

        if (!AssetDatabase.Contains(mesh))
        {
            string meshPath = AssetDatabase.GenerateUniqueAssetPath(
                meshFolder + "/DirtPathMesh.asset");

            mesh.name = "DirtPathMesh";

            AssetDatabase.CreateAsset(mesh, meshPath);
            filter.sharedMesh = mesh;
        }

        string prefabPath = AssetDatabase.GenerateUniqueAssetPath(
            prefabFolder + "/DirtPath.prefab");

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(
            finished, prefabPath);

        if (prefab == null)
        {
            Debug.LogError("Unity could not save the prefab.");
            return false;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Saved prefab: " + prefabPath);
        return true;
    }
}
