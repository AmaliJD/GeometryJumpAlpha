using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ManaOrb : MonoBehaviour
{
    private bool collected = false;
    private int init = 20;
    private CubeController cube;
    private Vector3 jumpPos;

    public float x, y;

    public Light2D blueLight;
    public Light2D whiteLight;

    public AudioSource pickup, sfx;

    private GameManager gamemanager;

    private void Awake()
    {
        gamemanager = FindObjectOfType<GameManager>();
        jumpPos = transform.position + new Vector3(x, y, 0);
        cube = GameObject.FindObjectOfType<CubeController>();
    }
    /*
    private void Update()
    {
        if(Input.GetKeyDown("x"))
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }*/

    // Update is called once per frame
    /*
    void FixedUpdate()
    {
        if(collected)
        {
            if(init > 0)
            {
                transform.position = Vector3.Lerp(transform.position, jumpPos, .2f);
                init--;
            }
            else
            {
                if (Mathf.Abs(transform.position.x - cube.transform.position.x) >= .3 && Mathf.Abs(transform.position.y - cube.transform.position.y) >= .3)
                {
                    transform.position = Vector3.Lerp(transform.position, new Vector3(cube.transform.position.x, cube.transform.position.y, 0f), .15f);
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, new Vector3(cube.transform.position.x, cube.transform.position.y, 0f), .6f);
                }

                if (Mathf.Abs(transform.position.x - cube.transform.position.x) <= .2 && Mathf.Abs(transform.position.y - cube.transform.position.y) <= .2)
                {
                    gamemanager.incrementManaCount(1);
                    Destroy(gameObject);
                }

                blueLight.intensity *= .95f;
                whiteLight.intensity *= .95f;
            }
        }
    }*/

    private IEnumerator Collect()
    {
        transform.parent = null;
        pickup.PlayOneShot(pickup.clip, gamemanager.sfx_volume);

        while (true)
        {
            if (init > 0)
            {
                transform.position = Vector3.Lerp(transform.position, jumpPos, .2f);
                init--;
            }
            else
            {
                if (Mathf.Abs(transform.position.x - cube.transform.position.x) >= .3 && Mathf.Abs(transform.position.y - cube.transform.position.y) >= .3)
                {
                    transform.position = Vector3.Lerp(transform.position, new Vector3(cube.transform.position.x, cube.transform.position.y, 0f), .3f);
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, new Vector3(cube.transform.position.x, cube.transform.position.y, 0f), .7f); //.66
                }

                if (Mathf.Abs(transform.position.x - cube.transform.position.x) <= .2 && Mathf.Abs(transform.position.y - cube.transform.position.y) <= .2)
                {
                    break;
                }

                blueLight.intensity *= .95f;
                whiteLight.intensity *= .95f;
            }

            yield return null;
        }

        sfx.PlayOneShot(sfx.clip, gamemanager.sfx_volume);
        //sfx.Play();
        gamemanager.incrementManaCount(1);
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
