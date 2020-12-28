using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GlobalData
{
    public float[] player_color_1;
    public float[] player_color_2;
    public int[] color_availability;

    public int icon_index;
    public int[] icon_availability;

    public int[][] levels_completed_and_coins;
    public float[] level_times;

    public string username, password;
    public int total_diamonds;

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string data)
    {
        JsonUtility.FromJsonOverwrite(data, this);
    }
}
