using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PulseTrigger)), CanEditMultipleObjects]
public class PulseTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("channelmode"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("copy"), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (serializedObject.FindProperty("channelmode").boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("channel"), true);
        }
        else
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("objects"), true);
        }

        if (!serializedObject.FindProperty("copy").boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("new_color"), true);
        }
        else
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("copy_color"), true);
        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("hue"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("sat"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("val"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("alpha"), true);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("fadein"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hold"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("duration"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("oneuse"), true);

        serializedObject.ApplyModifiedProperties();
    }
}
