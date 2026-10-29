
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LootScatterTool : EditorWindow
{
    private GameObject[] prefabs = new GameObject[4];

    private float brushRadius = 5f;
    private float spacing = 2f;
    private float minScale = 0.8f;
    private float maxScale = 1.2f;
    private float maxSlope = 45f;

    private LayerMask terrainLayer = ~0;

    private bool randomRotation = true;
    private bool alignToGround = false;
    private bool eraseMode = false;

    private Transform parent;
    private bool painting;
    private Vector3 lastPaintPosition;
    private int prefabIndex;

    [MenuItem("Tools/Loot & Enemy Scatter")]
    public static void Open()
    {
        GetWindow<LootScatterTool>("Scatter Tool");
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField(
            "Loot & Enemy Scatter",
            EditorStyles.boldLabel
        );

        EditorGUILayout.HelpBox(
            "Add prefabs, then paint them onto your terrain " +
            "in the Scene view. Hold Shift to erase while painting.",
            MessageType.Info
        );

        EditorGUILayout.LabelField("Prefab Palette", EditorStyles.boldLabel);

        for (int i = 0; i < prefabs.Length; i++)
        {
            prefabs[i] = (GameObject)EditorGUILayout.ObjectField(
                "Prefab " + (i + 1),
                prefabs[i],
                typeof(GameObject),
                false
            );
        }

        if (GUILayout.Button("Add Prefab Slot"))
        {
            System.Array.Resize(ref prefabs, prefabs.Length + 1);
        }

        EditorGUILayout.Space();

        brushRadius = EditorGUILayout.Slider(
            "Brush Radius", brushRadius, 0.5f, 30f
        );

        spacing = EditorGUILayout.Slider(
            "Spawn Spacing", spacing, 0.5f, 10f
        );

        minScale = EditorGUILayout.Slider(
            "Minimum Scale", minScale, 0.1f, 3f
        );

        maxScale = EditorGUILayout.Slider(
            "Maximum Scale", maxScale, minScale, 3f
        );

        maxSlope = EditorGUILayout.Slider(
            "Maximum Slope", maxSlope, 0f, 90f
        );

        terrainLayer = LayerMaskField("Ground Layers", terrainLayer);

        randomRotation = EditorGUILayout.Toggle(
            "Random Rotation", randomRotation
        );

        alignToGround = EditorGUILayout.Toggle(
            "Align To Ground", alignToGround
        );

        parent = (Transform)EditorGUILayout.ObjectField(
            "Parent (Optional)",
            parent,
            typeof(Transform),
            true
        );

        EditorGUILayout.Space();

        eraseMode = EditorGUILayout.Toggle(
            "Erase Mode", eraseMode
        );

        EditorGUILayout.HelpBox(
            "Open the Scene view and left-click or drag over the " +
            "ground. Select a prefab slot by clicking its button below.",
            MessageType.None
        );

        EditorGUILayout.LabelField(
            "Selected Prefab: " +
            (prefabIndex + 1),
            EditorStyles.boldLabel
        );

        int columns = 2;

        for (int i = 0; i < prefabs.Length; i += columns)
        {
            EditorGUILayout.BeginHorizontal();

            for (int j = i; j < i + columns && j < prefabs.Length; j++)
            {
                GUI.backgroundColor =
                    prefabIndex == j ? Color.green : Color.white;

                if (GUILayout.Button(
                    prefabs[j] != null
                        ? prefabs[j].name
                        : "Empty Slot " + (j + 1),
                    GUILayout.Height(28)))
                {
                    prefabIndex = j;
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space();

        if (GUILayout.Button("Stop Painting"))
        {
            painting = false;
            SceneView.RepaintAll();
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        bool hitGround = Physics.Raycast(
            ray,
            out RaycastHit hit,
            10000f,
            terrainLayer
        );

        if (hitGround && e.type == EventType.Repaint)
        {
            Handles.color = eraseMode ? Color.red : Color.green;

            Handles.DrawWireDisc(
                hit.point,
                hit.normal,
                brushRadius
            );
        }

        if (e.type == EventType.MouseDown &&
            e.button == 0 &&
            !e.alt &&
            !e.control &&
            !e.command &&
            !e.shift &&
            !e.isMouse &&
            false)
        {
            return;
        }

        if (e.alt)
            return;

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            painting = true;
            lastPaintPosition = Vector3.positiveInfinity;

            if (hitGround)
            {
                PaintAt(hit.point, hit.normal);
                lastPaintPosition = hit.point;
            }

            e.Use();
        }
        else if (e.type == EventType.MouseDrag &&
                 e.button == 0 &&
                 painting)
        {
            if (hitGround &&
                Vector3.Distance(lastPaintPosition, hit.point) >= spacing)
            {
                PaintAt(hit.point, hit.normal);
                lastPaintPosition = hit.point;
            }

            e.Use();
        }
        else if (e.type == EventType.MouseUp && e.button == 0)
        {
            painting = false;
        }

        if (hitGround && e.type == EventType.MouseMove)
            sceneView.Repaint();
    }

    private void PaintAt(Vector3 center, Vector3 normal)
    {
        if (eraseMode)
        {
            EraseAt(center);
            return;
        }

        List<GameObject> validPrefabs = new List<GameObject>();

        foreach (GameObject prefab in prefabs)
        {
            if (prefab != null)
                validPrefabs.Add(prefab);
        }

        if (validPrefabs.Count == 0)
        {
            ShowNotification(new GUIContent("Add a prefab first."));
            return;
        }

        int attempts = Mathf.Max(
            1,
            Mathf.RoundToInt(
                brushRadius * brushRadius /
                Mathf.Max(0.25f, spacing * spacing)
            )
        );

        for (int i = 0; i < attempts; i++)
        {
            Vector2 offset = Random.insideUnitCircle * brushRadius;

            Vector3 origin = center +
                Vector3.right * offset.x +
                Vector3.forward * offset.y +
                Vector3.up * 1000f;

            if (!Physics.Raycast(
                origin,
                Vector3.down,
                out RaycastHit hit,
                2000f,
                terrainLayer))
            {
                continue;
            }

            if (Vector3.Angle(hit.normal, Vector3.up) > maxSlope)
                continue;

            GameObject prefab =
                validPrefabs[Random.Range(0, validPrefabs.Count)];

            GameObject instance =
                (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            if (instance == null)
                continue;

            Undo.RegisterCreatedObjectUndo(
                instance,
                "Scatter Object"
            );

            instance.transform.position = hit.point;

            Quaternion rotation = randomRotation
                ? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
                : Quaternion.identity;

            if (alignToGround)
            {
                rotation = Quaternion.FromToRotation(
                    Vector3.up,
                    hit.normal
                ) * rotation;
            }

            instance.transform.rotation = rotation;

            float scale = Random.Range(minScale, maxScale);
            instance.transform.localScale *= scale;

            if (parent != null)
                instance.transform.SetParent(parent, true);
        }
    }

    private void EraseAt(Vector3 center)
    {
        List<GameObject> toDelete = new List<GameObject>();

        GameObject[] allObjects = FindObjectsByType<GameObject>(
            FindObjectsSortMode.None
        );

        foreach (GameObject obj in allObjects)
        {
            if (obj == null || PrefabUtility.IsPartOfPrefabAsset(obj))
                continue;

            if (parent != null && !obj.transform.IsChildOf(parent))
                continue;

            if (Vector3.Distance(obj.transform.position, center) <= brushRadius)
            {
                if (PrefabUtility.GetCorrespondingObjectFromSource(obj) != null)
                    toDelete.Add(obj);
            }
        }

        foreach (GameObject obj in toDelete)
            Undo.DestroyObjectImmediate(obj);
    }

    private static LayerMask LayerMaskField(
        string label,
        LayerMask selected)
    {
        List<string> names = new List<string>();
        List<int> numbers = new List<int>();

        for (int i = 0; i < 32; i++)
        {
            string name = LayerMask.LayerToName(i);

            if (!string.IsNullOrEmpty(name))
            {
                names.Add(name);
                numbers.Add(i);
            }
        }

        int maskWithoutEmpty = 0;

        for (int i = 0; i < numbers.Count; i++)
        {
            if ((selected.value & (1 << numbers[i])) != 0)
                maskWithoutEmpty |= 1 << i;
        }

        maskWithoutEmpty = EditorGUILayout.MaskField(
            label,
            maskWithoutEmpty,
            names.ToArray()
        );

        int mask = 0;

        for (int i = 0; i < numbers.Count; i++)
        {
            if ((maskWithoutEmpty & (1 << i)) != 0)
                mask |= 1 << numbers[i];
        }

        selected.value = mask;
        return selected;
    }
}
