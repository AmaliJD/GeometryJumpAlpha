using System.Collections;
using System.Collections.Generic;
//using System.Security.Policy; //THIS IS THE MARK OF THE UNITY GHOST
using UnityEngine;

public abstract class PlayerController : MonoBehaviour
{
    // The different 'protected' variables are separated based on how they functioned in their original scripts

    // SHARED "PRIVATE" VARIABLES
    protected GameObject eyes;
    protected GameObject icon;

    protected Rigidbody2D player_body;
    protected Animator Cube_Anim;

    protected AudioSource bgmusic;

    protected Vector3 v_Velocity; // ref Velocity

    // SHARED STATIC VARIABLES
    static public bool jump = false, isjumping = false, jump_ground = false, reversed = false, downjump = false, released = false;
    static public bool grounded = false, prev_grounded = false, fromGround = false, checkGrounded = true;
    static public bool dead = false, check_death = false, able = true;

    static public float prev_velocity = 0;
    static public float negate = 1, regate = 1;

    static public bool respawn_rev = false, respawn_mini = false;
    static public bool restartmusic = false;

    static public bool crouch = false, crouched = true;
    static public bool facingright = true, upright = true, mini = false;

    static public bool yellow = false, blue = false, red = false, pink = false, green = false, black = false, teleorb = false, triggerorb = false;
    static public bool yellow_j = false, red_j = false, blue_j = false, pink_j = false, green_j = false, black_j = false, teleorb_j = false, triggerorb_j = false;
    static public bool yellow_p = false, blue_p = false, red_p = false, pink_p = false;
    static public bool grav = false, gravN = false, teleA = false;
    static public Vector3 teleB, respawn, teleOrb_translate;

    static public float speed, speed0 = 40f, speed1 = 55f, speed2 = 75f, speed3 = 90f, speed4 = 110f, respawn_speed;

    public void resetStaticVariables()
    {
        jump = false;
        isjumping = false;
        jump_ground = false;
        reversed = false;
        downjump = false;
        released = false;

        grounded = false;
        prev_grounded = false;
        fromGround = false;
        checkGrounded = true;

        dead = false;
        check_death = false;
        able = true;

        respawn_rev = false;
        respawn_mini = false;

        crouch = false;
        crouched = true;

        facingright = true;
        upright = true;
        mini = false;

        yellow = false; blue = false; red = false; pink = false; green = false; black = false;
        yellow_j = false; red_j = false; blue_j = false; pink_j = false; green_j = false; black_j = false;
        yellow_p = false; blue_p = false; red_p = false; pink_p = false;
        grav = false; gravN = false; teleA = false;

        speed0 = 40f; speed1 = 55f; speed2 = 75f; speed3 = 90f; speed4 = 110f;
    }

    // PROTECTED "PUBLIC" VARIABLES
    [SerializeField] protected GameObject player_renderer;

    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected LayerMask deathLayer;
    
    [SerializeField] protected ParticleSystem death_particles;
    [SerializeField] protected AudioSource death_sfx;

    // SHARED METHODS
    private void Awake()
    {
        player_body = GetComponent<Rigidbody2D>();
        Cube_Anim = player_renderer.transform.GetComponent<Animator>();

        eyes = GameObject.Find("Icon_Eyes");
        eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
        eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
        eyes.transform.Find("Eyes_Wide").gameObject.SetActive(false);
        eyes.transform.Find("Eyes_Normal").gameObject.SetActive(true);

        setRespawn(transform.position, reversed, mini);
        setRepawnSpeed(1);

        v_Velocity = Vector3.zero;
        speed = speed1;
        
        Awake2();
    }

    // SET METHODS
    public void setIcons(GameObject i)
    {
        icon = i;
    }
    public void setBGMusic(AudioSource audio)
    {
        bgmusic = audio;
    }
    public void setRestartMusicOnDeath(bool r)
    {
        restartmusic = r;
    }
    public void setAble(bool a)
    {
        able = a;
    }

    // GET METHODS
    public bool isDead()
    {
        return dead;
    }

    // OnCollision
    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (!enabled) { return; }
        if (collision.gameObject.tag == "TriggerOrb")
        {
            triggerorb = true;
        }
        if (collision.gameObject.tag == "TeleOrb")
        {
            teleorb = true;

            teleOrb_translate = collision.gameObject.GetComponent<OrbComponent>().getTeleport().position - collision.gameObject.GetComponent<OrbComponent>().transform.position;
            teleOrb_translate.z = 0;
        }
        if (collision.gameObject.tag == "YellowOrb")
        {
            yellow = true;
        }
        if (collision.gameObject.tag == "BlueOrb")
        {
            blue = true;
        }
        if (collision.gameObject.tag == "PinkOrb")
        {
            pink = true;
        }
        if (collision.gameObject.tag == "RedOrb")
        {
            red = true;
        }
        if (collision.gameObject.tag == "GreenOrb")
        {
            green = true;
        }
        if (collision.gameObject.tag == "BlackOrb")
        {
            black = true;
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled) { return; }
        if (collision.gameObject.tag == "PortalGravity")
        {
            grav = true;
        }
        if (collision.gameObject.tag == "PortalGravityN")
        {
            gravN = true;
        }
        if (collision.gameObject.tag == "TeleportA")
        {
            teleA = true;

            Transform t = collision.gameObject.transform;

            Transform tb = t;
            foreach (Transform tr in t)
            {
                if (tr.tag == "TeleportB")
                {
                    tb = tr.GetComponent<Transform>();
                    break;
                }
            }

            teleB = tb.position - t.position;
            teleB.z = 0;
        }
        if (collision.gameObject.tag == "YellowPad")
        {
            yellow_p = true;
        }
        if (collision.gameObject.tag == "PinkPad")
        {
            pink_p = true;
        }
        if (collision.gameObject.tag == "RedPad")
        {
            red_p = true;
        }
        if (collision.gameObject.tag == "BluePad")
        {
            blue_p = true;
        }
        if (collision.gameObject.tag == "Speed0x")
        {
            speed = speed0;
        }
        if (collision.gameObject.tag == "Speed1x")
        {
            speed = speed1;
        }
        if (collision.gameObject.tag == "Speed2x")
        {
            speed = speed2;
        }
        if (collision.gameObject.tag == "Speed3x")
        {
            speed = speed3;
        }
        if (collision.gameObject.tag == "Speed4x")
        {
            speed = speed4;
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (!enabled) { return; }
        if (collision.gameObject.tag == "TriggerOrb")
        {
            triggerorb = false;
        }
        if (collision.gameObject.tag == "TeleOrb")
        {
            teleorb = false;
        }
        if (collision.gameObject.tag == "YellowOrb")
        {
            yellow = false;
        }
        if (collision.gameObject.tag == "BlueOrb")
        {
            blue = false;
        }
        if (collision.gameObject.tag == "PinkOrb")
        {
            pink = false;
        }
        if (collision.gameObject.tag == "RedOrb")
        {
            red = false;
        }
        if (collision.gameObject.tag == "GreenOrb")
        {
            green = false;
        }
        if (collision.gameObject.tag == "BlackOrb")
        {
            black = false;
        }
        if (collision.gameObject.tag == "YellowPad")
        {
            yellow_p = false;
        }
        if (collision.gameObject.tag == "PinkPad")
        {
            pink_p = false;
        }
        if (collision.gameObject.tag == "RedPad")
        {
            red_p = false;
        }
        if (collision.gameObject.tag == "BluePad")
        {
            blue_p = false;
        }
    }

    // ABSTRACT METHOD ----------------------------------------------------------
    public abstract void Awake2();
    public abstract void setAnimation();
    public abstract void Move();
    public abstract void Jump();
    public abstract void Pad();
    public abstract void Portal();
    public abstract bool isJumping();
    public abstract void setIsJumping(bool j);
    public abstract void Flip();
    public abstract void Respawn();
    public abstract void setRespawn(Vector3 pos, bool rev, bool min);
    public abstract void resetBooleans();
    public abstract void setRepawnSpeed(float s);
    public abstract void setSpeed(float s);
    public abstract float getSpeed();
    public abstract void resetColliders();
    public abstract void setColliders();
    public abstract string getMode();
}
