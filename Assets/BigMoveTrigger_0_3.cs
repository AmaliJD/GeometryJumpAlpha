using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigMoveTrigger_0_3 : MonoBehaviour
{
    private GameManager gamemanager;
    public int count = 100, moveamt = 10;
    public float speed = .3f;
    private int i = 0;
    public GameObject wall;
    public SpriteRenderer white, self;
    public ParticleSystem explosion;
    private void Awake()
    {
        gamemanager = GameObject.FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown("x"))
        {
            count = 0;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && gamemanager.getManaCount() >= count && i == 0)
        {
            i = 1;
            StartCoroutine(Move());
        }
    }

    private IEnumerator Move()
    {
        while (!(white.color.a >= 1))
        {
            white.color = new Color(255,255,255, white.color.a + 0.02f);
            yield return null;
        }

        self.color = Color.clear;
        white.color = Color.clear;
        explosion.Play();

        yield return null;

        for (int j = 0; j < moveamt; j++)
        {
            wall.transform.position = new Vector2(wall.transform.position.x, wall.transform.position.y+ speed);
            yield return null;
        }
        for (int j = 0; j < moveamt + 300; j++)
        {
            yield return null;
        }

        Destroy(gameObject);
    }
}