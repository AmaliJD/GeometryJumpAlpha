using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cinemachine.PostFX;

public class CameraSwitch : MonoBehaviour
{
    public CinemachineVirtualCamera activeCamera;
    private List<CinemachineVirtualCamera> cameraList;
    private GameObject[] initialList;
    private GameManager gamemanager;

    private void Awake()
    {
        gamemanager = FindObjectOfType<GameManager>();
        cameraList = new List<CinemachineVirtualCamera>();
        initialList = GameObject.FindGameObjectsWithTag("Camera");

        int i = 0;
        foreach (GameObject g in initialList)
        {
            cameraList.Add(g.GetComponent<CinemachineVirtualCamera>());
            cameraList[i].gameObject.SetActive(true);
            cameraList[i].Priority = 5;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (gamemanager.getpostfx())
            {
                if (activeCamera.VirtualCameraGameObject.GetComponent<CinemachineVolumeSettings>() != null)
                {
                    activeCamera.VirtualCameraGameObject.GetComponent<CinemachineVolumeSettings>().enabled = true;
                }
            }
            else
            {
                if (activeCamera.VirtualCameraGameObject.GetComponent<CinemachineVolumeSettings>() != null)
                {
                    activeCamera.VirtualCameraGameObject.GetComponent<CinemachineVolumeSettings>().enabled = false;
                }
            }

            foreach (CinemachineVirtualCamera c in cameraList)
            {
                c.Priority = 5;
                //if (c != activeCamera) { c.VirtualCameraGameObject.GetComponent<CinemachineVolumeSettings>().enabled = true; }
            }

            activeCamera.Priority = 10;
        }
    }
}
