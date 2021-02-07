using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTrigger : MonoBehaviour
{
    public int activeCount;
    public bool childTrigger;
    public bool toggleMode;
    public bool oneuse = true;
    GameManager gamemanager;

    public GameObject[] on_targets;
    public GameObject[] off_targets;
    private bool omaewamou = false;
    private bool finished = true;

    private void Awake()
    {
        gamemanager = GameObject.FindObjectOfType<GameManager>();
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void toggle()
    {
        finished = false;
        foreach(GameObject obj in on_targets)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in off_targets)
        {
            obj.SetActive(false);
        }
        finished = true;
    }

    public IEnumerator Toggle()
    {
        finished = false;

        foreach (GameObject obj in on_targets)
        {
            if(!childTrigger)
            {
                obj.SetActive(true);
                yield return null;
            }
            else
            {
                List<GameObject> childlist = new List<GameObject>();
                foreach (Transform child in obj.transform)
                {
                    childlist.Add(child.gameObject);
                    //yield return null;
                }
                foreach (GameObject child in childlist)
                {
                    child.SetActive(true);
                    yield return null;
                }
                /*foreach (Transform child in obj.transform)
                {
                    child.gameObject.SetActive(true);
                    yield return null;
                }*/
            }
        }
        foreach (GameObject obj in off_targets)
        {
            obj.SetActive(false);
            yield return null;
        }

        if(toggleMode)
        {
            /*GameObject[] temp_on = off_targets;
            GameObject[] temp_off = on_targets;

            off_targets = new GameObject[temp_off.Length];
            on_targets = new GameObject[temp_on.Length];

            off_targets = temp_off;
            on_targets = temp_on;*/

            GameObject[] temp = off_targets;
            off_targets = on_targets;
            on_targets = temp;
        }

        finished = true;

        if(oneuse)
        {
            Destroy(gameObject);
        }

        omaewamou = false;
    }

    public bool getFinished()
    {
        return finished;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && omaewamou == false && gamemanager.getManaCount() >= activeCount)
        {
            omaewamou = true;
            StartCoroutine(Toggle());//toggle();
            gamemanager.incrementManaCount(-activeCount);
            //Destroy(gameObject);
        }
    }
}
