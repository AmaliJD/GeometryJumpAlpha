using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class ManaCount : MonoBehaviour
{
    GameManager gamemanager;
    public Text text;
    public int count;
    public Light2D[] lights;

    private bool enter = false, exit = true;

    private void Awake()
    {
        gamemanager = GameObject.FindObjectOfType<GameManager>();
    }

    private IEnumerator Show()
    {
        float value = text.color.a;

        while (value < 1)
        {
            text.text = gamemanager.getManaCount() + "/" + count;

            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + .02f);
            
            foreach (Light2D light in lights)
            {
                light.intensity += .02f;
            }
            value += .02f;

            yield return null;
        }
    }

    private IEnumerator Hide()
    {
        float value = text.color.a;

        while (value > 0)
        {
            text.text = gamemanager.getManaCount() + "/" + count;

            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - .01f);
            foreach (Light2D light in lights)
            {
                light.intensity -= .01f;
            }
            value -= .01f;

            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !enter)
        {
            enter = true; exit = false;
            StopAllCoroutines();
            StartCoroutine(Show());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !exit)
        {
            exit = true; enter = false;
            StopAllCoroutines();
            StartCoroutine(Hide());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            text.text = gamemanager.getManaCount() + "/" + count;
            if (gamemanager.getManaCount() >= count) { text.color = new Color(0, 255, 0, text.color.a); }
        }
    }
}
