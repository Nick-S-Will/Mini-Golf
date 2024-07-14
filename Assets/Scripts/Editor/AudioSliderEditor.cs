using MiniGolf.Audio;
using MiniGolf.Audio.UI;
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
            var oldChannel = slider.channel;
            slider.channel = (Channel)EditorGUILayout.EnumPopup("Channel", slider.channel);
            if (oldChannel != slider.channel) EditorUtility.SetDirty(slider);
        }
    }
}