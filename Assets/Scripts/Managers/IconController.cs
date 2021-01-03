using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconController : MonoBehaviour
{
    public ColorReference playercolor1;
    public Color p1;
    public ColorReference playercolor2;
    public Color p2;
    public GameObject[] icon_array;
    public int index;
    public Material trail, wave_trail;
    public ParticleSystem death_effect;

    void Awake()
    {
        //*
        index = Random.Range(0,27);

        float h = 0, h2 = 0, s = 0, v = 0, a = p1.a;
        Color.RGBToHSV(p1, out h, out s, out v);

        float R1 = Random.Range(0f, 360f);
        float R2 = Random.Range(.3f * 360f, .7f * 360f);
        int V = Random.Range(0, 21);

        h += (R1 / 360);
        h2 = h;
        h2 += (R2 / 360);

        if (h > 1) { h -= 1; }
        else if (h < 0) { h += 1; }

        if (h2 > 1) { h2 -= 1; }
        else if (h2 < 0) { h2 += 1; }

        p1 = Color.HSVToRGB(h, V == 20 ? 0 : s, V == 19 ? 0 : v);
        p1.a = a;

        p2 = Color.HSVToRGB(h2, s, v);
        p2.a = a;//*/

        playercolor1.Set(p1);
        playercolor2.Set(p2);

        if(V == 19) { p1 = Color.HSVToRGB(h2, s, v); }

        death_effect.startColor = p1;

        p1.a = .2f;

        trail.SetColor("_BaseColor", p2);
        wave_trail.SetColor("_BaseColor", p1);

        int i = 0;
        foreach(GameObject icon in icon_array)
        {
            if(i != index)
            {
                icon.SetActive(false);
            }
            i++;
        }
    }

    public GameObject getIcon()
    {
        return icon_array[index];
    }
}
