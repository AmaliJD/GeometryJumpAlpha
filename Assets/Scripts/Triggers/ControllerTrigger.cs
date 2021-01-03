using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ControllerTrigger : MonoBehaviour
{
    public enum Mode { cube, auto, ship, auto_ship, ufo, auto_ufo, wave, auto_wave, ball, auto_ball, spider, auto_spider, copter, auto_copter }
    public Mode mode;

    private GameManager gamemanager;
    private bool inuse = false;
    public bool oneuse = false;
    public bool show_portal = true;
    public bool startmusic = false;
    public bool restartmusic = false;

    public GameObject texture, cube_portal, ship_portal, ufo_portal, wave_portal, ball_portal, spider_portal, copter_portal;

    // Start is called before the first frame update
    void Awake()
    {
        gamemanager = GameObject.FindObjectOfType<GameManager>();

        texture.SetActive(false);

        if (show_portal)
        {
            if (mode.ToString().Equals("cube") || mode.ToString().Equals("auto")) { cube_portal.SetActive(true); }
            else if (mode.ToString().Equals("ship") || mode.ToString().Equals("auto_ship")) { ship_portal.SetActive(true); }
            else if (mode.ToString().Equals("ufo") || mode.ToString().Equals("auto_ufo")) { ufo_portal.SetActive(true); }
            else if (mode.ToString().Equals("wave") || mode.ToString().Equals("auto_wave")) { wave_portal.SetActive(true); }
            else if (mode.ToString().Equals("ball") || mode.ToString().Equals("auto_ball")) { ball_portal.SetActive(true); }
            else if (mode.ToString().Equals("spider") || mode.ToString().Equals("auto_spider")) { spider_portal.SetActive(true); }
            else if (mode.ToString().Equals("copter") || mode.ToString().Equals("auto_copter")) { copter_portal.SetActive(true); }
        }
    }

    private void changeGamemode()
    {
        gamemanager.setPlayerController(mode.ToString(), startmusic, restartmusic);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !inuse)
        {
            inuse = true;
            changeGamemode();
            inuse = false;

            if (oneuse)
            {
                Destroy(gameObject);
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (show_portal)
        {
            if (mode.ToString().Equals("cube") || mode.ToString().Equals("auto"))
            {
                cube_portal.SetActive(true);
                ship_portal.SetActive(false);
                ufo_portal.SetActive(false);
                wave_portal.SetActive(false);
                ball_portal.SetActive(false);
                spider_portal.SetActive(false);
                copter_portal.SetActive(false);
            }
            else if (mode.ToString().Equals("ship") || mode.ToString().Equals("auto_ship"))
            {
                ship_portal.SetActive(true);
                cube_portal.SetActive(false);
                ufo_portal.SetActive(false);
                wave_portal.SetActive(false);
                ball_portal.SetActive(false);
                spider_portal.SetActive(false);
                copter_portal.SetActive(false);
            }
            else if (mode.ToString().Equals("ufo") || mode.ToString().Equals("auto_ufo"))
            {
                ufo_portal.SetActive(true);
                cube_portal.SetActive(false);
                ship_portal.SetActive(false);
                wave_portal.SetActive(false);
                ball_portal.SetActive(false);
                spider_portal.SetActive(false);
                copter_portal.SetActive(false);
            }
            else if (mode.ToString().Equals("wave") || mode.ToString().Equals("auto_wave"))
            {
                wave_portal.SetActive(true);
                cube_portal.SetActive(false);
                ship_portal.SetActive(false);
                ufo_portal.SetActive(false);
                ball_portal.SetActive(false);
                spider_portal.SetActive(false);
                copter_portal.SetActive(false);
            }
            else if (mode.ToString().Equals("ball") || mode.ToString().Equals("auto_ball"))
            {
                ball_portal.SetActive(true);
                cube_portal.SetActive(false);
                ship_portal.SetActive(false);
                ufo_portal.SetActive(false);
                wave_portal.SetActive(false);
                spider_portal.SetActive(false);
                copter_portal.SetActive(false);
            }
            else if (mode.ToString().Equals("spider") || mode.ToString().Equals("auto_spider"))
            {
                spider_portal.SetActive(true);
                cube_portal.SetActive(false);
                ship_portal.SetActive(false);
                ufo_portal.SetActive(false);
                wave_portal.SetActive(false);
                ball_portal.SetActive(false);
                copter_portal.SetActive(false);
            }
            else if (mode.ToString().Equals("copter") || mode.ToString().Equals("auto_copter"))
            {
                copter_portal.SetActive(true);
                spider_portal.SetActive(false);
                cube_portal.SetActive(false);
                ship_portal.SetActive(false);
                ufo_portal.SetActive(false);
                wave_portal.SetActive(false);
                ball_portal.SetActive(false);
            }
        }
        else
        {
            spider_portal.SetActive(false);
            cube_portal.SetActive(false);
            ship_portal.SetActive(false);
            ufo_portal.SetActive(false);
            wave_portal.SetActive(false);
            ball_portal.SetActive(false);
            copter_portal.SetActive(false);
        }
    }
#endif
}
