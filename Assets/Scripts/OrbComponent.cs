using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class OrbComponent : MonoBehaviour
{
    public float speed;
    private float ring_speed = .1f, pulse_speed = .1f;
    public GameObject pulse, ring;
    private Light2D pulse_light;
    private bool enter = false, stay = false, r = false, j = false, isjumping = false;
    private float red = 255, green = 255, blue = 255, scale = 1;

    public Transform TeleportTo;

    public AudioSource sfx;

    private PlayerController player;
    private GameManager gamemanager;
    private ParticleSystem particles;

    // Start is called before the first frame update
    void Awake()
    {
        gamemanager = FindObjectOfType<GameManager>();
        player = gamemanager.getController();
        particles = gameObject.GetComponent<ParticleSystem>();

        pulse_light = pulse.transform.GetChild(0).gameObject.GetComponent<Light2D>();
        pulse.SetActive(false);
        ring.SetActive(false);

        red = pulse.GetComponent<SpriteRenderer>().color.r;
        green = pulse.GetComponent<SpriteRenderer>().color.g;
        blue = pulse.GetComponent<SpriteRenderer>().color.b;
        scale = pulse.transform.localScale.x;
    }

    private void Update()
    {
        if (enter)
        {
            RingSetup();
            enter = false;
            r = true;
            ring.SetActive(true);
        }

        player = gamemanager.getController();
        if (stay && player.isJumping())
        {
            isjumping = true;
            PulseSetup();
            stay = false;
            j = true;
            pulse.SetActive(true);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, speed * Time.deltaTime);

        if(r)
        {
            ring_speed = .07f / ring.transform.localScale.x;
            ring.transform.localScale = new Vector2(ring.transform.localScale.x + ring_speed, ring.transform.localScale.y + ring_speed);
            ring.GetComponent<SpriteRenderer>().color = new Color(ring.GetComponent<SpriteRenderer>().color.r, ring.GetComponent<SpriteRenderer>().color.g, ring.GetComponent<SpriteRenderer>().color.b, ring.GetComponent<SpriteRenderer>().color.a * .85f);
            if(ring.GetComponent<SpriteRenderer>().color.a <= 0)
            {
                r = false;
                ring.SetActive(false);
                ring.transform.localScale = new Vector2(.6f, .6f);
                ring.GetComponent<SpriteRenderer>().color = new Color(ring.GetComponent<SpriteRenderer>().color.r, ring.GetComponent<SpriteRenderer>().color.g, ring.GetComponent<SpriteRenderer>().color.b, 1);
            }
        }
        
        if (j)
        {
            if (pulse.transform.localScale.x == scale && sfx != null) { sfx.PlayOneShot(sfx.clip, 1f); }
            pulse.transform.localScale = new Vector2(pulse.transform.localScale.x * .95f, pulse.transform.localScale.y * .95f); //.92
            pulse.GetComponent<SpriteRenderer>().color = new Color(red, green, blue, pulse.GetComponent<SpriteRenderer>().color.a * .7f); //.92
            pulse_light.intensity = pulse_light.intensity * .9f;
            if (pulse.GetComponent<SpriteRenderer>().color.a <= 0)
            {
                j = false;
                pulse.SetActive(false);
                pulse.transform.localScale = new Vector2(scale, scale);
                pulse.GetComponent<SpriteRenderer>().color = new Color(red, green, blue, 1f);
            }

            if (isjumping)
            {
                isjumping = false;
                player.setIsJumping(false);
            }
        }
    }

    void RingSetup()
    {
        r = false;
        ring.SetActive(false);
        ring.transform.localScale = new Vector2(.6f, .6f);
        ring.GetComponent<SpriteRenderer>().color = new Color(ring.GetComponent<SpriteRenderer>().color.r, ring.GetComponent<SpriteRenderer>().color.g,
                                                        ring.GetComponent<SpriteRenderer>().color.b, 1);
    }

    void PulseSetup()
    {
        j = false;
        pulse.SetActive(false);
        pulse.transform.localScale = new Vector2(scale, scale);
        pulse.GetComponent<SpriteRenderer>().color = new Color(red, green, blue, 1);
        pulse_light.intensity = 1;
    }

    public Transform getTeleport()
    {
        return TeleportTo;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            enter = true;
            stay = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            enter = false;
            stay = false;
        }
    }
    /*
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && stay == false)
        {
            stay = true;
        }
    }*/
}