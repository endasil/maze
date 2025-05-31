using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

[InitializeOnLoad]
public static class AutoSnapAllSelectedWithCtrl
{
    private static readonly Dictionary<Transform, Vector3> LastPositions = new();

    static AutoSnapAllSelectedWithCtrl()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        if (Selection.transforms.Length == 0)
            return;

        Event e = Event.current;
        bool ctrlHeld = e != null && e.control;
        Vector3 snap = GetSnapIncrement();

        foreach (var t in Selection.transforms)
        {
            if (!LastPositions.TryGetValue(t, out var lastPos))
                LastPositions[t] = t.position;

            Vector3 currentPos = t.position;

            if (!Approximately(currentPos, lastPos) && ctrlHeld)
            {
                Vector3 snapped = new Vector3(
                    Mathf.Round(currentPos.x / snap.x) * snap.x,
                    Mathf.Round(currentPos.y / snap.y) * snap.y,
                    Mathf.Round(currentPos.z / snap.z) * snap.z
                );

                if (!Approximately(currentPos, snapped))
                {
                    Undo.RecordObject(t, "Auto Snap to Grid");
                    t.position = snapped;
                    Debug.Log($"Snapped {t.name} to {snapped}");
                }

                LastPositions[t] = snapped;
            }
            else if (!ctrlHeld)
            {
                LastPositions[t] = currentPos;
            }
        }
    }

    private static Vector3 GetSnapIncrement()
    {
        return new Vector3(
            EditorPrefs.GetFloat("MoveSnapX", 1f),
            EditorPrefs.GetFloat("MoveSnapY", 1f),
            EditorPrefs.GetFloat("MoveSnapZ", 1f)
        );
    }

    private static bool Approximately(Vector3 a, Vector3 b)
    {
        return Mathf.Approximately(a.x, b.x)
            && Mathf.Approximately(a.y, b.y)
            && Mathf.Approximately(a.z, b.z);
    }
}
