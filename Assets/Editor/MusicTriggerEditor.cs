using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MusicTrigger))]
public class MusicTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        serializedObject.Update();
        //MusicTrigger trigger = (MusicTrigger)target;

        //trigger.mode = (MusicTrigger.Mode)EditorGUILayout.EnumPopup("Mode", trigger.mode);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("mode"), true);

        if (serializedObject.FindProperty("mode").enumValueIndex == 1)
        {
            //trigger.volume = EditorGUILayout.FloatField("Volume", trigger.volume);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("volume"), true);

            //trigger.fadetime = EditorGUILayout.FloatField("Fade", trigger.fadetime);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fadetime"), true);
        }
        else if (serializedObject.FindProperty("mode").enumValueIndex == 0)
        {
            //trigger.bgmusic = (AudioSource)EditorGUILayout.ObjectField("Music", trigger.bgmusic, typeof(AudioSource));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bgmusic"), true);
        }

        //trigger.oneuse = EditorGUILayout.Toggle("oneuse", trigger.oneuse);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("oneuse"), true);

        //EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }
}
