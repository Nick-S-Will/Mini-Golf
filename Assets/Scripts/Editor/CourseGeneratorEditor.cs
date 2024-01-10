using UnityEngine;
using UnityEditor;
using MiniGolf.Terrain;

namespace MiniGolf.Editor
{
    [CustomEditor(typeof(CourseGenerator))]
    public class CourseGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var courseGenerator = (CourseGenerator)target;

            GUILayout.Space(10f);
            if (GUILayout.Button("Generate Course")) courseGenerator.Generate();
            if (GUILayout.Button("Clear Course")) courseGenerator.Clear();
        }
    }
}