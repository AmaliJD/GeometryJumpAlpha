using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;
using Cinemachine.PostFX;
using UnityEngine.Experimental.Rendering.Universal;


public class GameManager : MonoBehaviour
{
    public Text Mana;
    public Text Timer;
    public Text DevTools;
    public GameObject Pause_Menu;
    public GameObject Menu1;
    public GameObject Menu2;
    public GameObject WindowedResolution;

    public Button Menu_Button;
    public Button Restart_Button;
    public Button Options_Button;
    public Button Fullscreen_Button;
    public Button Effects_Button;
    public Button Res1440_Button;
    public Button Res1080_Button;
    public Button Res720_Button;

    public CinemachineBrain main_camera_brain;
    private float aspectratio = 16f / 9f;
    private int prev_width, prev_height;

    public ColorReference[] color_channels;
    private Color[] channel_colors;

    private float time = 0;
    private bool shortcuts_enabled = false, game = false, paused = false;

    private GameObject effects;
    private GameObject globallight;
    private GameObject playerlight;
    private GameObject player;
    public AudioSource bgmusic;
    private AudioSource newbgmusic;

    private PlayerController playercontroller;
    private CubeController cubecontroller;
    private AutoController autocontroller;

    private AutoShipController autoshipcontroller;
    private ShipController shipcontroller;

    private UfoController ufocontroller;
    private AutoUfoController autoufocontroller;

    private WaveController wavecontroller;
    private AutoWaveController autowavecontroller;

    private BallController ballcontroller;
    private AutoBallController autoballcontroller;

    private SpiderController spidercontroller;
    private AutoSpiderController autospidercontroller;

    private Checkpoint_Controller checkpointcontroller;

    private IconController iconcontroller;
    private GameObject icon;

    private int mana_count = 0;
    private int diamond_count = 0;

    private int min = 0, sec = 0, milli = 0, m = 0, s = 0;

    private bool postfxon = true;

    private float deltaTime = 0.0f;

    private GameObject[] initialList;
    private List<CinemachineVirtualCamera> cameraList;
    CinemachineVirtualCamera activeCamera;

    //PLAYROOM
    public SpriteRenderer Background;

    private void Awake()
    {
        Resources.UnloadUnusedAssets();
        Screen.SetResolution(Screen.resolutions[Screen.resolutions.Length - 1].width, Screen.resolutions[Screen.resolutions.Length - 1].height, true);
        Screen.fullScreen = true;
        prev_width = Screen.width;
        prev_height = Screen.height;

        Restart_Button.onClick.AddListener(Restart);
        Options_Button.onClick.AddListener(Options);
        Menu_Button.onClick.AddListener(ReturnToMenu);
        Fullscreen_Button.onClick.AddListener(ToggleFullscreen);
        Effects_Button.onClick.AddListener(TogglePostProcessing);

        Res1440_Button.onClick.AddListener(() => { SetResolution(1440); });
        Res1080_Button.onClick.AddListener(() => { SetResolution(1080); });
        Res720_Button.onClick.AddListener(() => { SetResolution(720); });

        player = GameObject.Find("Player");
        playerlight = GameObject.Find("Player Light Bright"); playerlight.SetActive(false);
        effects = GameObject.Find("EFFECTS");
        globallight = GameObject.Find("Global Light");

        DevTools.color = Color.clear;
        cubecontroller = player.GetComponent<CubeController>();
        autocontroller = player.GetComponent<AutoController>();

        shipcontroller = player.GetComponent<ShipController>();
        autoshipcontroller = player.GetComponent<AutoShipController>();

        ufocontroller = player.GetComponent<UfoController>();
        autoufocontroller = player.GetComponent<AutoUfoController>();

        wavecontroller = player.GetComponent<WaveController>();
        autowavecontroller = player.GetComponent<AutoWaveController>();

        ballcontroller = player.GetComponent<BallController>();
        autoballcontroller = player.GetComponent<AutoBallController>();

        spidercontroller = player.GetComponent<SpiderController>();
        autospidercontroller = player.GetComponent<AutoSpiderController>();

        //------------------------------------------------------------------------------------------------
        playercontroller = cubecontroller;
        checkpointcontroller = FindObjectOfType<Checkpoint_Controller>();
        playercontroller.setBGMusic(bgmusic);
        newbgmusic = bgmusic;

        iconcontroller = FindObjectOfType<IconController>();
        icon = iconcontroller.getIcon();

        playercontroller.setIcons(icon);

        // ------------------------------------------------------------------------------------------------
        channel_colors = new Color[color_channels.Length];
        int i = 0;
        foreach(ColorReference c in color_channels)
        {
            channel_colors[i] = c.channelcolor;
            if(c.refer != null) { channel_colors[i] = c.refer.channelcolor; }
            i++;
        }

        // camera list ------------------------
        cameraList = new List<CinemachineVirtualCamera>();
        initialList = GameObject.FindGameObjectsWithTag("Camera");

        i = 0;
        foreach (GameObject g in initialList)
        {
            cameraList.Add(g.GetComponent<CinemachineVirtualCamera>());
            cameraList[i].gameObject.SetActive(true);
            cameraList[i].Priority = 5;
        }
    }

    public List<CinemachineVirtualCamera> getCameraList()
    {
        return cameraList;
    }

    public CinemachineVirtualCamera getActiveCamera()
    {
        foreach (CinemachineVirtualCamera c in cameraList)
        {
            if (c.Priority == 10) { activeCamera = c; break; }
        }

        return activeCamera;
    }

    public Transform getPlayerTransform()
    {
        return player.transform;
    }

    void resetColorChannels()
    {
        int i = 0;
        foreach (ColorReference c in color_channels)
        {
            c.Set(channel_colors[i]);
            i++;
        }
    }

    // Button Functions
    public void Restart()
    {
        playercontroller.resetStaticVariables();
        resetColorChannels();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1.0f;
    }
    public void Options()
    {
        Menu1.SetActive(false);
        Menu2.SetActive(true);
    }
    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
    public void SetResolution(int width)
    {
        switch(width)
        {
            case 1440:
                Screen.SetResolution(2560, 1440, true); Screen.fullScreen = false; break;
            case 1080:
                Screen.SetResolution(1920, 1080, true); Screen.fullScreen = false; break;
            case 720:
                Screen.SetResolution(1280, 720, true); Screen.fullScreen = false; break;
            default:
                break;
        }
        
    }
    public void TogglePostProcessing()
    {
        effects.SetActive(!effects.activeSelf);
        Debug.Log(main_camera_brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVolumeSettings>() == null);
        postfxon = !postfxon;
        if (postfxon && main_camera_brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVolumeSettings>() != null)
        {
            main_camera_brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVolumeSettings>().enabled = true;
        }
        else
        {
            main_camera_brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVolumeSettings>().enabled = false;
        }
    }
    public void ReturnToMenu()
    {
        //playercontroller.resetStaticVariables();
        //resetColorChannels();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            resetColorChannels();
            Application.Quit();
        }
        if (Input.GetKeyDown("f"))
        {
            if (!Screen.fullScreen)
            {
                Screen.SetResolution(Screen.resolutions[Screen.resolutions.Length - 1].width, Screen.resolutions[Screen.resolutions.Length - 1].height, true);
            }
            else
            {
                Screen.fullScreen = !Screen.fullScreen;
            }
        }
        if (Input.GetKeyDown("r"))
        {
            Restart();
        }

        /*if(!Input.GetMouseButton(0))
        {
            if (prev_height != Screen.height && !Screen.fullScreen)
            {
                int remainder = Screen.height % 9;
                int newHeight = Screen.height - remainder;

                Screen.SetResolution((int)(newHeight * aspectratio), newHeight, true);
                Screen.fullScreen = false;

                prev_height = newHeight;
                prev_width = Screen.width;
            }
            else if (prev_width != Screen.width && !Screen.fullScreen)
            {
                int remainder = Screen.width % 16;
                int newWidth = Screen.width - remainder;

                Screen.SetResolution(newWidth, (int)(newWidth * (1 / aspectratio)), true);
                Screen.fullScreen = false;

                prev_height = Screen.height;
                prev_width = newWidth;
            }
        }*/

        // PLAYROOM
        if (Input.GetKeyDown("c"))
        {
            playercontroller.Respawn();
        }
        if (Input.GetKeyDown("5"))
        {
            playercontroller.setSpeed(0);
            playercontroller.playSpeedParticles(0);
        }
        if (Input.GetKeyDown("1"))
        {
            playercontroller.setSpeed(1);
            playercontroller.playSpeedParticles(1);
        }
        if (Input.GetKeyDown("2"))
        {
            playercontroller.setSpeed(2);
            playercontroller.playSpeedParticles(2);
        }
        if (Input.GetKeyDown("3"))
        {
            playercontroller.setSpeed(3);
            playercontroller.playSpeedParticles(3);
        }
        if (Input.GetKeyDown("4"))
        {
            playercontroller.setSpeed(4);
            playercontroller.playSpeedParticles(4);
        }
        if (Input.GetKeyDown("m"))
        {
            playercontroller.setMini(!playercontroller.getMini());
        }
        if (Input.GetKeyDown("t"))
        {
            if (Time.timeScale == 1.0f)
                Time.timeScale = .3f;
            else
                Time.timeScale = 1.0f;
        }
        if (Input.GetKey(KeyCode.Backspace))
        {
            float h, s, v;
            Color.RGBToHSV(Background.color, out h, out s, out v);

            Background.color = Color.HSVToRGB(h+.01f, s, v);
        }


        if (Input.GetKeyDown("escape"))
        {
            if (!paused)
            {
                paused = !paused;
                Pause_Menu.SetActive(true);
                bgmusic.Pause();
                Time.timeScale = 0;
            }
            else if(paused)
            {
                if(Menu1.activeSelf)
                {
                    paused = !paused;
                    Pause_Menu.SetActive(false);
                    Time.timeScale = 1;
                    bgmusic.Play();
                }
                else
                {
                    Menu1.SetActive(true);
                    Menu2.SetActive(false);
                }
            }
        }
        /*
        if (shortcuts_enabled)
        {
            if (Input.GetKeyDown("p"))
            {
                effects.SetActive(!effects.activeSelf);
                Debug.Log(main_camera_brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVolumeSettings>() == null);
                postfxon = !postfxon;
                if (postfxon && main_camera_brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVolumeSettings>() != null)
                {
                    main_camera_brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVolumeSettings>().enabled = true;
                }
                else
                {
                    main_camera_brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVolumeSettings>().enabled = false;
                }
            }

            if (Input.GetKeyDown("l"))
            {
                globallight.SetActive(!globallight.activeSelf);
                playerlight.SetActive(!globallight.activeSelf);
            }

            if (Input.GetKeyDown("t"))
            {
                if (Time.timeScale == 1.0f)
                    Time.timeScale = .3f;
                else
                    Time.timeScale = 1.0f;
            }
        }

        if (Input.GetKeyDown("o"))
        {
            if(DevTools.color == Color.clear)
            {
                DevTools.color = Color.white;
                shortcuts_enabled = true;
            }
            else
            {
                DevTools.color = Color.clear;
                shortcuts_enabled = false;
            }
        }//*/


        if (!game)
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

            milli = (int)(time * 1000) % 1000;
            sec = (int)(time);
            min = (int)(sec / 60);

            milli -= (1000 * m);
            sec -= (60 * s);

            if (milli == 1000) { m++; };
            if (sec == 60) { s++; };

            Mana.text = "Mana: " + mana_count;
            Timer.text = "Time: " + min + " : " + sec + " : " + milli + "\nFPS: " + Mathf.RoundToInt((1/deltaTime)) + "\nTime Scale: " + Time.timeScale;

            time += Time.deltaTime;
        }

        /*
        if(postfxon)
        {
            //bool volume_enabled = main_camera_brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVolumeSettings>().enabled;
            main_camera_brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVolumeSettings>().enabled = true;
        }
        else
        {
            main_camera_brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVolumeSettings>().enabled = false;
        }*/

        prev_height = Screen.height;
        prev_width = Screen.width;
    }

    public PlayerController getController()
    {
        if (cubecontroller.isActiveAndEnabled)
        {
            playercontroller = cubecontroller;
            return playercontroller;
        }
        else if (autocontroller.isActiveAndEnabled)
        {
            playercontroller = autocontroller;
            return playercontroller;
        }
        else if (shipcontroller.isActiveAndEnabled)
        {
            playercontroller = shipcontroller;
            return playercontroller;
        }
        else if (autoshipcontroller.isActiveAndEnabled)
        {
            playercontroller = autoshipcontroller;
            return playercontroller;
        }
        else if (ufocontroller.isActiveAndEnabled)
        {
            playercontroller = ufocontroller;
            return playercontroller;
        }
        else if (autoufocontroller.isActiveAndEnabled)
        {
            playercontroller = autoufocontroller;
            return playercontroller;
        }
        else if (wavecontroller.isActiveAndEnabled)
        {
            playercontroller = wavecontroller;
            return playercontroller;
        }
        else if (autowavecontroller.isActiveAndEnabled)
        {
            playercontroller = autowavecontroller;
            return playercontroller;
        }
        else if (ballcontroller.isActiveAndEnabled)
        {
            playercontroller = ballcontroller;
            return playercontroller;
        }
        else if (autoballcontroller.isActiveAndEnabled)
        {
            playercontroller = autoballcontroller;
            return playercontroller;
        }
        else if (spidercontroller.isActiveAndEnabled)
        {
            playercontroller = spidercontroller;
            return playercontroller;
        }
        else if (autospidercontroller.isActiveAndEnabled)
        {
            playercontroller = autospidercontroller;
            return playercontroller;
        }
        else
        {
            return playercontroller;
        }
    }

    public void setPlayerController(string mode, bool startmusic, bool restartmusic)
    {
        //bool reversed = playercontroller.getReversed();
        //bool mini = playercontroller.getMini();
        //float speed;

        if(mode.Equals(playercontroller.getMode()))
        {
            //Debug.Log("Equals");
            if (bgmusic != newbgmusic)
            {
                bgmusic.Stop();
                bgmusic = newbgmusic;
            }
            else
            {
                startmusic = false;
            }

            return;
        }

        if (mode != playercontroller.getMode())/*
            || (mode.Equals("cube") && playercontroller != cubecontroller)
            || (mode.Equals("auto") && playercontroller != autocontroller)
            || (mode.Equals("ship") && playercontroller != shipcontroller)
            || (mode.Equals("auto_ship") && playercontroller != autoshipcontroller)
            || (mode.Equals("ufo") && playercontroller != ufocontroller)
            || (mode.Equals("auto_ufo") && playercontroller != autoufocontroller)
            || (mode.Equals("wave") && playercontroller != wavecontroller)
            || (mode.Equals("auto_wave") && playercontroller != autowavecontroller)
            || (mode.Equals("ball") && playercontroller != ballcontroller)
            || (mode.Equals("auto_ball") && playercontroller != autoballcontroller)
            || (mode.Equals("spider") && playercontroller != spidercontroller)
            || (mode.Equals("auto_spider") && playercontroller != autospidercontroller))*/
        {
            //Debug.Log("curr: " + playercontroller.getMode() + "    new: " + mode);
            if (bgmusic != newbgmusic)
            {
                bgmusic.Stop();
                bgmusic = newbgmusic;
            }

            playercontroller.enabled = false;
            playercontroller.setAble(false);
            //reversed = playercontroller.getReversed();
            //speed = playercontroller.getSpeed();
            //mini = playercontroller.getMini();
            playercontroller.resetColliders();
            //playercontroller.setVariables(false, false, false);

            if (playercontroller.isDead())
            {
                //reversed = checkpointcontroller.getReversed();
                //speed = checkpointcontroller.getSpeed();
            }

            if (mode.Equals("cube")) { playercontroller = cubecontroller; }
            else if (mode.Equals("auto")) { playercontroller = autocontroller; }
            else if (mode.Equals("ship")) { playercontroller = shipcontroller; }
            else if (mode.Equals("auto_ship")) { playercontroller = autoshipcontroller; }
            else if (mode.Equals("ufo")) { playercontroller = ufocontroller; }
            else if (mode.Equals("auto_ufo")) { playercontroller = autoufocontroller; }
            else if (mode.Equals("wave")) { playercontroller = wavecontroller; }
            else if (mode.Equals("auto_wave")) { playercontroller = autowavecontroller; }
            else if (mode.Equals("ball")) { playercontroller = ballcontroller; }
            else if (mode.Equals("auto_ball")) { playercontroller = autoballcontroller; }
            else if (mode.Equals("spider")) { playercontroller = spidercontroller; }
            else if (mode.Equals("auto_spider")) { playercontroller = autospidercontroller; }

            playercontroller.setAble(false);
            playercontroller.setColliders();
            playercontroller.setIcons(icon);
            playercontroller.setBGMusic(bgmusic);
            //playercontroller.setSpeed(speed);
            playercontroller.setRestartMusicOnDeath(restartmusic);

            if (startmusic)
            {
                bgmusic.Play();
                bgmusic.volume = 1;
            }
            /*
            if (checkpointcontroller.getIndex() != -1)
            {
                playercontroller.setRespawn(checkpointcontroller.getTransform(), checkpointcontroller.getReversed(), checkpointcontroller.getMini());
                playercontroller.setRepawnSpeed(checkpointcontroller.getSpeed());
            }*/


            //playercontroller.setVariables((Input.GetButton("Jump") || Input.GetKey("space")), reversed, mini);
            playercontroller.setAnimation();
            playercontroller.setAble(true);
            playercontroller.enabled = true;

            return;
        }
    }

    public void playBGMusic(float playvolume)
    {
        if (bgmusic != newbgmusic)
        {
            bgmusic.Stop();
            bgmusic = newbgmusic;
        }

        playercontroller.setBGMusic(bgmusic);

        bgmusic.volume = playvolume;
        bgmusic.Play();
    }

    public void setBGMusic(AudioSource audio)
    {
        newbgmusic = audio;
    }

    public AudioSource getBGMusic()
    {
        return bgmusic;
    }

    public void incrementManaCount(int amt)
    {
        mana_count += amt;
    }

    public int getManaCount()
    {
        return mana_count;
    }

    public void incrementDiamondCount(int amt)
    {
        diamond_count += amt;
    }

    public int getDiamondCount()
    {
        return diamond_count;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            game = true;
        }
    }

    public bool shorten()
    {
        return shortcuts_enabled;
    }

    public bool getpostfx()
    {
        return postfxon;
    }
}
