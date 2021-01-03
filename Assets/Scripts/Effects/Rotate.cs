using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float speedX, speedY, speedZ;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(Vector3.left, speedX * Time.deltaTime);
        transform.Rotate(Vector3.up, speedY * Time.deltaTime);
        transform.Rotate(Vector3.back, speedZ * Time.deltaTime);
    }
}
