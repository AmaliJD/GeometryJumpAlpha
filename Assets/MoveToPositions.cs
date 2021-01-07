using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPositions : MonoBehaviour
{
    public Vector4[] movement; //x, y, duration, waitbefore
    public GameObject eyes, scale;

    private Rigidbody2D body;

    private void Start()
    {
        body = gameObject.GetComponent<Rigidbody2D>(); ;
        StartCoroutine(Move());
    }
    
    public IEnumerator Move()
    {
        int i = 0;
        while(true)
        {
            float time = 0;
            while(time < movement[i].w)
            {
                eyes.transform.localPosition = new Vector3(body.velocity.x / 50, (scale.transform.localScale.y - 1) / 5, 0);
                time += Time.deltaTime;
                yield return null;
            }

            MoveTrigger trigger = new MoveTrigger();
            trigger.group = gameObject;
            trigger.x = movement[i].x;
            trigger.y = movement[i].y;
            trigger.duration = movement[i].z;
            trigger.easing = MoveTrigger.Ease.EaseInOut;
            trigger.userigidbody = true;

            trigger.Initialize();

            StartCoroutine(trigger.Move());
            

            time = 0;
            while (time < movement[i].z)
            {
                eyes.transform.localPosition = new Vector3(body.velocity.x / 50, (scale.transform.localScale.y - 1)/5, 0);
                time += Time.deltaTime;
                yield return null;
            }

            eyes.transform.localPosition = new Vector3(body.velocity.x / 50, (scale.transform.localScale.y - 1) / 5, 0);
            i++;
            if(i >= movement.Length) { i = 0; }
            yield return null;
        }
    }
}
