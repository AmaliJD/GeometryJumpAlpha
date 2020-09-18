using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColorTrigger))]
public class ColorTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //ColorTrigger trigger = (ColorTrigger)target;

        EditorGUILayout.BeginHorizontal();
        //trigger.channelmode = EditorGUILayout.Toggle("Channel Mode", trigger.channelmode);
        //trigger.copy = EditorGUILayout.Toggle("Copy Color", trigger.copy);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("channelmode"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("copy"), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (serializedObject.FindProperty("channelmode").boolValue)
        {
            /*
            EditorGUILayout.BeginHorizontal();
            trigger.channel = (ColorReference)EditorGUILayout.ObjectField("Channel", trigger.channel, typeof(ColorReference));
            if (trigger.channel != null) { trigger.channel.channelcolor = EditorGUILayout.ColorField(trigger.channel.channelcolor); }
            EditorGUILayout.EndHorizontal();*/

            EditorGUILayout.PropertyField(serializedObject.FindProperty("channel"), true);
        }
        else
        {
            /*
            int size = trigger.objects.Count;

            size = EditorGUILayout.IntField("Size", size);
            for (int i = 0; i < size; i++)
            {
                trigger.objects[i] = (GameObject)EditorGUILayout.ObjectField("Element " + i, trigger.objects[i], typeof(GameObject));
            }*/

            EditorGUILayout.PropertyField(serializedObject.FindProperty("objects"), true);
        }

        if(!serializedObject.FindProperty("copy").boolValue)
        {
            //trigger.new_color = EditorGUILayout.ColorField("New Color", trigger.new_color);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("new_color"), true);
        }
        else
        {
            /*
            EditorGUILayout.BeginHorizontal();
            trigger.copy_color = (ColorReference)EditorGUILayout.ObjectField("Copy Color", trigger.copy_color, typeof(ColorReference));
            if (trigger.copy_color != null) { trigger.copy_color.channelcolor = EditorGUILayout.ColorField(trigger.copy_color.channelcolor); }
            EditorGUILayout.EndHorizontal();*/

            EditorGUILayout.PropertyField(serializedObject.FindProperty("copy_color"), true);
        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("hue"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("sat"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("val"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("alpha"), true);
        //trigger.hue = EditorGUILayout.Slider("Hue", trigger.hue, -360f, 360f);
        //trigger.sat = EditorGUILayout.Slider("Sat", trigger.sat, -1f, 1f);
        //trigger.val = EditorGUILayout.Slider("Val", trigger.val, -1f, 1f);
        //trigger.alpha = EditorGUILayout.Slider("Alpha", trigger.alpha, -1f, 1f);

        //trigger.duration = EditorGUILayout.FloatField("Duration", trigger.duration);
        //trigger.oneuse = EditorGUILayout.Toggle("Oneuse", trigger.oneuse);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("duration"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("oneuse"), true);

        //EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }
}
