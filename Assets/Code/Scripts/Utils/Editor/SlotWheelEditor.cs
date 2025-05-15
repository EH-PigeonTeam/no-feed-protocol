using UnityEditor;
using UnityEngine;

namespace Core.Gameplay.SlotMachine
{
    [CustomEditor(typeof(SlotWheel), true)]
    public class SlotWheelEditor : ButtonAudioEditor
    {
        private SerializedProperty m_wheelObject;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_wheelObject = serializedObject.FindProperty("m_wheelObject");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_wheelObject, new GUIContent("Wheel Object"));

            SlotWheel wheel = (SlotWheel)target;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("Is Locked", wheel.IsLocked);
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
