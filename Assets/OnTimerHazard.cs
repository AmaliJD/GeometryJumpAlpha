using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTimerHazard : MonoBehaviour
{
    public AudioSource sound;
    public Transform player;
    public ParticleSystem particles;
    public BoxCollider2D collider;
    public float inittime, timeon, timeoff, fadein, fadeout;
    public Vector4 startBox, endBox;
    public float minDistance, maxDistance, nullDistance;

    private bool start = false;
    private GameManager gamemanager;

    private void Awake()
    {
        gamemanager = FindObjectOfType<GameManager>();
        particles.Stop();
    }

    private void Start()
    {
        StartCoroutine(Init());
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if(distance > nullDistance)
        {
            sound.volume = 0;
        }
        else if (distance > maxDistance)
        {
            sound.Stop();
        }
        else if(distance < minDistance)
        {
            sound.volume = gamemanager.sfx_volume;
        }
        else
        {
            sound.volume = gamemanager.sfx_volume * (1 - (distance - minDistance) / (maxDistance - minDistance));
        }

        if(start)
        {
            StopAllCoroutines();
            StartCoroutine(Sequence());
            start = false;
        }
    }

    IEnumerator Init()
    {
        float time = 0;
        while (time < inittime)
        {
            time += Time.deltaTime;
            yield return null;
        }

        start = true;
    }

    IEnumerator Sequence()
    {
        sound.Stop();
        particles.Stop();
        collider.size = new Vector2(endBox.x, endBox.y);
        collider.offset = new Vector2(endBox.z, endBox.w);

        float time = 0;
        while(time < timeoff)
        {
            time += Time.deltaTime;
            yield return null;
        }

        if(Vector2.Distance(transform.position, player.position) < nullDistance)
        {
            sound.Play();
            particles.Play();
        }
            

        time = 0;
        while(time/fadein < 1)
        {
            collider.size = Vector2.Lerp(new Vector2(endBox.x, endBox.y), new Vector2(startBox.x, startBox.y), time);
            collider.offset = Vector2.Lerp(new Vector2(endBox.z, endBox.w), new Vector2(startBox.z, startBox.w), time);
            time += Time.deltaTime;
            yield return null;
        }

        collider.size = new Vector2(startBox.x, startBox.y);
        collider.offset = new Vector2(startBox.z, startBox.w);

        time = 0;
        while (time < timeon)
        {
            time += Time.deltaTime;
            yield return null;
        }

        time = 0;
        while (time / fadeout < 1)
        {
            collider.size = Vector2.Lerp(new Vector2(startBox.x, startBox.y), new Vector2(endBox.x, endBox.y), time);
            collider.offset = Vector2.Lerp(new Vector2(startBox.z, startBox.w), new Vector2(endBox.z, endBox.w), time);
            time += Time.deltaTime;
            yield return null;
        }

        collider.size = new Vector2(endBox.x, endBox.y);
        collider.offset = new Vector2(endBox.z, endBox.w);

        start = true;
    }
}
