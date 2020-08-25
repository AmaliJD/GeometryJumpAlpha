using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Checkpoint : MonoBehaviour
{
    public GameObject gray;
    public GameObject green;
    public GameObject active;

    public bool reversed = false, mini = false;
    public enum Speed { x0, x1, x2, x3, x4 }
    public Speed speed;

    private PlayerController player;
    private GameManager gamemanager;
    private Checkpoint_Controller check;

    private int state = 0, index = -1;

    // Start is called before the first frame update
    void Awake()
    {
        gamemanager = FindObjectOfType<GameManager>();
        player = gamemanager.getController();
        check = FindObjectOfType<Checkpoint_Controller>();

        gray.SetActive(true);
        green.SetActive(false);
        active.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            gray.SetActive(false);
            green.SetActive(false);
            active.SetActive(true);
            state = 2;

            check.updateStates(index);
            player = gamemanager.getController();
            player.setRespawn(transform.position, reversed, mini);
            player.setRepawnSpeed(Convert.ToInt32(speed.ToString().Substring(1)));
            //Debug.Log(Convert.ToInt32(speed.ToString().Substring(1)));
        }
    }

    public void Green()
    {
        gray.SetActive(false);
        active.SetActive(false);
        green.SetActive(true);
        state = 1;
    }

    public int getState()
    {
        return state;
    }

    public void setIndex(int i)
    {
        index = i;
    }
}
