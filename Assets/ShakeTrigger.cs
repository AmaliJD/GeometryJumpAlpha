using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ShakeTrigger : MonoBehaviour
{
    public float intensity, fadein, hold, fadeout;
    public bool holdOnStay, oneuse;

    private GameManager gamemanager;
    private bool entered;

    private void Awake()
    {
        gamemanager = FindObjectOfType<GameManager>();
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    IEnumerator Shake()
    {
        CinemachineBasicMultiChannelPerlin camera = gamemanager.getActiveCamera().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        //camera.m_AmplitudeGain = 0;
        float startingIntensity = camera.m_AmplitudeGain;

        float time = 0;
        while(time < fadein)
        {
            camera.m_AmplitudeGain = Mathf.Lerp(startingIntensity, intensity, time / fadein);
            Debug.Log("fadein: " + camera.m_AmplitudeGain);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0;
        while (holdOnStay ? entered : time < hold)
        {
            camera.m_AmplitudeGain = intensity;
            Debug.Log("hold: " + camera.m_AmplitudeGain);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0;
        while (time < fadeout)
        {
            camera.m_AmplitudeGain = Mathf.Lerp(intensity, 0, time / fadein);
            Debug.Log("fadeout: " + camera.m_AmplitudeGain);
            time += Time.deltaTime;
            yield return null;
        }

        camera.m_AmplitudeGain = 0;

        if(oneuse)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && holdOnStay)
        {
            if(!entered)
            {
                entered = true;
                StopAllCoroutines();
                StartCoroutine(Shake());
            }
            entered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            entered = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !holdOnStay)
        {
            StopAllCoroutines();
            StartCoroutine(Shake());
        }
    }
}
