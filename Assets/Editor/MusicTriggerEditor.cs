using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MusicTrigger))]
public class MusicTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MusicTrigger trigger = (MusicTrigger)target;

        trigger.mode = (MusicTrigger.Mode)EditorGUILayout.EnumPopup("Mode", trigger.mode);

        if (trigger.mode == MusicTrigger.Mode.volume)
        {
            trigger.volume = EditorGUILayout.FloatField("Volume", trigger.volume);

            trigger.fadetime = EditorGUILayout.FloatField("Fade", trigger.fadetime);
        }
        else if (trigger.mode == MusicTrigger.Mode.music)
        {
            //base.OnInspectorGUI();
            trigger.bgmusic = (AudioSource)EditorGUILayout.ObjectField("Music", trigger.bgmusic, typeof(AudioSource));
        }

        trigger.oneuse = EditorGUILayout.Toggle("oneuse", trigger.oneuse);
    }
}
