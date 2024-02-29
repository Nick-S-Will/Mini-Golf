using MiniGolf.Managers.SceneTransition;
using MiniGolf.Overlay.UI;
using UnityEditor;
using UnityEditor.UI;

namespace MiniGolf.Editor
{
    [CustomEditor(typeof(SceneChangeButton))]
    public class SceneChangeButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), true);
            EditorGUI.EndDisabledGroup();
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            SceneChangeButton button = (SceneChangeButton)target;
            button.scene = (Scene)EditorGUILayout.EnumPopup("Scene", button.scene);
        }
    }
}