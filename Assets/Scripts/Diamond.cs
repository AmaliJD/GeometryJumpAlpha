using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Diamond : MonoBehaviour
{
    private bool collected = false;
    private int init = 20;
    private CubeController cube;
    private Vector3 jumpPos;

    public float x, y;

    public Light2D blueLight;

    public AudioSource pickup, sfx;

    private GameManager gamemanager;

    private void Awake()
    {
        gamemanager = FindObjectOfType<GameManager>();
        jumpPos = transform.position + new Vector3(x, y, 0);
        cube = GameObject.FindObjectOfType<CubeController>();
    }
    
    private IEnumerator Collect()
    {
        transform.parent = null;
        pickup.PlayOneShot(pickup.clip, 1f);
        //pickup.Play();

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
                    transform.position = Vector3.Lerp(transform.position, new Vector3(cube.transform.position.x, cube.transform.position.y, 0f), .5f);
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, new Vector3(cube.transform.position.x, cube.transform.position.y, 0f), .66f);
                }

                if (Mathf.Abs(transform.position.x - cube.transform.position.x) <= .2 && Mathf.Abs(transform.position.y - cube.transform.position.y) <= .2)
                {
                    break;
                }

                blueLight.intensity *= .95f;
            }

            yield return null;
        }

        gamemanager.incrementDiamondCount(1);
        sfx.PlayOneShot(sfx.clip, 1f);
        //sfx.Play();
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
