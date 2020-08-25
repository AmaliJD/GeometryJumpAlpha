using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startpos, ypos, cam_y, cam_y0;
    public GameObject cam;
    public float parallexEffect;
    void Start()
    {
        startpos = transform.position.x;
        //ypos = transform.position.y;
        cam_y = cam.transform.position.y;
        cam_y0 = cam_y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    void Update()
    {
        cam_y0 = cam_y;
        cam_y = cam.transform.position.y;

        float temp = (cam.transform.position.x * (1 - parallexEffect));
        float dist_x = (cam.transform.position.x * parallexEffect);

        transform.position = new Vector3(startpos + dist_x, transform.position.y - (cam_y - cam_y0), transform.position.z);

        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }
}

