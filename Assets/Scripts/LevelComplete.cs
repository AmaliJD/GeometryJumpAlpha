using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelComplete : MonoBehaviour
{
    public AudioSource sfx;
    public ParticleSystem rays, burst;

    private GameManager gamemanager;
    private bool active;

    private void Awake()
    {
        gamemanager = FindObjectOfType<GameManager>();
    }
    public void EndSequence()
    {
        rays.Play();
        sfx.PlayOneShot(sfx.clip, gamemanager.sfx_volume);
        StartCoroutine(FinalSequence());
    }
    public IEnumerator FinalSequence()
    {
        float timer = 0;
        while(timer < 1.9f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        burst.Play();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !active)
        {
            active = true;
            EndSequence();
        }
    }
}
