using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTrigger : MonoBehaviour
{
    public GameObject group;
    public float x, y, z;
    public float duration;

    private bool finished = true;
    public enum Ease
    {
        Linear,
        EaseInOut, EaseIn, EaseOut,
        ElasInOut, ElasIn, ElasOut,
        ExpoInOut, ExpoIn, ExpoOut,
        SinInOut, SinIn, SinOut,
        BackInOut, BackIn, BackOut,
        BounceInOut, BounceIn, BounceOut
    };
    public Ease easing;

    private bool inuse;
    private Vector2 original_position;

    private void Awake()
    {
        original_position = group.transform.position;
        x /= 10; y /= 10; z /= 10;

        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public IEnumerator Move()
    {
        finished = false;

        float time = 0, step = (Time.deltaTime / duration) / 10, t = 0, t0 = 0, d = duration;
        float x1 = 0, y1 = 0, z1 = 0, x0 = 0, y0 = 0, z0 = 0, xPos = 0, yPos = 0, zPos = 0;

        while (time <= duration)
        {
            if (duration == 0) { break; }

            float p, s;
            float u0, u1, u2, u3;
            t = time;

            switch (easing)
            {
                case Ease.Linear:
                    x1 = x * (t / d);
                    y1 = y * (t / d);
                    z1 = z * (t / d);

                    x0 = x * (t0 / d);
                    y0 = y * (t0 / d);
                    z0 = z * (t0 / d);
                    break;

                case Ease.EaseInOut:
                    t /= d / 2;

                    if (t < 1)
                    {
                        x1 = x / 2 * t * t * t;
                        y1 = y / 2 * t * t * t;
                    }
                    else
                    {
                        t -= 2;
                        x1 = x / 2 * (t * t * t + 2);
                        y1 = y / 2 * (t * t * t + 2);
                    }

                    t0 /= d / 2;
                    if (t0 < 1)
                    {
                        x0 = x / 2 * t0 * t0 * t0;
                        y0 = y / 2 * t0 * t0 * t0;
                    }
                    else
                    {
                        t0 -= 2;
                        x0 = x / 2 * (t0 * t0 * t0 + 2);
                        y0 = y / 2 * (t0 * t0 * t0 + 2);
                    }

                    break;

                case Ease.EaseIn:
                    t /= d;
                    x1 = x * t * t * t;
                    y1 = y * t * t * t;

                    t0 /= d;
                    x0 = x * t0 * t0 * t0;
                    y0 = y * t0 * t0 * t0;
                    break;

                case Ease.EaseOut:
                    t /= d;
                    t--;
                    x1 = x * (t * t * t + 1);
                    y1 = y * (t * t * t + 1);

                    t0 /= d;
                    t0--;
                    x0 = x * (t0 * t0 * t0 + 1);
                    y0 = y * (t0 * t0 * t0 + 1);
                    break;

                case Ease.ElasInOut:
                    if ((t /= d / 2) == 2)
                    {
                        x1 = x;
                        y1 = y;
                    }
                    else
                    {
                        p = d * (.3f * 1.5f);
                        s = p / 4;

                        if (t < 1)
                        {
                            x1 = -.5f * (x * Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p));
                            y1 = -.5f * (y * Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p));
                        }
                        else
                        {
                            x1 = x * Mathf.Pow(2, -10 * (t -= 1)) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p) * .5f + x;
                            y1 = y * Mathf.Pow(2, -10 * (t -= 1)) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p) * .5f + y;
                        }
                    }

                    if ((t0 /= d / 2) == 2)
                    {
                        x0 = x;
                        y0 = y;
                    }
                    else
                    {
                        p = d * (.3f * 1.5f);
                        s = p / 4;

                        if (t0 < 1)
                        {
                            x0 = -.5f * (x * Mathf.Pow(2, 10 * (t0 -= 1)) * Mathf.Sin((t0 * d - s) * (2 * Mathf.PI) / p));
                            y0 = -.5f * (y * Mathf.Pow(2, 10 * (t0 -= 1)) * Mathf.Sin((t0 * d - s) * (2 * Mathf.PI) / p));
                        }
                        else
                        {
                            x0 = x * Mathf.Pow(2, -10 * (t0 -= 1)) * Mathf.Sin((t0 * d - s) * (2 * Mathf.PI) / p) * .5f + x;
                            y0 = y * Mathf.Pow(2, -10 * (t0 -= 1)) * Mathf.Sin((t0 * d - s) * (2 * Mathf.PI) / p) * .5f + y;
                        }
                    }

                    break;

                case Ease.ElasIn:
                    if ((t /= d) == 1)
                    {
                        x1 = x;
                        y1 = y;
                    }
                    else
                    {
                        p = d * .3f;
                        s = p / 4;

                        x1 = -(x * Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p));
                        y1 = -(y * Mathf.Pow(2, 10 * (t -= 1)) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p));
                    }

                    if ((t0 /= d) == 1)
                    {
                        x0 = x;
                        y0 = y;
                    }
                    else
                    {
                        p = d * .3f;
                        s = p / 4;

                        x0 = -(x * Mathf.Pow(2, 10 * (t0 -= 1)) * Mathf.Sin((t0 * d - s) * (2 * Mathf.PI) / p));
                        y0 = -(y * Mathf.Pow(2, 10 * (t0 -= 1)) * Mathf.Sin((t0 * d - s) * (2 * Mathf.PI) / p));
                    }
                    break;

                case Ease.ElasOut:
                    if ((t /= d) == 1)
                    {
                        x1 = x;
                        y1 = y;
                    }
                    else
                    {
                        p = d * .3f;
                        s = p / 4f;

                        x1 = x * Mathf.Pow(2, -10 * t) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p) + x;
                        y1 = y * Mathf.Pow(2, -10 * t) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p) + y;
                    }

                    if ((t0 /= d) == 1)
                    {
                        x0 = x;
                        y0 = y;
                    }
                    else
                    {
                        p = d * .3f;
                        s = p / 4f;

                        x0 = x * Mathf.Pow(2, -10 * t0) * Mathf.Sin((t0 * d - s) * (2 * Mathf.PI) / p) + x;
                        y0 = y * Mathf.Pow(2, -10 * t0) * Mathf.Sin((t0 * d - s) * (2 * Mathf.PI) / p) + y;
                    }

                    break;

                case Ease.ExpoInOut:
                    u0 = 1f; u1 = 0f; u2 = 0f; u3 = 1f;

                    x1 = x * cubic_bezier(t / d, u0, u1, u2, u3);
                    x0 = x * cubic_bezier(t0 / d, u0, u1, u2, u3);

                    y1 = y * cubic_bezier(t / d, u0, u1, u2, u3);
                    y0 = y * cubic_bezier(t0 / d, u0, u1, u2, u3);

                    break;

                case Ease.ExpoIn:
                    u0 = 1f; u1 = 0f; u2 = 1f; u3 = 0f;

                    x1 = x * cubic_bezier(t / d, u0, u1, u2, u3);
                    x0 = x * cubic_bezier(t0 / d, u0, u1, u2, u3);

                    y1 = y * cubic_bezier(t / d, u0, u1, u2, u3);
                    y0 = y * cubic_bezier(t0 / d, u0, u1, u2, u3);

                    break;

                case Ease.ExpoOut:
                    u0 = 0f; u1 = 1f; u2 = 0f; u3 = 1f;

                    x1 = x * cubic_bezier(t / d, u0, u1, u2, u3);
                    x0 = x * cubic_bezier(t0 / d, u0, u1, u2, u3);

                    y1 = y * cubic_bezier(t / d, u0, u1, u2, u3);
                    y0 = y * cubic_bezier(t0 / d, u0, u1, u2, u3);

                    break;

                case Ease.SinInOut:
                    x1 = (-x / 2) * (Mathf.Cos(Mathf.PI * (t / d)) - 1);
                    y1 = (-y / 2) * (Mathf.Cos(Mathf.PI * (t / d)) - 1);
                    x0 = (-x / 2) * (Mathf.Cos(Mathf.PI * (t0 / d)) - 1);
                    y0 = (-y / 2) * (Mathf.Cos(Mathf.PI * (t0 / d)) - 1);
                    break;

                case Ease.SinIn:
                    x1 = -x * Mathf.Cos(t / d * (Mathf.PI / 2)) + x;
                    y1 = -y * Mathf.Cos(t / d * (Mathf.PI / 2)) + y;
                    x0 = -x * Mathf.Cos(t0 / d * (Mathf.PI / 2)) + x;
                    y0 = -y * Mathf.Cos(t0 / d * (Mathf.PI / 2)) + y;
                    break;

                case Ease.SinOut:
                    x1 = x * Mathf.Sin(t / d * (Mathf.PI / 2));
                    y1 = y * Mathf.Sin(t / d * (Mathf.PI / 2));
                    x0 = x * Mathf.Sin(t0 / d * (Mathf.PI / 2));
                    y0 = y * Mathf.Sin(t0 / d * (Mathf.PI / 2));
                    break;

                case Ease.BackInOut:
                    u0 = .43f; u1 = -.5f; u2 = .57f; u3 = 1.5f;

                    x1 = x * cubic_bezier(t / d, u0, u1, u2, u3);
                    x0 = x * cubic_bezier(t0 / d, u0, u1, u2, u3);

                    y1 = y * cubic_bezier(t / d, u0, u1, u2, u3);
                    y0 = y * cubic_bezier(t0 / d, u0, u1, u2, u3);

                    break;

                case Ease.BackIn:
                    u0 = .43f; u1 = -.5f; u2 = 1f; u3 = 1f;

                    x1 = x * cubic_bezier(t / d, u0, u1, u2, u3);
                    x0 = x * cubic_bezier(t0 / d, u0, u1, u2, u3);

                    y1 = y * cubic_bezier(t / d, u0, u1, u2, u3);
                    y0 = y * cubic_bezier(t0 / d, u0, u1, u2, u3);

                    break;

                case Ease.BackOut:
                    u0 = 0f; u1 = 0f; u2 = .57f; u3 = 1.5f;

                    x1 = x * cubic_bezier(t / d, u0, u1, u2, u3);
                    x0 = x * cubic_bezier(t0 / d, u0, u1, u2, u3);

                    y1 = y * cubic_bezier(t / d, u0, u1, u2, u3);
                    y0 = y * cubic_bezier(t0 / d, u0, u1, u2, u3);

                    break;

                case Ease.BounceInOut:
                    s = 1.70158f;
                    x1 = x * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1);
                    y1 = y * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1);
                    x0 = x * ((t0 = t0 / d - 1) * t0 * ((s + 1) * t0 + s) + 1);
                    y0 = y * ((t0 = t0 / d - 1) * t0 * ((s + 1) * t0 + s) + 1);
                    break;

                case Ease.BounceIn:
                    s = 1.70158f;
                    x1 = x * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1);
                    y1 = y * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1);
                    x0 = x * ((t0 = t0 / d - 1) * t0 * ((s + 1) * t0 + s) + 1);
                    y0 = y * ((t0 = t0 / d - 1) * t0 * ((s + 1) * t0 + s) + 1);
                    break;

                case Ease.BounceOut:
                    if ((t /= d) < (1f / 2.75f))
                    {
                        x1 = x * (7.5625f * t * t);
                        y1 = y * (7.5625f * t * t);
                    }
                    else if (t < (2f / 2.75f))
                    {
                        x1 = x * (7.5625f * (t -= (1.5f / 2.75f)) * t + .75f);
                        y1 = y * (7.5625f * (t -= (1.5f / 2.75f)) * t + .75f);
                    }
                    else if (t < (2.5f / 2.75f))
                    {
                        x1 = x * (7.5625f * (t -= (2.25f / 2.75f)) * t + .9375f);
                        y1 = y * (7.5625f * (t -= (2.25f / 2.75f)) * t + .9375f);
                    }
                    else
                    {
                        x1 = x * (7.5625f * (t -= (2.625f / 2.75f)) * t + .984375f);
                        y1 = y * (7.5625f * (t -= (2.625f / 2.75f)) * t + .984375f);
                    }

                    if ((t0 /= d) < (1f / 2.75f))
                    {
                        x0 = x * (7.5625f * t0 * t0);
                        y0 = y * (7.5625f * t0 * t0);
                    }
                    else if (t0 < (2f / 2.75f))
                    {
                        x0 = x * (7.5625f * (t0 -= (1.5f / 2.75f)) * t0 + .75f);
                        y0 = y * (7.5625f * (t0 -= (1.5f / 2.75f)) * t0 + .75f);
                    }
                    else if (t0 < (2.5f / 2.75f))
                    {
                        x0 = x * (7.5625f * (t0 -= (2.25f / 2.75f)) * t0 + .9375f);
                        y0 = y * (7.5625f * (t0 -= (2.25f / 2.75f)) * t0 + .9375f);
                    }
                    else
                    {
                        x0 = x * (7.5625f * (t0 -= (2.625f / 2.75f)) * t0 + .984375f);
                        y0 = y * (7.5625f * (t0 -= (2.625f / 2.75f)) * t0 + .984375f);
                    }

                    x1 /= 10;
                    y1 /= 10;
                    x0 /= 10;
                    y0 /= 10;

                    break;

                default:
                    break;
            }

            xPos = x1 - x0;
            yPos = y1 - y0;
            zPos = z1 - z0;

            //group.transform.position = new Vector2(group.transform.position.x + xPos, group.transform.position.y + yPos);
            group.transform.Rotate(Vector3.left, group.transform.rotation.x + xPos);
            group.transform.Rotate(Vector3.up, group.transform.rotation.y + yPos);
            group.transform.Rotate(Vector3.back, group.transform.rotation.eulerAngles.z + zPos);
            t0 = time;
            time += Time.deltaTime;
            yield return null;
        }

        xPos = x - x1;
        yPos = y - y1;
        zPos = z1 - z0;

        group.transform.Rotate(Vector3.left, group.transform.rotation.x + xPos);
        group.transform.Rotate(Vector3.up, group.transform.rotation.y + yPos);
        group.transform.Rotate(Vector3.back, group.transform.rotation.z + zPos);

        finished = true;
        inuse = false;
    }

    float cubic_bezier(float t, float u0, float u1, float u2, float u3)
    {
        Vector2 p0 = Vector2.zero;
        Vector2 p3 = Vector2.one;

        Vector2 p1 = new Vector2(u0, u1);
        Vector2 p2 = new Vector2(u2, u3);

        Vector2 p;

        p = (p0 * Mathf.Pow((1 - t), 3)) +
            (3 * p1 * Mathf.Pow((1 - t), 2) * t) +
            (3 * p2 * Mathf.Pow(t, 2) * (1 - t)) +
            (p3 * Mathf.Pow(t, 3));

        /*
        t = p.x;

        p = (p0 * Mathf.Pow((1 - t), 3)) +
            (3 * p1 * Mathf.Pow((1 - t), 2) * t) +
            (3 * p2 * Mathf.Pow(t, 2) * (1 - t)) +
            (p3 * Mathf.Pow(t, 3));*/

        return p.y;
    }

    public void Reset()
    {
        group.transform.position = original_position;
        finished = true;
        inuse = false;
    }

    public float getDuration()
    {
        return duration;
    }

    public bool getFinished()
    {
        return finished;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !inuse)
        {
            inuse = true;
            StartCoroutine(Move());

            Debug.Log("ROTATE");
        }
    }
}
