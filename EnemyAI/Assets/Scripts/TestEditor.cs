using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestScript))]
public class TestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TestScript script = (TestScript)target;

        script.a = EditorGUILayout.Vector2Field("a", script.a);
        script.b = EditorGUILayout.Vector2Field("b", script.b);
        script.c = EditorGUILayout.Vector2Field("c", script.c);

        if (GUI.changed)
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }

    [DrawGizmo(GizmoType.Active | GizmoType.Selected)]
    static void DrawGizmos(TestScript script, GizmoType gizmoType)
    {
        Gizmos.DrawWireSphere(script.a, 1.0f);
        Gizmos.DrawWireSphere(script.b, 1.0f);
        Gizmos.DrawWireSphere(script.c, 1.0f);
    }
}

