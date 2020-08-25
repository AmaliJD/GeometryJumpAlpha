using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class ColorTrigger : MonoBehaviour
{
    public ColorReference channel;
    //public GameObject group;

    public ColorReference copy_color;
    public Color new_color;
    private string channel_id;
    private StopColorChange parent;

    [SerializeField] [Range(-360f, 360f)] private float hue;
    [SerializeField] [Range(-1f, 1f)] private float sat;
    [SerializeField] [Range(-1f, 1f)] private float val;
    [SerializeField] [Range(-1f, 1f)] private float alpha;

    private Color curr_color;
    public float duration;
    public bool oneuse = false;
    private bool inuse = false;

    // Start is called before the first frame update
    void Awake()
    {
        //if (channel != null) { channel_id = channel.name; }
        //else { channel_id = "object renderer"; }
        channel_id = channel.name;

        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        parent = gameObject.transform.parent.gameObject.GetComponent<StopColorChange>();

        if(copy_color != null)
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
        curr_color = new Color(channel.r, channel.g, channel.b, channel.a);

        /*if (channel != null)
        {
            curr_color = new Color(channel.r, channel.g, channel.b, channel.a);
        }
        else if (group != null)
        {
            SpriteRenderer group_sr = group.GetComponent<SpriteRenderer>();

            if (group_sr != null)
            {
                curr_color = group_sr.color;
            }
        }*/
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
            StartCoroutine(ChangeColor());
        }
    }

    private IEnumerator ChangeColor()
    {
        float time = 0;

        if (duration <= 0)
        {
            channel.Set(new_color);
        }
        else
        {
            while (curr_color != new_color)
            {
                //curr_color = Color.Lerp(curr_color, new_color, Mathf.PingPong(Time.time, 1/duration));
                curr_color = Color.Lerp(curr_color, new_color, time);
                channel.Set(curr_color);
                time += Time.deltaTime / (duration * 100);
                yield return null;
            }

            channel.Set(new_color);
        }

        if(oneuse)
        {
            Destroy(gameObject);
        }

        inuse = false;
    }

    public void Stop()
    {
        channel.Set(new_color);
        StopAllCoroutines();
        inuse = false;
    }
}
