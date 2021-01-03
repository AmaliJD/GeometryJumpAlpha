using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadComponent : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject pulse;

    private bool enter = false, start = false;
    private float red = 255, green = 255, blue = 255, scale = 1;
    private float pulse_speed = .1f;
    void Awake()
    {
        pulse.SetActive(false);

        red = pulse.GetComponent<SpriteRenderer>().color.r;
        green = pulse.GetComponent<SpriteRenderer>().color.g;
        blue = pulse.GetComponent<SpriteRenderer>().color.b;
        scale = pulse.transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (enter)
        {
            PulseSetup();
            enter = false;
            start = true;
            pulse.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        if (start)
        {
            //pulse.transform.localScale = new Vector2(pulse.transform.localScale.x * 1.2f, pulse.transform.localScale.y * 1.2f);
            pulse_speed = 1f / pulse.transform.localScale.x;
            //pulse.transform.localScale = new Vector2(pulse.transform.localScale.x + pulse_speed, pulse.transform.localScale.y + pulse_speed);
            pulse.transform.localScale = Vector3.Lerp(pulse.transform.localScale, Vector3.one * 3, .1f);
            pulse.GetComponent<SpriteRenderer>().color = new Color(red, green, blue, pulse.GetComponent<SpriteRenderer>().color.a * (pulse_speed * 1.1f));
            if (pulse.GetComponent<SpriteRenderer>().color.a <= 0)
            {
                start = false;
                pulse.SetActive(false);
                pulse.transform.localScale = new Vector2(scale, scale);
                pulse.GetComponent<SpriteRenderer>().color = new Color(red, green, blue, 1f);
            }
        }
    }

    void PulseSetup()
    {
        pulse.SetActive(false);
        pulse.transform.localScale = new Vector2(scale, scale);
        pulse.GetComponent<SpriteRenderer>().color = new Color(pulse.GetComponent<SpriteRenderer>().color.r, pulse.GetComponent<SpriteRenderer>().color.g, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            enter = true;
        }
    }
}
