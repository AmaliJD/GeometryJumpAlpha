using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class PulseTrigger : MonoBehaviour
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

    private List<Color> curr_color, old_color;
    public float fadein, hold, duration;
    public bool oneuse = false;
    private bool finished = true, inuse = false;

    // Start is called before the first frame update
    void Awake()
    {
        curr_color = new List<Color>();
        old_color = new List<Color>();

        // collect id's of each target object/channel
        if (channelmode) { channel_id = channel.name; curr_color.Add(Color.clear); old_color.Add(Color.clear); }
        else
        {
            channel_id = "";
            foreach (GameObject obj in objects)
            {
                channel_id += obj.GetHashCode();
                curr_color.Add(Color.clear);
                old_color.Add(Color.clear);
            }
        }

        // hide trigger's texture
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        parent = gameObject.transform.parent.gameObject.GetComponent<StopColorChange>();
        /*
        // if copy color
        if (copy && copy_color != null)
        {
            new_color = copy_color.channelcolor;
            Debug.Log(new_color == copy_color.channelcolor);
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

        duration *= 10;*/
    }

    private void Start()
    {
        if (copy && copy_color != null)
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

        duration *= 10;
        fadein *= 10;
    }

    private void updateColor()
    {
        if (copy && copy_color != null)
        {
            new_color = copy_color.channelcolor;

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

        // svae old color
        if (finished)
        {
            if (channelmode) { old_color[0] = new Color(channel.r, channel.g, channel.b, channel.a); curr_color[0] = new_color; }
            else
            {
                int i = 0;
                foreach (GameObject obj in objects)
                {
                    if (obj.GetComponent<SpriteRenderer>() != null)
                    {
                        SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
                        old_color[i] = new Color(renderer.color.r, renderer.color.g, renderer.color.b, renderer.color.a);
                        curr_color[i] = new_color;
                    }
                    else if (obj.GetComponent<Tilemap>() != null)
                    {
                        Tilemap renderer = obj.GetComponent<Tilemap>();
                        old_color[i] = new Color(renderer.color.r, renderer.color.g, renderer.color.b, renderer.color.a);
                        curr_color[i] = new_color;
                    }
                    else if (obj.GetComponent<Light2D>() != null)
                    {
                        Light2D renderer = obj.GetComponent<Light2D>();
                        old_color[i] = new Color(renderer.color.r, renderer.color.g, renderer.color.b, renderer.color.a);
                        curr_color[i] = new_color;
                    }
                    else if (obj.GetComponent<Graphic>() != null)
                    {
                        Graphic renderer = obj.GetComponent<Graphic>();
                        old_color[i] = new Color(renderer.color.r, renderer.color.g, renderer.color.b, renderer.color.a);
                        curr_color[i] = new_color;
                    }

                    i++;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !inuse)
        {
            if (oneuse) { inuse = true; }
            Enter();
            /*StopAllCoroutines();

            parent.Send(channel_id);
            parent.setActiveTrigger(gameObject, channel_id);

            updateColor();

            if (channelmode) { StartCoroutine(ChangeColor(0)); return; }
            int i = 0;
            foreach (GameObject obj in objects)
            {
                StartCoroutine(ChangeColor(i));
                i++;
            }*/
        }
    }

    public void Enter()
    {
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

    public IEnumerator ChangeColor(int index)
    {
        finished = false;
        float time = 0;

        Color curr = channel.channelcolor;
        Color old = old_color[index];
        GameObject obj = new GameObject();

        if (fadein > 0)
        {
            while (curr != new_color)
            {
                old = old_color[index];
                curr = Color.Lerp(curr, new_color, time / fadein);

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

                time += Time.deltaTime;
                yield return null;
            }
        }

        curr = curr_color[index];

        if (channelmode)
        {
            channel.Set(new_color);
        }
        else if (!channelmode)
        {
            obj = objects[index];

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

        time = 0;

        while (time < hold)
        {
            time += Time.deltaTime;
            yield return null;
        }

        time = 0;

        while (curr != old)
        {
            old = old_color[index];
            curr = Color.Lerp(curr, old, time / duration);

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
            
            time += Time.deltaTime;
            yield return null;
        }

        finished = true;

        if (oneuse)
        {
            Destroy(gameObject);
        }
    }

    public void setOldColor(Color c)
    {
        old_color[0] = c;
    }

    public bool getFinished()
    {
        return finished;
    }

    public void Stop()
    {
        StopAllCoroutines();

        finished = true;
        if (channelmode) { channel.Set(old_color[0]); }
        else
        {
            int i = 0;
            foreach (GameObject obj in objects)
            {
                if (obj.GetComponent<SpriteRenderer>() != null)
                {
                    SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
                    renderer.color = old_color[i];
                }
                else if (obj.GetComponent<Tilemap>() != null)
                {
                    Tilemap renderer = obj.GetComponent<Tilemap>();
                    renderer.color = old_color[i]; ;
                }
                else if (obj.GetComponent<Light2D>() != null)
                {
                    Light2D renderer = obj.GetComponent<Light2D>();
                    renderer.color = old_color[i]; ;
                }
                else if (obj.GetComponent<Graphic>() != null)
                {
                    Graphic renderer = obj.GetComponent<Graphic>();
                    renderer.color = old_color[i]; ;
                }

                i++;
            }
        }
    }
}