using System.Linq;
using UnityEditor;
using UnityEngine;

// Based on https://gist.github.com/mandarinx/ed733369fbb2eea6c7fa9e3da65a0e17
[CustomEditor(typeof(MeshCollider))]
public class VertexVisualizer : Editor
{
    private const string NORMAL_KEY = "_normals_length";
    private const string TANGENT_KEY = "_tangents_length";

    private MeshCollider meshCollider;
    [Min(0f)] private float normalLength = 1f, tangentLength = 1f;

    private void OnEnable()
    {
        meshCollider = target as MeshCollider;
        normalLength = EditorPrefs.GetFloat(NORMAL_KEY);
        tangentLength = EditorPrefs.GetFloat(TANGENT_KEY);
    }

    private void OnSceneGUI()
    {
        if (!meshCollider.enabled) return;

        Mesh mesh = meshCollider.sharedMesh;
        if (mesh == null) return;

        Handles.matrix = meshCollider.transform.localToWorldMatrix;

        var vertices = mesh.vertices;
        DrawLines(vertices, mesh.normals, Color.yellow, normalLength);
        DrawLines(vertices, mesh.tangents.Select(t => (Vector3)t).ToArray(), Color.blue, tangentLength);
    }

    private void DrawLines(Vector3[] starts, Vector3[] deltas, Color color, float length)
    {
        Handles.color = color;

        for (int i = 0; i < starts.Length; i++)
        {
            Handles.DrawLine(starts[i], starts[i] + deltas[i] * length);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUI.BeginChangeCheck();
        normalLength = EditorGUILayout.FloatField("Normal Length", normalLength);
        if (EditorGUI.EndChangeCheck()) EditorPrefs.SetFloat(NORMAL_KEY, normalLength);
        tangentLength = EditorGUILayout.FloatField("Tangent Length", tangentLength);
        if (EditorGUI.EndChangeCheck()) EditorPrefs.SetFloat(TANGENT_KEY, tangentLength);
    }
}