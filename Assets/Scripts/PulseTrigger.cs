using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PulseTrigger : MonoBehaviour
{
    public ColorReference channel;
    private string channel_id;
    private StopColorChange parent;
    public Color new_color;
    private Color curr_color, old_color;
    public float duration, hold;
    public bool oneuse = false;
    private bool finished = true;

    // Start is called before the first frame update
    void Awake()
    {
        channel_id = channel.name;
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        parent = gameObject.transform.parent.gameObject.GetComponent<StopColorChange>();
        duration *= 10;
    }


    private void updateColor()
    {
        if (finished)
        {
            old_color = new Color(channel.r, channel.g, channel.b, channel.a);
        }
        curr_color = new_color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StopAllCoroutines();
            parent.Send(channel_id);
            parent.setActiveTrigger(gameObject, channel_id);
            updateColor();
            StartCoroutine(ChangeColor());
        }
    }

    private IEnumerator ChangeColor()
    {
        finished = false;
        float time = 0;
        channel.Set(new_color);

        while (time < hold)
        {
            time += Time.deltaTime;
            yield return null;
        }

        time = 0;

        while (curr_color != old_color)
        {
            //curr_color = Color.Lerp(curr_color, new_color, Mathf.PingPong(Time.time, 1/duration));
            curr_color = Color.Lerp(curr_color, old_color, time);
            channel.Set(curr_color);
            time += Time.deltaTime / duration;
            yield return null;
        }

        finished = true;

        if (oneuse)
        {
            Destroy(gameObject);
        }
    }

    public void Stop()
    {
        channel.Set(old_color);
        StopAllCoroutines();
    }
}