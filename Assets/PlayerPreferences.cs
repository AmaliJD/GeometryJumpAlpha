using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreferences : MonoBehaviour
{
    

    public void SavePrefs(float music, float sfx)
    {
        PlayerPrefs.SetFloat("music_volume", music);
        PlayerPrefs.SetFloat("sfx_volume", sfx);
        PlayerPrefs.Save();
    }

    public void LoadPrefs()
    {

    }
}
