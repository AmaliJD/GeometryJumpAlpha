using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ProximityLighting : MonoBehaviour
{
    [Range(0.0f, 10.0f)]
    public float range;
    public float max_light_value;
    public float buffer;
    public float fadein, fadeout;
    public bool affect_sprite;

    Light2D light;
    GameObject player;
    SpriteRenderer sprite;
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        light = gameObject.GetComponent<Light2D>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float distance = Vector2.Distance(player.transform.position, transform.position);
        float intensity = (range - distance + buffer) / (range / max_light_value);
        float alpha = (range - distance + buffer) / (range / 1);

        if (intensity < 0) { intensity = 0; alpha = 0; }
        //if (alpha < 0) {  }

        if(intensity >= light.intensity)
        {
            light.intensity = Mathf.Lerp(light.intensity, intensity, fadein);
            if (affect_sprite)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b,
                               Mathf.Lerp(sprite.color.a, alpha, fadein));
            }
        }
        else
        {
            light.intensity = Mathf.Lerp(light.intensity, intensity, fadeout);
            if (affect_sprite)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b,
                               Mathf.Lerp(sprite.color.a, alpha, fadeout));
            }
        }
    }
}