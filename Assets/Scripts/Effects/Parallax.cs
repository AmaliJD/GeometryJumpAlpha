using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startpos, ypos, cam_y, cam_y0;
    public GameObject cam;
    public float parallexEffect;
    public bool center;

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

        transform.position = new Vector3((center ? 0 : startpos) + dist_x, transform.position.y - (cam_y - cam_y0), transform.position.z);
        /*
        float cam_r = cam.transform.rotation.eulerAngles.z;
        if(cam_r > 180) { cam_r = cam_r - 360; }

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, -cam_r/2));
        Debug.Log("cam rotation: " + cam.transform.rotation.eulerAngles.z);*/

        if (center) { return; }
        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }//*/

    /*void Start()
    {
        startpos = transform.position.x;
        //ypos = transform.position.y;
        cam_y = cam.transform.position.y;
        cam_y0 = cam_y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    float cam_x;
    void Update()
    {
        float dist_x = cam.transform.position.x - cam_x;

        transform.position = new Vector2(cam.transform.position.x, transform.position.y);

        cam_x = cam.transform.position.x;
    }//*/
}

