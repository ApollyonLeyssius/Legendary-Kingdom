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

        if (GUILayout.Button(
            "Add Point At End"))
        {
            AddPointAtEnd();
        }

        if (GUILayout.Button(
            "Delete Selected Point"))
        {
            DeleteSelectedPoint();
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
    }

    private void OnSceneGUI(
        SceneView sceneView)
    {
        if (path == null)
            return;

        DrawCurve();

        if (editPoints)
            EditPoints();

        DrawInsertButtons();
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

                EditorUtility.SetDirty(path);

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

    private void DrawInsertButtons()
    {
        if (!editPoints ||
            path.points == null ||
            path.points.Length < 2)
        {
            return;
        }

        for (int i = 0;
             i < path.points.Length - 1;
             i++)
        {
            Vector3 a =
                path.transform.TransformPoint(
                    path.points[i]
                );

            Vector3 b =
                path.transform.TransformPoint(
                    path.points[i + 1]
                );

            Vector3 midpoint =
                Vector3.Lerp(
                    a,
                    b,
                    0.5f
                );

            float size =
                HandleUtility.GetHandleSize(
                    midpoint
                ) * 0.08f;

            Handles.color =
                Color.green;

            if (Handles.Button(
                midpoint,
                Quaternion.identity,
                size,
                size,
                Handles.CircleHandleCap))
            {
                InsertPoint(i + 1);

                break;
            }
        }
    }

    private void InsertPoint(
        int index)
    {
        Vector3 newPoint;

        if (index <= 0)
        {
            newPoint =
                path.points[0];
        }
        else if (
            index >= path.points.Length)
        {
            newPoint =
                path.points[
                    path.points.Length - 1
                ];
        }
        else
        {
            newPoint =
                Vector3.Lerp(
                    path.points[index - 1],
                    path.points[index],
                    0.5f
                );
        }

        List<Vector3> points =
            new List<Vector3>(
                path.points
            );

        points.Insert(
            index,
            newPoint
        );

        Undo.RecordObject(
            path,
            "Insert Path Point"
        );

        path.points =
            points.ToArray();

        selectedPoint =
            index;

        path.GenerateMesh();

        EditorUtility.SetDirty(path);

        SceneView.RepaintAll();
    }

    private void AddPointAtEnd()
    {
        if (path.points == null ||
            path.points.Length == 0)
        {
            Debug.LogWarning(
                "The path has no points."
            );

            return;
        }

        Vector3 last =
            path.points[
                path.points.Length - 1
            ];

        Vector3 direction =
            Vector3.forward;

        if (path.points.Length >= 2)
        {
            Vector3 previous =
                path.points[
                    path.points.Length - 2
                ];

            direction =
                (
                    last -
                    previous
                ).normalized;
        }

        Vector3 newPoint =
            last +
            direction * 4f;

        List<Vector3> points =
            new List<Vector3>(
                path.points
            );

        Undo.RecordObject(
            path,
            "Add Path Point"
        );

        points.Add(
            newPoint
        );

        path.points =
            points.ToArray();

        selectedPoint =
            path.points.Length - 1;

        path.GenerateMesh();

        EditorUtility.SetDirty(path);

        SceneView.RepaintAll();
    }

    private void DeleteSelectedPoint()
    {
        if (selectedPoint < 0 ||
            selectedPoint >=
            path.points.Length)
        {
            Debug.LogWarning(
                "Select a point first."
            );

            return;
        }

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

        EditorUtility.SetDirty(path);

        SceneView.RepaintAll();
    }

    private void Regenerate()
    {
        Undo.RecordObject(
            path,
            "Regenerate Path"
        );

        path.GenerateMesh();

        EditorUtility.SetDirty(path);

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

        EditorUtility.SetDirty(path);

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

        selectedPoint = -1;

        SceneView.RepaintAll();
    }
}