using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class ColorTrigger : MonoBehaviour
{
    public bool channelmode = true, copy = false;

    public ColorReference channel;
    public List<GameObject> objects;
    public ColorReference copy_color;
    public Color new_color;

    private string channel_id;
    private StopColorChange parent;

    [Range(-360f, 360f)] public float hue;
    [Range(-1f, 1f)] public float sat;
    [Range(-1f, 1f)] public float val;
    [Range(-1f, 1f)] public float alpha;

    private List<Color> curr_color;
    public float duration;
    public bool oneuse = false;
    private bool inuse = false;

    public bool keepcolor;

    void Awake()
    {
        curr_color = new List<Color>();

        // collect id's of each target object/channel
        if (channelmode) { channel_id = channel.name; curr_color.Add(Color.clear); }
        else
        {
            channel_id = "";
            foreach (GameObject obj in objects)
            {
                channel_id += obj.GetHashCode();
                curr_color.Add(Color.clear);
            }
        }

        // hide trigger's texture
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        parent = gameObject.transform.parent.gameObject.GetComponent<StopColorChange>();

        // if copy color
        if(copy && copy_color != null)
        {
            new_color = copy_color.channelcolor;
        }

        float h = 0, s = 0, v = 0, a = new_color.a;
        Color.RGBToHSV(new_color, out h, out s, out v);

        h += (hue / 360);
        s += sat;
        v += val;
        a += alpha;

        if (h > 1) { h -= 1; }
        else if (h < 0) { h += 1; }

        if (s > 1) { s = 1; }
        else if (s < 0) { s = 0; }

        if (v > 1) { v = 1; }
        else if (v < 0) { v = 0; }

        if (a > 1) { a = 1; }
        else if (a < 0) { a = 0; }

        new_color = Color.HSVToRGB(h, s, v);
        new_color.a = a;
    }

    
    private void updateColor()
    {
        // save current color
        if (channelmode) { curr_color[0] = new Color(channel.r, channel.g, channel.b, channel.a); }
        else
        {
            int i = 0;
            foreach (GameObject obj in objects)
            {
                if(obj.GetComponent<SpriteRenderer>() != null)
                {
                    SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
                    curr_color[i] = new Color(renderer.color.r, renderer.color.g, renderer.color.b, renderer.color.a);
                }
                else if (obj.GetComponent<Tilemap>() != null)
                {
                    Tilemap renderer = obj.GetComponent<Tilemap>();
                    curr_color[i] = new Color(renderer.color.r, renderer.color.g, renderer.color.b, renderer.color.a);
                }
                else if (obj.GetComponent<Light2D>() != null)
                {
                    Light2D renderer = obj.GetComponent<Light2D>();
                    curr_color[i] = new Color(renderer.color.r, renderer.color.g, renderer.color.b, renderer.color.a);
                }
                else if (obj.GetComponent<Graphic>() != null)
                {
                    Graphic renderer = obj.GetComponent<Graphic>();
                    curr_color[i] = new Color(renderer.color.r, renderer.color.g, renderer.color.b, renderer.color.a);
                }

                i++;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !inuse)
        {
            inuse = true;
            StopAllCoroutines();

            parent.Send(channel_id);
            parent.setActiveTrigger(gameObject, channel_id);

            updateColor();

            if (channelmode) { StartCoroutine(ChangeColor(0)); return; }
            int i = 0;
            foreach (GameObject obj in objects)
            {
                StartCoroutine(ChangeColor(i));
                i++;
            }
        }
    }

    public void SpawnActivate()
    {
        inuse = true;
        StopAllCoroutines();

        parent.Send(channel_id);
        parent.setActiveTrigger(gameObject, channel_id);

        updateColor();

        if (channelmode) { StartCoroutine(ChangeColor(0)); return; }
        int i = 0;
        foreach (GameObject obj in objects)
        {
            StartCoroutine(ChangeColor(i));
            i++;
        }
    }

    private IEnumerator ChangeColor(int index)
    {
        float time = 0;
        Color curr = curr_color[index];
        GameObject obj = new GameObject();
        if (!channelmode) { obj = objects[index]; }

        if (duration > 0)
        {
            while (curr != new_color)
            {
                //curr_color = Color.Lerp(curr_color, new_color, Mathf.PingPong(Time.time, 1/duration));
                curr = Color.Lerp(curr, new_color, time);

                if (channelmode) { channel.Set(curr); }
                else
                {
                    if (obj.GetComponent<SpriteRenderer>() != null)
                    {
                        SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
                        renderer.color = curr;
                    }
                    else if (obj.GetComponent<Tilemap>() != null)
                    {
                        Tilemap renderer = obj.GetComponent<Tilemap>();
                        renderer.color = curr;
                    }
                    else if (obj.GetComponent<Light2D>() != null)
                    {
                        Light2D renderer = obj.GetComponent<Light2D>();
                        renderer.color = curr;
                    }
                    else if (obj.GetComponent<Graphic>() != null)
                    {
                        Graphic renderer = obj.GetComponent<Graphic>();
                        renderer.color = curr;
                    }
                }

                time += Time.deltaTime / (duration * 100);
                yield return null;
            }
        }

        if (channelmode) { channel.Set(new_color); }
        else
        {
            if (obj.GetComponent<SpriteRenderer>() != null)
            {
                SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
                renderer.color = new_color;
            }
            else if (obj.GetComponent<Tilemap>() != null)
            {
                Tilemap renderer = obj.GetComponent<Tilemap>();
                renderer.color = new_color;
            }
            else if (obj.GetComponent<Light2D>() != null)
            {
                Light2D renderer = obj.GetComponent<Light2D>();
                renderer.color = new_color;
            }
            else if (obj.GetComponent<Graphic>() != null)
            {
                Graphic renderer = obj.GetComponent<Graphic>();
                renderer.color = new_color;
            }
        }

        //Debug.Log("COLOR: " + (channel.channelcolor == new_color));

        if (oneuse)
        {
            Destroy(gameObject);
        }

        inuse = false;
    }

    public void Stop()
    {
        StopAllCoroutines();

        if (!keepcolor)
        {
            if (channelmode) { channel.Set(new_color); }
            else
            {
                foreach (GameObject obj in objects)
                {
                    if (obj.GetComponent<SpriteRenderer>() != null)
                    {
                        SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
                        renderer.color = new_color;
                    }
                    else if (obj.GetComponent<Tilemap>() != null)
                    {
                        Tilemap renderer = obj.GetComponent<Tilemap>();
                        renderer.color = new_color;
                    }
                    else if (obj.GetComponent<Light2D>() != null)
                    {
                        Light2D renderer = obj.GetComponent<Light2D>();
                        renderer.color = new_color;
                    }
                    else if (obj.GetComponent<Graphic>() != null)
                    {
                        Graphic renderer = obj.GetComponent<Graphic>();
                        renderer.color = new_color;
                    }
                }
            }
        }

        inuse = false;
    }

    public bool getFinished()
    {
        return !inuse;
    }
}
