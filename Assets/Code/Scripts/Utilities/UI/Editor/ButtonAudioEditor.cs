using UnityEngine;
using UnityEditor.UI;
using UnityEditor;

[CustomEditor(typeof(ButtonAudio), true)]
public class ButtonAudioEditor : ButtonEditor
{
    private SerializedProperty m_onHover;
    private SerializedProperty m_onClick;

    private SerializedProperty m_graphic;
    private SerializedProperty m_normalColor;
    private SerializedProperty m_highlightedColor;
    private SerializedProperty m_pressedColor;
    private SerializedProperty m_selectedColor;
    private SerializedProperty m_disabledColor;

    protected override void OnEnable()
    {
        base.OnEnable();

        m_onHover = serializedObject.FindProperty("m_onHoverSound");
        m_onClick = serializedObject.FindProperty("m_onClickSound");

        m_graphic = serializedObject.FindProperty("m_graphic");
        m_normalColor = serializedObject.FindProperty("m_normalColor");
        m_highlightedColor = serializedObject.FindProperty("m_highlightedColor");
        m_pressedColor = serializedObject.FindProperty("m_pressedColor");
        m_selectedColor = serializedObject.FindProperty("m_selectedColor");
        m_disabledColor = serializedObject.FindProperty("m_disabledColor");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(m_onHover, new GUIContent("Hover Sound"));
        EditorGUILayout.PropertyField(m_onClick, new GUIContent("Click Sound"));

        EditorGUILayout.PropertyField(m_graphic, new GUIContent("Graphic"));
        EditorGUILayout.PropertyField(m_normalColor, new GUIContent("Normal Color"));
        EditorGUILayout.PropertyField(m_highlightedColor, new GUIContent("Highlighted Color"));
        EditorGUILayout.PropertyField(m_pressedColor, new GUIContent("Pressed Color"));
        EditorGUILayout.PropertyField(m_selectedColor, new GUIContent("Selected Color"));
        EditorGUILayout.PropertyField(m_disabledColor, new GUIContent("Disabled Color"));

        serializedObject.ApplyModifiedProperties();
    }
}
