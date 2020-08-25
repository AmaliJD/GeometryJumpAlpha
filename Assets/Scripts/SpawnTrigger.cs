using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    public GameObject[] triggers;
    public float[] delay;
    public bool loop;

    private bool inuse = false;

    private void Awake()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    private IEnumerator Begin()
    {
        int i = 0;

        while(i < triggers.Length)
        {
            float d1 = delay[i], time = 0;
            GameObject trigger = triggers[i];

            while(time <= d1)
            {
                time += Time.deltaTime;
                yield return null;
            }

            if(trigger.GetComponent<MoveTrigger>() != null)
            {
                MoveTrigger move = trigger.GetComponent<MoveTrigger>();
                float d2 = move.getDuration();
                time = 0;

                Debug.Log("Trigger " +  i);
                StartCoroutine(move.Move());
                
                while (time <= d2)
                {
                    time += Time.deltaTime;
                    yield return null;
                }
                while (move.getFinished() != true)
                {
                    Debug.Log("Waiting to finish");
                    yield return null;
                }

                move.StopAllCoroutines();
            }

            i++;

            if(i == triggers.Length && loop)
            {
                i = 0;
            }

            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !inuse)
        {
            inuse = true;
            StartCoroutine(Begin());
        }
    }
}
