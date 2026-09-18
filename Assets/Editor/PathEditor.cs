using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    private Path path;

    private void OnEnable()
    {
        path = (Path)target;
    }

    private void OnSceneGUI()
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
}