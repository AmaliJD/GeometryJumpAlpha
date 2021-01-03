using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopMove : MonoBehaviour
{
    private List<GameObject> triggers;

    private void Awake()
    {
        triggers = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<MoveTrigger>() != null
                || child.gameObject.GetComponent<RotateTrigger>() != null)
            {
                triggers.Add(child.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Stop();
        }
    }

    private void Stop()
    {
        foreach (GameObject obj in triggers)
        {
            if(obj.GetComponent<MoveTrigger>() != null)
            {
                MoveTrigger m = obj.GetComponent<MoveTrigger>();
                m.StopAllCoroutines();
                m.Reset();
            }
        }
    }
}
