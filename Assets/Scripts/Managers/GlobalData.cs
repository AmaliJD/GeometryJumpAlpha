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

    public int[,] levels_completed_and_coins;
    public float[,] level_times;

    public int total_diamonds;

    /*public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string data)
    {
        JsonUtility.FromJsonOverwrite(data, this);
    }*/
    public GlobalData()
    {
        player_color_1 = new float[4];
        player_color_2 = new float[4];

        icon_index = 0;

        color_availability = new int[32];
        icon_availability = new int[30];

        levels_completed_and_coins = new int[13, 4];
        level_times = new float[13, 2];

        total_diamonds = 0;
    }

    public void SaveLevelData(int level_index, int[] coins, float time, float all_coins_time, int add_diamonds)
    {
        levels_completed_and_coins[level_index, 0] = 1;
        levels_completed_and_coins[level_index, 1] = coins[0];
        levels_completed_and_coins[level_index, 2] = coins[1];
        levels_completed_and_coins[level_index, 3] = coins[2];

        level_times[level_index, 0] = Mathf.Min(time, level_times[level_index, 0]);
        level_times[level_index, 1] = Mathf.Min(all_coins_time, level_times[level_index, 1]);

        total_diamonds += add_diamonds;
    }

    public void SaveColorPurchaces(int index)
    {
        color_availability[index] = 1;
    }

    public void SaveIconPurchaces(int index)
    {
        icon_availability[index] = 1;
    }

    public void SavePlayerSelection(Color p1, Color p2, int i)
    {
        player_color_1[0] = p1.r;
        player_color_1[1] = p1.g;
        player_color_1[2] = p1.b;
        player_color_1[3] = p1.a;

        player_color_2[0] = p2.r;
        player_color_2[1] = p2.g;
        player_color_2[2] = p2.b;
        player_color_2[3] = p2.a;

        icon_index = i;
    }
}
