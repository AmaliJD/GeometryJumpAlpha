using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    public enum Mode { music, volume }
    public Mode mode;

    private GameManager gamemanager;
    public AudioSource bgmusic;
    public float volume, fadetime;
    public bool oneuse;
    private bool omaewamou = false;
    // Start is called before the first frame update
    void Awake()
    {
        gamemanager = FindObjectOfType<GameManager>();
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
    private void setBGMusic()
    {
        gamemanager.setBGMusic(bgmusic);
        if (oneuse)
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator setMusicVolume()
    {
        AudioSource bgMusic = gamemanager.getBGMusic();

        if (fadetime == 0) { bgMusic.volume = volume; }
        else
        {
            float time = 0;
            float step = (bgMusic.volume - volume) / (fadetime / Time.deltaTime);

            while (time < fadetime)
            {
                bgMusic.volume = bgMusic.volume - step;

                time += Time.deltaTime;
                yield return null;
            }
        }

        bgMusic.volume = volume;

        if (oneuse)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !omaewamou)
        {
            if (oneuse) { omaewamou = true; }
            if(mode.ToString() == "music") { setBGMusic(); }
            else if (mode.ToString() == "volume") { StartCoroutine(setMusicVolume()); }
        }
    }
}
