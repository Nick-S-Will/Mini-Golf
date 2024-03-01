using MiniGolf.Managers.Options;
using MiniGolf.Overlay.UI;
using UnityEditor;
using UnityEditor.UI;

namespace MiniGolf.Editor
{
    [CustomEditor(typeof(AudioSlider))]
    public class AudioSliderEditor : SliderEditor
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

            AudioSlider slider = (AudioSlider)target;
            slider.audioChannel = (AudioChannel)EditorGUILayout.EnumPopup("Channel", slider.audioChannel);
        }
    }
}