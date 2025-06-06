using UnityEditor;
using UnityEngine;

namespace Core.Gameplay.SlotMachine
{
    [CustomEditor(typeof(SlotWheel), true)]
    public class SlotWheelEditor : ButtonAudioEditor
    {
        private SerializedProperty m_targetVisual;
        private SerializedProperty m_wheelObject;
        private SerializedProperty m_wheelObjectActivate;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_targetVisual = serializedObject.FindProperty("m_display");
            m_wheelObject = serializedObject.FindProperty("m_wheelObject");
            m_wheelObjectActivate = serializedObject.FindProperty("m_wheelObjectActive");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(m_targetVisual, new GUIContent("Target Visual"));
            EditorGUILayout.PropertyField(m_wheelObject, new GUIContent("Wheel Object"));
            EditorGUILayout.PropertyField(m_wheelObjectActivate, new GUIContent("Wheel Object Activate"));

            SlotWheel wheel = (SlotWheel)target;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("Is Locked", wheel.IsLocked);
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
