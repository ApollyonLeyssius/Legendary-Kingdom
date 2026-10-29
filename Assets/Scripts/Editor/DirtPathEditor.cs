using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DirtPath))]
public class DirtPathEditor : Editor
{
    private DirtPath path;

    private bool editPoints;

    private int selectedPoint = -1;

    private void OnEnable()
    {
        path =
            (DirtPath)target;

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

        EditorGUILayout.LabelField(
            "Path Editing",
            EditorStyles.boldLabel
        );

        if (GUILayout.Button(
            editPoints
                ? "Stop Editing"
                : "Edit Points"))
        {
            editPoints =
                !editPoints;

            SceneView.RepaintAll();
        }

        if (GUILayout.Button(
            "Regenerate Mesh"))
        {
            Regenerate();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Prefab",
            EditorStyles.boldLabel
        );

        if (GUILayout.Button(
            "Save / Apply Changes"))
        {
            SavePrefab();
        }

        if (GUILayout.Button(
            "Revert Prefab"))
        {
            RevertPrefab();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField(
            "Danger Zone",
            EditorStyles.boldLabel
        );

        GUI.backgroundColor =
            Color.red;

        if (GUILayout.Button(
            "Delete Prefab & Mesh",
            GUILayout.Height(30)))
        {
            DeletePrefabAndMesh();
        }

        GUI.backgroundColor =
            Color.white;
    }

    private void OnSceneGUI(
        SceneView sceneView)
    {
        if (path == null)
            return;

        DrawCurve();

        if (editPoints)
        {
            EditPoints();

            DrawSceneButtons();
        }
    }

    private void DrawCurve()
    {
        List<Vector3> centerline =
            path.GetCenterlinePoints();

        if (centerline.Count < 2)
            return;

        Handles.color =
            Color.yellow;

        for (int i = 0;
             i < centerline.Count - 1;
             i++)
        {
            Vector3 a =
                path.transform.TransformPoint(
                    centerline[i]
                );

            Vector3 b =
                path.transform.TransformPoint(
                    centerline[i + 1]
                );

            Handles.DrawLine(
                a,
                b
            );
        }
    }

    private void EditPoints()
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
                ) * 0.12f;

            Handles.color =
                i == selectedPoint
                    ? Color.yellow
                    : Color.cyan;

            EditorGUI.BeginChangeCheck();

            Vector3 newWorldPoint =
                Handles.FreeMoveHandle(
                    worldPoint,
                    size,
                    Vector3.zero,
                    Handles.SphereHandleCap
                );

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(
                    path,
                    "Move Path Point"
                );

                path.points[i] =
                    path.transform
                        .InverseTransformPoint(
                            newWorldPoint
                        );

                selectedPoint =
                    i;

                path.GenerateMesh();

                EditorUtility.SetDirty(
                    path
                );

                SceneView.RepaintAll();
            }

            if (Handles.Button(
                worldPoint,
                Quaternion.identity,
                size * 0.6f,
                size * 0.6f,
                Handles.DotHandleCap))
            {
                selectedPoint =
                    i;

                Repaint();

                SceneView.RepaintAll();
            }

            Handles.Label(
                worldPoint +
                Vector3.up * 0.35f,
                i.ToString()
            );
        }
    }

    private void DrawSceneButtons()
    {
        if (!editPoints)
            return;

        if (path.points == null ||
            path.points.Length == 0)
            return;

        if (selectedPoint < 0 ||
            selectedPoint >= path.points.Length)
            return;

        Vector3 selectedWorld =
            path.transform.TransformPoint(
                path.points[selectedPoint]
            );

        float handleSize =
            HandleUtility.GetHandleSize(
                selectedWorld
            );

        float buttonSize =
            handleSize * 0.3f;

        Vector3 plusPosition =
            selectedWorld +
            Vector3.up *
            handleSize *
            0.45f;

        Vector3 minusPosition =
            selectedWorld -
            Vector3.up *
            handleSize *
            0.45f;

        Handles.color =
            Color.green;

        if (Handles.Button(
            plusPosition,
            Quaternion.identity,
            buttonSize,
            buttonSize,
            Handles.CircleHandleCap))
        {
            AddPointAfterSelected();
        }

        Handles.Label(
            plusPosition,
            "+"
        );

        Handles.color =
            Color.red;

        if (Handles.Button(
            minusPosition,
            Quaternion.identity,
            buttonSize,
            buttonSize,
            Handles.CircleHandleCap))
        {
            DeleteSelectedPoint();
        }

        Handles.Label(
            minusPosition,
            "-"
        );
    }

    private void AddPointAfterSelected()
    {
        if (path.points == null ||
            path.points.Length == 0)
            return;

        int insertIndex =
            selectedPoint + 1;

        Vector3 newPoint;

        if (selectedPoint >=
            path.points.Length - 1)
        {
            Vector3 last =
                path.points[selectedPoint];

            Vector3 direction =
                Vector3.forward;

            if (path.points.Length >= 2)
            {
                Vector3 previous =
                    path.points[
                        selectedPoint - 1
                    ];

                direction =
                    (
                        last -
                        previous
                    ).normalized;
            }

            newPoint =
                last +
                direction * 4f;
        }
        else
        {
            newPoint =
                Vector3.Lerp(
                    path.points[
                        selectedPoint
                    ],
                    path.points[
                        selectedPoint + 1
                    ],
                    0.5f
                );
        }

        List<Vector3> points =
            new List<Vector3>(
                path.points
            );

        Undo.RecordObject(
            path,
            "Add Path Point"
        );

        points.Insert(
            insertIndex,
            newPoint
        );

        path.points =
            points.ToArray();

        selectedPoint =
            insertIndex;

        path.GenerateMesh();

        EditorUtility.SetDirty(
            path
        );

        SceneView.RepaintAll();
    }

    private void DeleteSelectedPoint()
    {
        if (path.points == null)
            return;

        if (selectedPoint < 0 ||
            selectedPoint >= path.points.Length)
            return;

        if (path.points.Length <= 2)
        {
            Debug.LogWarning(
                "A path needs at least 2 points."
            );

            return;
        }

        Undo.RecordObject(
            path,
            "Delete Path Point"
        );

        List<Vector3> points =
            new List<Vector3>(
                path.points
            );

        points.RemoveAt(
            selectedPoint
        );

        path.points =
            points.ToArray();

        selectedPoint =
            Mathf.Clamp(
                selectedPoint,
                0,
                path.points.Length - 1
            );

        path.GenerateMesh();

        EditorUtility.SetDirty(
            path
        );

        SceneView.RepaintAll();
    }

    private void Regenerate()
    {
        Undo.RecordObject(
            path,
            "Regenerate Path"
        );

        path.GenerateMesh();

        EditorUtility.SetDirty(
            path
        );

        SceneView.RepaintAll();
    }

    private void SavePrefab()
    {
        if (!PrefabUtility.IsPartOfPrefabInstance(
            path.gameObject))
        {
            Debug.Log(
                "This DirtPath is not a prefab instance."
            );

            return;
        }

        path.GenerateMesh();

        EditorUtility.SetDirty(
            path
        );

        PrefabUtility.ApplyPrefabInstance(
            path.gameObject,
            InteractionMode.UserAction
        );

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            "DirtPath changes applied."
        );
    }

    private void RevertPrefab()
    {
        if (!PrefabUtility.IsPartOfPrefabInstance(
            path.gameObject))
        {
            Debug.Log(
                "This DirtPath is not a prefab instance."
            );

            return;
        }

        PrefabUtility.RevertPrefabInstance(
            path.gameObject,
            InteractionMode.UserAction
        );

        selectedPoint =
            -1;

        SceneView.RepaintAll();
    }

    private void DeletePrefabAndMesh()
    {
        GameObject targetObject =
            path.gameObject;

        string prefabPath =
            PrefabUtility
                .GetPrefabAssetPathOfNearestInstanceRoot(
                    targetObject
                );

        if (string.IsNullOrEmpty(
            prefabPath))
        {
            EditorUtility.DisplayDialog(
                "Not a Prefab",
                "This DirtPath is not connected to a prefab asset.",
                "OK"
            );

            return;
        }

        bool confirmed =
            EditorUtility.DisplayDialog(
                "Delete DirtPath?",
                "This will permanently delete:\n\n" +
                "Prefab:\n" +
                prefabPath +
                "\n\n" +
                "And its generated mesh asset.\n\n" +
                "This cannot be undone.",
                "Delete",
                "Cancel"
            );

        if (!confirmed)
            return;

        MeshFilter meshFilter =
            targetObject.GetComponent<MeshFilter>();

        string meshPath =
            null;

        if (meshFilter != null &&
            meshFilter.sharedMesh != null)
        {
            meshPath =
                AssetDatabase.GetAssetPath(
                    meshFilter.sharedMesh
                );
        }

        DestroyImmediate(
            targetObject
        );

        if (AssetDatabase.LoadAssetAtPath<GameObject>(
            prefabPath) != null)
        {
            AssetDatabase.DeleteAsset(
                prefabPath
            );
        }

        if (!string.IsNullOrEmpty(
            meshPath))
        {
            if (AssetDatabase.LoadAssetAtPath<Object>(
                meshPath) != null)
            {
                AssetDatabase.DeleteAsset(
                    meshPath
                );
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            "DirtPath prefab and mesh deleted."
        );
    }
}