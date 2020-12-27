using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    public enum Mode { music, volume }
    public Mode mode;

    private GameManager gamemanager;

    public MusicSource bgmusic;

    public float volume, fadetime, playvolume = 1;
    public bool play;
    public bool oneuse;
    private bool omaewamou = false, finished = false;
    // Start is called before the first frame update
    void Awake()
    {
        gamemanager = FindObjectOfType<GameManager>();
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
    public void setBGMusic()
    {
        gamemanager.setBGMusic(bgmusic);

        if (play)
        {
            gamemanager.playBGMusic(playvolume);
        }

        finished = true;

        if (oneuse)
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator setMusicVolume()
    {
        MusicSource bgMusic = gamemanager.getBGMusic();

        if (fadetime == 0) { bgMusic.realVolume = volume; }
        else
        {
            float time = 0;
            float step = (bgMusic.realVolume - volume) / (fadetime / Time.deltaTime);

            while (time < fadetime)
            {
                bgMusic.realVolume = bgMusic.realVolume - step;

                time += Time.deltaTime;
                //time += Time.deltaTime / (fadetime * 100);
                yield return null;
            }
        }

        bgMusic.realVolume = volume;
        finished = true;

        if (oneuse)
        {
            Destroy(gameObject);
        }
    }

    public float getDuration()
    {
        return fadetime;
    }

    public bool getFinished()
    {
        return finished;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !omaewamou)
        {
            finished = false;
            if (oneuse) { omaewamou = true; }
            if(mode.ToString() == "music") { setBGMusic(); }
            else if (mode.ToString() == "volume") { StartCoroutine(setMusicVolume()); }
        }
    }
}
