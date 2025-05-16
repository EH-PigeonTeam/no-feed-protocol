using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ButtonAudioToggle))]
public class ButtonAudioToggleEditor : ButtonAudioEditor
{

    private SerializedProperty m_targetVisual;
    private SerializedProperty m_hoverVisual;
    private SerializedProperty m_activeVisual;
    private SerializedProperty m_disabledVisual;

    private SerializedProperty m_activeSprite;
    private SerializedProperty m_inactiveSprite;

    private SerializedProperty m_oppositeButton;

    protected override void OnEnable()
    {
        base.OnEnable();

        m_targetVisual = serializedObject.FindProperty("m_targetVisual");
        m_hoverVisual = serializedObject.FindProperty("m_hoverVisual");
        m_activeVisual = serializedObject.FindProperty("m_activeVisual");
        m_disabledVisual = serializedObject.FindProperty("m_disabledVisual");

        m_activeSprite = serializedObject.FindProperty("m_activeSprite");
        m_inactiveSprite = serializedObject.FindProperty("m_inactiveSprite");

        m_oppositeButton = serializedObject.FindProperty("m_oppositeButton");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(m_targetVisual, new GUIContent("Target Visual"));
        EditorGUILayout.PropertyField(m_hoverVisual, new GUIContent("Hover Visual"));
        EditorGUILayout.PropertyField(m_activeVisual, new GUIContent("Active Visual"));
        EditorGUILayout.PropertyField(m_disabledVisual, new GUIContent("Disabled Visual"));

        EditorGUILayout.PropertyField(m_activeSprite, new GUIContent("Active Sprite"));
        EditorGUILayout.PropertyField(m_inactiveSprite, new GUIContent("Inactive Sprite"));

        EditorGUILayout.PropertyField(m_oppositeButton, new GUIContent("Opposite Button"));

        serializedObject.ApplyModifiedProperties();
    }
}