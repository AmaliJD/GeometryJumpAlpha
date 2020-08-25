using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTrigger : MonoBehaviour
{
    public int activeCount;
    GameManager gamemanager;

    public GameObject[] on_targets;
    public GameObject[] off_targets;
    private bool omaewamou = false;

    private void Awake()
    {
        gamemanager = GameObject.FindObjectOfType<GameManager>();
    }

    private void toggle()
    {
        foreach(GameObject obj in on_targets)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in off_targets)
        {
            obj.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && omaewamou == false && gamemanager.getManaCount() >= activeCount)
        {
            omaewamou = true;
            toggle();
            gamemanager.incrementManaCount(-activeCount);
            Destroy(gameObject);
        }
    }
}
