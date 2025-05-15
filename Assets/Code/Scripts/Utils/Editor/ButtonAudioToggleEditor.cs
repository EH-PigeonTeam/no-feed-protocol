using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ButtonAudioToggle))]
public class ButtonAudioToggleEditor : ButtonAudioEditor
{
    private SerializedProperty m_hoverVisual;
    private SerializedProperty m_activeVisual;
    private SerializedProperty m_disabledVisual;

    private SerializedProperty m_oppositeButton;

    protected override void OnEnable()
    {
        base.OnEnable();

        m_hoverVisual = serializedObject.FindProperty("m_hoverVisual");
        m_activeVisual = serializedObject.FindProperty("m_activeVisual");
        m_disabledVisual = serializedObject.FindProperty("m_disabledVisual");

        m_oppositeButton = serializedObject.FindProperty("m_oppositeButton");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(m_hoverVisual, new GUIContent("Hover Visual"));
        EditorGUILayout.PropertyField(m_activeVisual, new GUIContent("Active Visual"));
        EditorGUILayout.PropertyField(m_disabledVisual, new GUIContent("Disabled Visual"));

        EditorGUILayout.PropertyField(m_oppositeButton, new GUIContent("Opposite Button"));

        serializedObject.ApplyModifiedProperties();
    }
}