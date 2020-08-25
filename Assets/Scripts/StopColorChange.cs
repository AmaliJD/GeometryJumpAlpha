using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopColorChange : MonoBehaviour
{
    private List<string> channel_ids;
    private List<GameObject> active_trigger;

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if(!everybodygetsone)
            {
                active_trigger.SendMessage("StopPulse");
            }
            everybodygetsone = false;
        }
    }*/

    private void Awake()
    {
        channel_ids = new List<string>();
        active_trigger = new List<GameObject>();
    }

    public void Send(string id)
    {
        int index = channel_ids.IndexOf(id);

        if(index != -1)
        {
            active_trigger[index].BroadcastMessage("Stop");
        }
    }

    public void setActiveTrigger(GameObject g, string id)
    {
        if(channel_ids.IndexOf(id) == -1)
        {
            channel_ids.Add(id);
            active_trigger.Add(g);
        }
        else
        {
            int index = channel_ids.IndexOf(id);
            channel_ids[index] = id;
            active_trigger[index] = g;
        }
        
    }
}
