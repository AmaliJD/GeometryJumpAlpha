using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    public GameObject[] triggers;
    public float[] delay;
    public bool loop;
    public bool TriggerOrg;

    private bool inuse = false, finished = false;

    private void Awake()
    {
        if (!TriggerOrg) { gameObject.transform.GetChild(0).gameObject.SetActive(false); }
    }

    public IEnumerator Begin()
    {
        int i = 0;

        while(i < triggers.Length)
        {
            float d1 = delay[i], time = 0, d2 = 0;
            GameObject trigger = triggers[i];

            while(time <= d1)
            {
                time += Time.deltaTime;
                yield return null;
            }

            switch (trigger.gameObject.tag)
            {
                // MOVE TRIGGER
                case "MoveTrigger":
                    MoveTrigger move = trigger.GetComponent<MoveTrigger>();
                    d2 = move.getDuration();
                    time = 0;

                    //Debug.Log("Trigger " +  i);
                    StartCoroutine(move.Move());

                    while (time <= d2)
                    {
                        time += Time.deltaTime;
                        yield return null;
                    }
                    while (move.getFinished() != true)
                    {
                        Debug.Log("Waiting to finish");
                        yield return null;
                    }

                    move.StopAllCoroutines();
                    break;

                // SPAWN TRIGGER
                case "SpawnTrigger":
                    SpawnTrigger spawn = trigger.GetComponent<SpawnTrigger>();
                    spawn.StopAllCoroutines();
                    StartCoroutine(spawn.Begin());
                    break;

                // MUSIC TRIGGER
                case "MusicTrigger":
                    MusicTrigger music = trigger.GetComponent<MusicTrigger>();
                    d2 = music.getDuration();
                    time = 0;

                    // Volume
                    if (music.mode == MusicTrigger.Mode.volume)
                    {
                        StartCoroutine(music.setMusicVolume());

                        while (music.getFinished() != true)
                        {
                            Debug.Log("Waiting to finish");
                            yield return null;
                        }
                    }

                    // Change Song
                    else if (music.mode == MusicTrigger.Mode.music)
                    {
                        music.setBGMusic();
                    }

                    music.StopAllCoroutines();
                    break;

                // TOGGLE TRIGGER
                case "ToggleTrigger":
                    ToggleTrigger toggle = trigger.GetComponent<ToggleTrigger>();
                    StartCoroutine(toggle.Toggle());
                    while (toggle.getFinished() != true)
                    {
                        Debug.Log("Waiting to finish");
                        yield return null;
                    }
                    break;

                default:
                    break;
            }

            /*
            // Move Trigger
            if(trigger.GetComponent<MoveTrigger>() != null)
            {
                MoveTrigger move = trigger.GetComponent<MoveTrigger>();
                float d2 = move.getDuration();
                time = 0;

                //Debug.Log("Trigger " +  i);
                StartCoroutine(move.Move());
                
                while (time <= d2)
                {
                    time += Time.deltaTime;
                    yield return null;
                }
                while (move.getFinished() != true)
                {
                    Debug.Log("Waiting to finish");
                    yield return null;
                }

                move.StopAllCoroutines();
            }

            // Spawn Trigger
            else if (trigger.GetComponent<SpawnTrigger>() != null)
            {
                SpawnTrigger spawn = trigger.GetComponent<SpawnTrigger>();

                StartCoroutine(spawn.Begin());
            }

            // Music
            else if (trigger.GetComponent<MusicTrigger>() != null)
            {
                MusicTrigger music = trigger.GetComponent<MusicTrigger>();
                float d2 = music.getDuration();
                time = 0;

                // Volume
                if(music.mode == MusicTrigger.Mode.volume)
                {
                    StartCoroutine(music.setMusicVolume());

                    while (music.getFinished() != true)
                    {
                        Debug.Log("Waiting to finish");
                        yield return null;
                    }
                }

                // Change Song
                else if (music.mode == MusicTrigger.Mode.music)
                {
                    music.setBGMusic();
                }

                music.StopAllCoroutines();
            }*/

            i++;

            if(i == triggers.Length && loop)
            {
                i = 0;
            }

            yield return null;
        }
    }

    public void Activate()
    {
        inuse = true;
        StartCoroutine(Begin());
    }

    public bool getFinished()
    {
        return finished;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !inuse && !TriggerOrg)
        {
            inuse = true;
            StartCoroutine(Begin());
        }
    }
}
