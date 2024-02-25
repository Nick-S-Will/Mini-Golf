using UnityEngine;
using UnityEditor;
using MiniGolf.Terrain;

namespace MiniGolf.Editor
{
    [CustomEditor(typeof(HoleGenerator))]
    public class HoleGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var holeGenerator = (HoleGenerator)target;

            GUILayout.Space(10f);
            if (GUILayout.Button("Generate Hole")) holeGenerator.Generate();
            if (GUILayout.Button("Clear")) holeGenerator.Clear();
        }
    }
}