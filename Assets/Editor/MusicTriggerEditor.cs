using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MusicTrigger)), CanEditMultipleObjects]
public class MusicTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        serializedObject.Update();
        //MusicTrigger trigger = (MusicTrigger)target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("mode"), true);

        if (serializedObject.FindProperty("mode").enumValueIndex == 1)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("volume"), true);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("fadetime"), true);
        }
        else if (serializedObject.FindProperty("mode").enumValueIndex == 0)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bgmusic"), true);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("play"), true);

            if(serializedObject.FindProperty("play").boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("playvolume"), true);
            }
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("oneuse"), true);

        //EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }
}
