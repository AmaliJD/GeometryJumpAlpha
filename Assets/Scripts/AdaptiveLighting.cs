using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class AdaptiveLighting : MonoBehaviour
{
    Light2D light;
    Light2D global;
    void Awake()
    {
        global = GameObject.FindGameObjectWithTag("Global Light").GetComponent<Light2D>();
        light = gameObject.GetComponent<Light2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(1 - global.intensity >= 0)
        {
            light.intensity = 1 - global.intensity;
        }
        else
        {
            light.intensity = 0;
        }
    }
}
