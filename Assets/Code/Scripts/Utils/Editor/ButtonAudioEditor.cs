using UnityEngine;
using UnityEditor.UI;
using UnityEditor;

[CustomEditor(typeof(ButtonAudio))]
public class ButtonAudioEditor : ButtonEditor
{
    private SerializedProperty m_onHover;
    private SerializedProperty m_onClick;

    protected override void OnEnable()
    {
        base.OnEnable();

        m_onHover = serializedObject.FindProperty("m_onHoverSound");
        m_onClick = serializedObject.FindProperty("m_onClickSound");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(m_onHover, new GUIContent("Hover Sound"));
        EditorGUILayout.PropertyField(m_onClick, new GUIContent("Click Sound"));

        serializedObject.ApplyModifiedProperties();
    }
}
