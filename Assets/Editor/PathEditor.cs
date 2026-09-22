using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    private Path path;

    private bool isDrawing = false;

    private void OnEnable()
    {
        path = (Path)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (!isDrawing)
        {
            if (GUILayout.Button("Draw Path"))
                isDrawing = true;
        }
        else
        {
            if (GUILayout.Button("Stop Drawing"))
                isDrawing = false;
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Mesh"))
        {
            path.GenerateMesh();
        }
    }

    private void OnSceneGUI()
    {
        DrawPathWidth();
        DrawPoints();

        if (!isDrawing)
            return;

        HandleDrawing();
    }

    private void DrawPoints()
    {
        for (int i = 0; i < path.points.Length; i++)
        {
            Vector3 point = path.points[i];

            EditorGUI.BeginChangeCheck();

            Vector3 newPosition = Handles.PositionHandle(
                point,
                Quaternion.identity
            );

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(path, "Move Path Point");

                path.points[i] = newPosition;

                EditorUtility.SetDirty(path);
            }

            Handles.Label(
                point + Vector3.up * 0.5f,
                "Point " + i
            );

            if (i > 0)
            {
                Vector3 previousPoint = path.points[i - 1];

                Handles.DrawLine(
                    previousPoint,
                    point
                );
            }
        }
    }

    private void HandleDrawing()
    {
        Event currentEvent = Event.current;

        HandleUtility.AddDefaultControl(
            GUIUtility.GetControlID(FocusType.Passive)
        );

        if (currentEvent.type == EventType.MouseDown &&
            currentEvent.button == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(
                currentEvent.mousePosition
            );

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 hitPoint = hit.point;

                AddPoint(hitPoint);

                SceneView.RepaintAll();

                currentEvent.Use();
            }
        }
    }

    private void AddPoint(Vector3 position)
    {
        Undo.RecordObject(path, "Add Path Point");

        Vector3[] newPoints =
            new Vector3[path.points.Length + 1];

        for (int i = 0; i < path.points.Length; i++)
        {
            newPoints[i] = path.points[i];
        }

        newPoints[newPoints.Length - 1] = position;

        path.points = newPoints;

        EditorUtility.SetDirty(path);
    }

    private void DrawCurve()
    {
        if (path.points.Length < 2)
            return;

        for (int i = 0; i < path.points.Length - 1; i++)
        {
            Vector3 p0;
            Vector3 p1 = path.points[i];
            Vector3 p2 = path.points[i + 1];
            Vector3 p3;

            if (i == 0)
                p0 = p1;
            else
                p0 = path.points[i - 1];

            if (i + 2 >= path.points.Length)
                p3 = p2;
            else
                p3 = path.points[i + 2];

            Vector3 previousPoint = p1;

            int resolution = 20;

            for (int j = 1; j <= resolution; j++)
            {
                float t = j / (float)resolution;

                Vector3 currentPoint =
                    PathUtility.GetPoint(
                        p0,
                        p1,
                        p2,
                        p3,
                        t
                    );

                Handles.DrawLine(
                    previousPoint,
                    currentPoint
                );

                previousPoint = currentPoint;
            }
        }
    }

    private List<Vector3> GetCenterlinePoints()
    {
        List<Vector3> points = new List<Vector3>();

        if (path.points.Length < 2)
            return points;

        for (int i = 0; i < path.points.Length - 1; i++)
        {
            Vector3 p0;
            Vector3 p1 = path.points[i];
            Vector3 p2 = path.points[i + 1];
            Vector3 p3;

            if (i == 0)
                p0 = p1;
            else
                p0 = path.points[i - 1];

            if (i + 2 >= path.points.Length)
                p3 = p2;
            else
                p3 = path.points[i + 2];

            int resolution = 20;

            for (int j = 0; j < resolution; j++)
            {
                float t = j / (float)resolution;

                Vector3 point = PathUtility.GetPoint(
                    p0,
                    p1,
                    p2,
                    p3,
                    t
                );

                points.Add(point);
            }
        }

        points.Add(path.points[path.points.Length - 1]);

        return points;
    }

    private void DrawPathWidth()
    {
        List<Vector3> centerPoints = GetCenterlinePoints();

        if (centerPoints.Count < 2)
            return;

        for (int i = 0; i < centerPoints.Count; i++)
        {
            Vector3 center = centerPoints[i];

            Vector3 direction;

            if (i == 0)
            {
                direction = centerPoints[i + 1] - center;
            }
            else if (i == centerPoints.Count - 1)
            {
                direction = center - centerPoints[i - 1];
            }
            else
            {
                direction =
                    centerPoints[i + 1] -
                    centerPoints[i - 1];
            }

            direction.Normalize();

            Vector3 side =
                Vector3.Cross(Vector3.up, direction).normalized;

            Vector3 left =
                center - side * (path.width * 0.5f);

            Vector3 right =
                center + side * (path.width * 0.5f);

            if (i > 0)
            {
                Vector3 previousCenter = centerPoints[i - 1];

                Vector3 previousDirection;

                if (i - 1 == 0)
                {
                    previousDirection =
                        center - previousCenter;
                }
                else
                {
                    previousDirection =
                        center -
                        centerPoints[i - 2];
                }

                previousDirection.Normalize();

                Vector3 previousSide =
                    Vector3.Cross(
                        Vector3.up,
                        previousDirection
                    ).normalized;

                Vector3 previousLeft =
                    previousCenter -
                    previousSide * (path.width * 0.5f);

                Vector3 previousRight =
                    previousCenter +
                    previousSide * (path.width * 0.5f);

                Handles.DrawLine(previousLeft, left);
                Handles.DrawLine(previousRight, right);
            }
        }
    }
}
    