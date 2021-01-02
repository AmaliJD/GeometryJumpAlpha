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
    public int levelNumber;

    public Text ManaCount;
    public Text DiamondCount;
    public Text Timer;
    public Text ManaScore;
    public Text DiamondScore;
    public Text TimeScore;
    public Text FPS;
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

    public Text MusicText;
    public Text SfxText;
    public Slider MusicSlider;
    public Slider SfxSlider;

    public Animator UIAnimator;
    public GameObject UIIntroSignal;
    public GameObject UIRestartSignal;
    public GameObject UIEndSignal;

    public CinemachineBrain main_camera_brain;
    private float aspectratio = 16f / 9f;
    private int prev_width, prev_height;

    //public ColorReference[] color_channels;
    //private Color[] channel_colors;

    private float time = 0;
    private bool shortcuts_enabled = false, game = false, paused = false, start = false;

    private GameObject effects;
    private GameObject globallight;
    private GameObject playerlight;
    private GameObject player;
    public MusicSource bgmusic;
    private MusicSource newbgmusic;

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

    private CopterController coptercontroller;
    private AutoCopterController autocoptercontroller;

    private Checkpoint_Controller checkpointcontroller;

    private IconController iconcontroller;
    private GameObject icon;

    private int mana_count = 0;
    private int diamond_count = 0;
    private int[] coin_count = new int[3];
    public Coin[] Coins;
    public GameObject[] CoinIcons;
    public AudioSource coinget;
    public AudioSource diamondget;

    [Range(0f,1f)] [HideInInspector]
    public float music_volume, sfx_volume;

    private int min = 0, sec = 0, milli = 0, m = 0, s = 0;

    private bool postfxon = true;

    private float deltaTime = 0.0f;
    private float endgame_timer = 0;

    private GameObject[] initialList;
    private List<CinemachineVirtualCamera> cameraList;
    CinemachineVirtualCamera activeCamera;

    public GameObject End;

    //PLAYROOM
    public SpriteRenderer Background;
    public TrailRenderer editorTrail;

    private void Awake()
    {
        //PLAYROOM
        //editorTrail.emitting = false;
        //bgmusic = GameObject.Find("BG Music 1").GetComponent<AudioSource>();
        //
        LoadPrefs();

        Resources.UnloadUnusedAssets();
        //Screen.SetResolution(Screen.resolutions[Screen.resolutions.Length - 1].width, Screen.resolutions[Screen.resolutions.Length - 1].height, true);
        //Screen.fullScreen = true;
        prev_width = Screen.width;
        prev_height = Screen.height;

        Restart_Button.onClick.AddListener(/*Restart*/StartRestart);
        Options_Button.onClick.AddListener(Options);
        Menu_Button.onClick.AddListener(ReturnToMenu);
        Fullscreen_Button.onClick.AddListener(ToggleFullscreen);
        Effects_Button.onClick.AddListener(TogglePostProcessing);

        Res1440_Button.onClick.AddListener(() => { SetResolution(1440); });
        Res1080_Button.onClick.AddListener(() => { SetResolution(1080); });
        Res720_Button.onClick.AddListener(() => { SetResolution(720); });

        setButtonOn(Fullscreen_Button, Screen.fullScreen);
        Fullscreen_Button.GetComponentInChildren<Text>().text = "FULLSCREEN: " + (Screen.fullScreen ? "ON" : "OFF");

        setButtonOn(Effects_Button, postfxon);
        Effects_Button.GetComponentInChildren<Text>().text = "POST PROCESSING: " + (postfxon ? "ON" : "OFF");

        MusicSlider.value = music_volume;
        SfxSlider.value = sfx_volume;

        /*int i = 0;
        foreach (Coin c in Coins)
        {
            CoinIcons[i].GetComponent<Image>().color = new Color(1,1,1, coin_count[i] == 1 ? 1 : 0);
            c.gameObject.SetActive(coin_count[i] == 0);
            i++;
        }*/
        int i = 0;
        coin_count[0] = Random.Range(0, 2);
        coin_count[1] = Random.Range(0, 2);
        coin_count[2] = Random.Range(0, 2);
        for (int j = 0; j < coin_count.Length; j++)
        {
            CoinIcons[i].GetComponent<Image>().color = new Color(1, 1, 1, coin_count[j] == 1 ? 1 : 0);
            CoinIcons[i+1].GetComponent<Image>().color = new Color(1, 1, 1, 0);
            CoinIcons[i + 1].gameObject.SetActive(false);

            Coins[i].gameObject.SetActive(coin_count[j] == 0);
            Coins[i+1].gameObject.SetActive(coin_count[j] == 1);
            i += 2;
        }

        player = GameObject.Find("Player");
        playerlight = GameObject.Find("Player Light Bright"); playerlight.SetActive(false);
        effects = GameObject.Find("EFFECTS");
        globallight = GameObject.Find("Global Light");

        //DevTools.color = Color.clear;
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

        coptercontroller = player.GetComponent<CopterController>();
        autocoptercontroller = player.GetComponent<AutoCopterController>();

        //------------------------------------------------------------------------------------------------
        playercontroller = cubecontroller;
        checkpointcontroller = FindObjectOfType<Checkpoint_Controller>();

        playercontroller.setBGMusic(bgmusic.audio);
        newbgmusic = bgmusic;

        iconcontroller = FindObjectOfType<IconController>();
        icon = iconcontroller.getIcon();

        playercontroller.setIcons(icon);

        // ------------------------------------------------------------------------------------------------
        /*channel_colors = new Color[color_channels.Length];
        int i = 0;
        foreach(ColorReference c in color_channels)
        {
            channel_colors[i] = c.channelcolor;
            if(c.refer != null) { channel_colors[i] = c.refer.channelcolor; }
            i++;
        }*/

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

    private void Start()
    {
        /*if (!bgmusic.isPlaying)
        {
            bgmusic.Play();
        }*/
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

    /*void resetColorChannels()
    {
        int i = 0;
        foreach (ColorReference c in color_channels)
        {
            c.Set(channel_colors[i]);
            i++;
        }
    }*/

    // Button Functions
    public void StartRestart()
    {
        paused = false;
        Time.timeScale = 1;
        playercontroller.setAble(false);
        playercontroller.stopBGMusic();

        SavePrefs();

        UIAnimator.Play("UI_Restart_Sequence");
    }
    public void Restart()
    {
        playercontroller.resetStaticVariables();
        //resetColorChannels();
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
        if(!Screen.fullScreen)
        {
            Screen.SetResolution(Screen.resolutions[Screen.resolutions.Length - 1].width, Screen.resolutions[Screen.resolutions.Length - 1].height, true);
        }
        setButtonOn(Fullscreen_Button, !Screen.fullScreen);
        Fullscreen_Button.GetComponentInChildren<Text>().text = "FULLSCREEN: " + (!Screen.fullScreen ? "ON" : "OFF");
        setButtonOn(Res1440_Button, false); setButtonOn(Res1080_Button, false); setButtonOn(Res720_Button, false);
    }
    public void SetResolution(int width)
    {
        switch(width)
        {
            case 1440:
                Screen.SetResolution(2560, 1440, true); Screen.fullScreen = false;
                setButtonOn(Res1440_Button, true); setButtonOn(Res1080_Button, false); setButtonOn(Res720_Button, false);
                setButtonOn(Fullscreen_Button, false);
                Fullscreen_Button.GetComponentInChildren<Text>().text = "FULLSCREEN: OFF"; break;
            case 1080:
                Screen.SetResolution(1920, 1080, true); Screen.fullScreen = false;
                setButtonOn(Res1440_Button, false); setButtonOn(Res1080_Button, true); setButtonOn(Res720_Button, false);
                setButtonOn(Fullscreen_Button, false);
                Fullscreen_Button.GetComponentInChildren<Text>().text = "FULLSCREEN: OFF"; break;
            case 720:
                Screen.SetResolution(1280, 720, true); Screen.fullScreen = false;
                setButtonOn(Res1440_Button, false); setButtonOn(Res1080_Button, false); setButtonOn(Res720_Button, true);
                setButtonOn(Fullscreen_Button, false);
                Fullscreen_Button.GetComponentInChildren<Text>().text = "FULLSCREEN: OFF"; break;
            default:
                break;
        }
        
    }
    public void TogglePostProcessing()
    {
        effects.SetActive(!effects.activeSelf);
        //Debug.Log(main_camera_brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVolumeSettings>() == null);
        postfxon = !postfxon;
        if (postfxon && main_camera_brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVolumeSettings>() != null)
        {
            main_camera_brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVolumeSettings>().enabled = true;
        }
        else
        {
            main_camera_brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVolumeSettings>().enabled = false;
        }

        setButtonOn(Effects_Button, postfxon);
        Effects_Button.GetComponentInChildren<Text>().text = "POST PROCESSING: " + (postfxon ? "ON" : "OFF");
    }
    public void setButtonOn(Button button, bool on)
    {
        if(on)
        {
            ColorBlock colorblock = button.colors;
            colorblock.normalColor = new Color32(255, 255, 255, 255);
            colorblock.highlightedColor = new Color32(245, 245, 245, 112);
            colorblock.pressedColor = new Color32(200, 200, 200, 255);
            colorblock.selectedColor = new Color32(245, 245, 245, 255);
            colorblock.disabledColor = new Color32(128, 128, 128, 255);

            button.colors = colorblock;
            button.GetComponentInChildren<Text>().color = Color.black;
        }
        else
        {
            ColorBlock colorblock = button.colors;
            colorblock.normalColor = new Color32(255, 255, 242, 13);
            colorblock.highlightedColor = new Color32(245, 245, 245, 112);
            colorblock.pressedColor = new Color32(200, 200, 200, 255);
            colorblock.selectedColor = new Color32(245, 245, 245, 255);
            colorblock.disabledColor = new Color32(128, 128, 128, 255);

            button.colors = colorblock;
            button.GetComponentInChildren<Text>().color = Color.white;
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
        if (UIRestartSignal.activeSelf) { Restart(); }
        if (UIIntroSignal.activeSelf)
        {
            playercontroller.forceRespawn();
            //playercontroller.setAble(false);
            return;
        }
        if (!start)
        {
            start = true;
            playercontroller.playBGMusic();
            playercontroller.setAble(true);
        }

        if (Input.GetKeyDown("q"))
        {
            //resetColorChannels();
            SavePrefs();
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
            StartRestart();
        }
        Debug.Log("[" + coin_count[0] + " " + coin_count[1] + " " + coin_count[2] + "]");

        // SOUND
        music_volume = MusicSlider.value;
        sfx_volume = SfxSlider.value;
        MusicText.text = "Music: " + (int)(music_volume * 100);
        SfxText.text = "Sfx: " + (int)(sfx_volume * 100);
        bgmusic.audio.volume = bgmusic.realVolume * music_volume;

        if (!game)
        {
            // PAUSE MENU
            if (Input.GetKeyDown("escape"))
            {
                if (!paused)
                {
                    paused = !paused;
                    Pause_Menu.SetActive(true);
                    bgmusic.Pause();
                    Time.timeScale = 0;
                }
                else if (paused)
                {
                    if (Menu1.activeSelf)
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

            // UI TEXT
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

            milli = (int)(time * 1000) % 1000;
            sec = (int)(time);
            min = (int)(sec / 60);

            milli -= (1000 * m);
            sec -= (60 * s);

            if (milli == 1000) { m++; };
            if (sec == 60) { s++; };

            ManaCount.text = "x" + mana_count;
            DiamondCount.text = "x" + diamond_count;
            Timer.text = "Time: " + min + " : " + (sec < 10 ? "0" : "") + sec + " : " + (milli < 100 ? "0" : "") + (milli < 10 ? "0" : "") + milli;
            FPS.text = Mathf.RoundToInt((1 / deltaTime)) + " FPS\n" + Time.timeScale + "x speed";

            ManaScore.text = ManaCount.text;
            DiamondScore.text = DiamondCount.text;
            TimeScore.text = min + " : " + (sec < 10 ? "0" : "") + sec + " : " + (milli < 100 ? "0" : "") + (milli < 10 ? "0" : "") + milli;

            time += Time.deltaTime;
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

        //Debug.Log(coin_count[0] + ", " + coin_count[1] + ", " + coin_count[2]);

        

        // PLAYROOM
        /*
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
        if (Input.GetKeyDown("e"))
        {
            editorTrail.Clear();
            editorTrail.emitting = !editorTrail.emitting;
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bgmusic.Play();
        }
        //*/

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

    private void FixedUpdate()
    {
        if(game)
        {
            player.GetComponent<Collider2D>().isTrigger = true;

            Rigidbody2D playerbody = player.GetComponent<Rigidbody2D>();

            playercontroller.setAble(false);
            playercontroller.TurnOffEverything();
            playercontroller.enabled = false;

            playerbody.interpolation = RigidbodyInterpolation2D.Extrapolate;
            playerbody.velocity = Vector2.zero;
            playerbody.gravityScale = 0;
            playerbody.rotation = Mathf.Lerp(playerbody.rotation, playerbody.rotation - 10, .6f);
        }
    }

    public IEnumerator countScore()
    {
        while(!UIEndSignal.activeSelf)
        {
            yield return null;
        }

        int j = 0;
        for (int i = 0; i < coin_count.Length; i++)
        {
            if (coin_count[i] == 2)
            {
                j = 2 * i;
                coin_count[i] = 1;
                CoinIcons[j].transform.localScale = new Vector3(3, 3, 1);
                Image coinimage = CoinIcons[j].GetComponent<Image>();
                coinimage.color = new Color(1, 1, 1, 0);

                yield return null;

                float timer = 0;

                coinget.PlayOneShot(coinget.clip, sfx_volume);
                while (coinimage.color.a < 1f)
                {
                    CoinIcons[j].transform.localScale = Vector3.Lerp(new Vector3(3, 3, 1), new Vector3(1, 1, 1), timer / .2f);
                    coinimage.color = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), timer / .2f);
                    timer += Time.deltaTime;

                    yield return null;
                }

                //CoinIcons[i].transform.localScale = new Vector3(1, 1, 1);
                coinimage.color =new Color(1, 1, 1, 1);

                timer = 0;
                while(timer < .1f)
                {
                    timer += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
            }
            else if(coin_count[i] == 3)
            {
                j = 2 * i;
                coin_count[i] = 1;
                CoinIcons[j].gameObject.SetActive(false);
                CoinIcons[j+1].gameObject.SetActive(true);
                CoinIcons[j+1].transform.localScale = new Vector3(3, 3, 1);
                Image coinimage = CoinIcons[j+1].GetComponent<Image>();
                coinimage.color = new Color(1, 1, 1, 0);

                yield return null;

                float timer = 0;

                coinget.PlayOneShot(coinget.clip, sfx_volume);
                while (coinimage.color.a < 1f)
                {
                    CoinIcons[j+1].transform.localScale = Vector3.Lerp(new Vector3(3, 3, 1), new Vector3(1, 1, 1), timer / .2f);
                    coinimage.color = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), timer / .2f);
                    timer += Time.deltaTime;

                    yield return null;
                }

                //CoinIcons[i].transform.localScale = new Vector3(1, 1, 1);
                coinimage.color = new Color(1, 1, 1, 1);

                timer = 0;
                while (timer < .1f)
                {
                    timer += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
            }
        }
        
        int diamondgain = 0;
        for(int i = mana_count; i > 0; i--)
        {
            float timer = 0;
            while (timer < .001f)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            mana_count--;
            diamondgain++;

            if(diamondgain % 25 == 0)
            {
                diamond_count++;
                diamondget.PlayOneShot(diamondget.clip, sfx_volume);
            }

            ManaScore.text = "x" + mana_count;
            DiamondScore.text = "x" + diamond_count;
        }
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
        else if (coptercontroller.isActiveAndEnabled)
        {
            playercontroller = coptercontroller;
            return playercontroller;
        }
        else if (autocoptercontroller.isActiveAndEnabled)
        {
            playercontroller = autocoptercontroller;
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

            /*if (mode.Equals("cube")) { playercontroller = cubecontroller; }
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
            else if (mode.Equals("auto_spider")) { playercontroller = autospidercontroller; }*/

            switch(mode)
            {
                case "cube": playercontroller = cubecontroller; break;
                case "auto": playercontroller = autocontroller; break;
                case "ship": playercontroller = shipcontroller; break;
                case "auto_ship": playercontroller = autoshipcontroller; break;
                case "ufo": playercontroller = ufocontroller; break;
                case "auto_ufo": playercontroller = autoufocontroller; break;
                case "wave": playercontroller = wavecontroller; break;
                case "auto_wave": playercontroller = autowavecontroller; break;
                case "ball": playercontroller = ballcontroller; break;
                case "auto_ball": playercontroller = autoballcontroller; break;
                case "spider": playercontroller = spidercontroller; break;
                case "auto_spider": playercontroller = autospidercontroller; break;
                case "copter":
                    bool gu = false;
                    if (playercontroller == autocoptercontroller)
                    {
                        gu = autocoptercontroller.getGoingUp();
                    }
                    playercontroller = coptercontroller; coptercontroller.setGoingUp(gu); break;
                case "auto_copter":
                    gu = false;
                    if (playercontroller == coptercontroller)
                    {
                        gu = coptercontroller.getGoingUp();
                    }
                    playercontroller = autocoptercontroller; autocoptercontroller.setGoingUp(gu); break;
            }

            playercontroller.setAble(false);
            playercontroller.setColliders();
            playercontroller.setIcons(icon);
            playercontroller.setBGMusic(bgmusic.audio);
            //playercontroller.setSpeed(speed);
            playercontroller.setRestartMusicOnDeath(restartmusic);

            if (startmusic)
            {
                bgmusic.Play();
                //bgmusic.volume = music_volume;
                bgmusic.realVolume = 1;
                bgmusic.audio.volume = music_volume;
            }
            /*
            if (checkpointcontroller.getIndex() != -1)
            {
                playercontroller.setRespawn(checkpointcontroller.getTransform(), checkpointcontroller.getReversed(), checkpointcontroller.getMini());
                playercontroller.setRepawnSpeed(checkpointcontroller.getSpeed());
            }*/


            //playercontroller.setVariables((Input.GetButton("Jump") || Input.GetKey("space")), reversed, mini);
            playercontroller.setAnimation();
            playercontroller.enabled = true;
            playercontroller.setAble(true);

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

        playercontroller.setBGMusic(bgmusic.audio);

        //bgmusic.volume = playvolume * music_volume;
        bgmusic.realVolume = playvolume;
        bgmusic.audio.volume = bgmusic.realVolume * music_volume;
        bgmusic.Play();
    }

    public void setBGMusic(MusicSource music)
    {
        newbgmusic = music;
    }

    public MusicSource getBGMusic()
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

    public void incrementCoinCount(int num, bool check, bool ghost)
    {
        if (check) { coin_count[num - 1] = -1 + (ghost?-1:0); }
        else { coin_count[num - 1] = 2 + (ghost ? 1 : 0); }
    }

    public int[] getCoinCount()
    {
        return coin_count;
    }

    public void resolveCoins(bool proceed)
    {
        for (int i = 0; i < coin_count.Length; i++)
        {
            if(coin_count[i] == -1)
            {
                coin_count[i] = proceed ? 2 : 0;
                if (!proceed)
                {
                    Coins[i*2].resetCoin();
                }
                else
                {
                    Destroy(Coins[i * 2].gameObject);
                }
            }
            else if (coin_count[i] == -2)
            {
                coin_count[i] = proceed ? 3 : 1;
                if (!proceed)
                {
                    Coins[(i * 2) + 1].resetCoin();
                }
                else
                {
                    Destroy(Coins[(i * 2) + 1].gameObject);
                    //CoinIcons[i * 2].gameObject.SetActive(false);
                    //CoinIcons[(i * 2) + 1].gameObject.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            game = true;
        }
    }

    public bool getpostfx()
    {
        return postfxon;
    }

    public float getTime()
    {
        return time;
    }

    public int getLevelNumber()
    {
        return 1;
    }

    // PLAYER PREFERENCES
    public void SavePrefs()
    {
        PlayerPrefs.SetFloat("music_volume", music_volume);
        PlayerPrefs.SetFloat("sfx_volume", sfx_volume);
        PlayerPrefs.SetInt("post_proccessing_on", postfxon ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadPrefs()
    {
        music_volume = PlayerPrefs.GetFloat("music_volume", .8f);
        sfx_volume = PlayerPrefs.GetFloat("sfx_volume", 1);
        postfxon = PlayerPrefs.GetInt("post_proccessing_on") == 1 ? true : false;
    }
}
