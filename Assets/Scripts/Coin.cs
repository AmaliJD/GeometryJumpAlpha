using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Coin : MonoBehaviour
{
    private bool collected = false;
    private Animator animator;
    private SpriteRenderer sprite;
    private CubeController cube;
    private Vector3 jumpPos;

    public AudioSource pickup;
    public Light2D coinLight;
    public ParticleSystem particles;
    public int coinNum;

    private GameManager gamemanager;

    private void Awake()
    {
        gamemanager = FindObjectOfType<GameManager>();
        cube = FindObjectOfType<CubeController>();
        animator = gameObject.GetComponent<Animator>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    private IEnumerator Collect()
    {
        transform.parent = null;
        if (particles.isPlaying) { particles.Stop(); }
        pickup.PlayOneShot(pickup.clip, gamemanager.sfx_volume);
        gamemanager.incrementCoinCount(coinNum);
        animator.speed = 3;
        sprite.color = new Color(1, 1, 1, 1);

        while (sprite.color.a >= .1f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + .03f, transform.position.z);
            sprite.color = new Color(1,1,1, sprite.color.a *.95f);
            coinLight.intensity *= .95f;

            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !collected)
        {
            collected = true;
            StartCoroutine(Collect());
        }
    }
}
