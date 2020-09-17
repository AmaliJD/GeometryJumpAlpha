using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AutoController : PlayerController
{
    //  reference velocity
    private Vector3 v_Velocity;

    private Rigidbody2D player_body;
    public Collider2D player_collider;
    public CircleCollider2D circle_collider;

    private AudioSource bgmusic;

    public LayerMask groundLayer;
    public LayerMask deathLayer;
    public TrailRenderer trail;

    public ParticleSystem death_particles;
    public GameObject player_renderer;
    public Transform HEIGHT;
    private GameObject eyes;
    private GameObject icon;

    private float jumpForce = 19.2f; //19.2
    private float speed, speed0 = 40, speed1 = 55f, speed2 = 75f, speed3 = 90f, speed4 = 110f, respawn_speed;
    private float posJump, negate = 1, regate = 1;

    private float moveX, grav_scale;
    private float smoothing;

    private float time = 0;
    private bool grounded = false, prev_grounded = true, fromGround = false, reversed = false, jump = false, jump_ground = false, downjump = false, released = false, checkGrounded = true;
    private bool yellow = false, blue = false, red = false, pink = false, green = false, black = false;
    private bool yellow_p = false, blue_p = false, red_p = false, pink_p = false;
    private bool grav = false, gravN = false, teleA = false;
    private Vector3 teleB, respawn;
    private bool respawn_rev = false, respawn_mini = false;
    private bool crouch = false, facingRight = true, dead = false, check_death = false, able = true;

    private bool isjumping = false;

    private float maxSpeed = 21f * 1.5f;

    private bool mini = false;

    void Awake()
    {
        speed = getSpeed();
        moveX = speed;
        smoothing = .05f;
        v_Velocity = Vector3.zero;
        posJump = jumpForce;
        player_body = GetComponent<Rigidbody2D>();
        player_collider = GetComponent<Collider2D>();

        grav_scale = player_body.gravityScale;

        circle_collider.enabled = false;
        setRespawn(transform.position, reversed, false);
        setRepawnSpeed(1f);

        eyes = GameObject.Find("Icon_Eyes");
        //setAnimation();
    }

    private void Start()
    {
        //bgmusic.Play();
        //player_body.freezeRotation = false;
    }

    public override void setAnimation()
    {
        player_renderer.SetActive(true);
        player_body.freezeRotation = true;
        player_body.gravityScale = 9.6f;
        if (reversed) { player_body.gravityScale *= -1; }
        grav_scale = player_body.gravityScale;

        Vector3 newAngle = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.Euler(newAngle);

        ChangeSize();

        icon.transform.localScale = new Vector3(1f, 1f, 1f);
        icon.transform.localPosition = new Vector3(0, 0, 0);
        icon.SetActive(true);

        trail.transform.localPosition = new Vector3(0, 0, 0);

        HEIGHT.GetComponent<Animator>().ResetTrigger("Crouch");
        HEIGHT.GetComponent<Animator>().ResetTrigger("Squash");
        HEIGHT.GetComponent<Animator>().ResetTrigger("Stretch");
        HEIGHT.GetComponent<Animator>().SetTrigger("Default");

        eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
        eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
        eyes.transform.Find("Eyes_Wide").gameObject.SetActive(false);
        eyes.transform.Find("Eyes_Normal").gameObject.SetActive(true);
    }

    void ChangeSize()
    {
        if (mini)
        {
            transform.localScale = new Vector2(.47f, .47f);
            jumpForce = 15f;
        }
        else
        {
            transform.localScale = new Vector2(1f, 1f);
            jumpForce = 19.2f;
        }

        posJump = jumpForce;
        if (reversed) { jumpForce *= -1; }
    }

    public override void setIcons(GameObject i)
    {
        icon = i;
    }

    void Update()
    {
        if (able)
        {
            // CHECK IF DEAD
            dead = check_death && (Physics2D.IsTouchingLayers(circle_collider, deathLayer) || Mathf.Abs(player_body.velocity.x) <= .2f);
            //grounded = Physics2D.Raycast(player_body.transform.position, Vector2.down, .51f, groundLayer);

            // CHECK IF GROUNDED
            if (reversed)
            {
                grounded = !Physics2D.BoxCast(player_body.transform.position, new Vector2(.95f, .1f), 0f, Vector2.down, .51f, groundLayer) && checkGrounded
                        && (Physics2D.IsTouchingLayers(player_collider, groundLayer) || Physics2D.IsTouchingLayers(circle_collider, groundLayer));
                regate = -1;
            }
            else
            {//.9
                grounded = !Physics2D.BoxCast(player_body.transform.position, new Vector2(.95f, .1f), 0f, Vector2.up, .51f, groundLayer) && checkGrounded
                        && (Physics2D.IsTouchingLayers(player_collider, groundLayer) || Physics2D.IsTouchingLayers(circle_collider, groundLayer));
                regate = 1;
            }

            // IF GROUNDED --> TURN OFF TRAIL
            /*
            if (grounded && (!red_p && !yellow_p && !blue_p && !pink_p))
            {
                trail.emitting = false;
                animator.SetBool("Jump", false);
                animator.SetBool("Orb", false);
            }*/

            // LIMIT Y SPEED
            if (player_body.velocity.y > maxSpeed)
            {
                player_body.velocity = new Vector2(player_body.velocity.x, maxSpeed);
            }
            else if (player_body.velocity.y < -maxSpeed)
            {
                player_body.velocity = new Vector2(player_body.velocity.x, -maxSpeed);
            }


            // Movement Speed
            moveX = speed;

            // JUMP!
            if (Input.GetButtonDown("Jump") || Input.GetKeyDown("space"))
            {
                jump = true;
                jump_ground = true;
                released = false;
                fromGround = ((grounded || time < 0.05f) && jump);

                if (!fromGround)
                {
                    isjumping = true;
                }
                if (!reversed && player_body.velocity.y <= 1)
                {
                    downjump = true;
                }
                else if (reversed && player_body.velocity.y >= -1)
                {
                    downjump = true;
                }
                else
                {
                    downjump = false;
                }
            }

            // RELEASE JUMP
            if (Input.GetButtonUp("Jump") || Input.GetKeyUp("space"))
            {
                isjumping = false;
                jump = false;
                jump_ground = false;
                released = true;
            }

            // CHANGE JUMP DIRECTION WHEN REVERSED
            if (reversed)
            {
                jumpForce = -posJump;
            }
            else
            {
                jumpForce = posJump;
            }

            // IF DEAD --> RESPAWN
            if (dead)
            {
                dead = false;
                Respawn();
            }

            prev_grounded = grounded;
            time += Time.deltaTime;
            if (prev_grounded) { time = 0; }

            //dead = Physics2D.IsTouchingLayers(circle_collider, deathLayer) || Mathf.Abs(player_body.velocity.x) <= 1f;
        }
    }

    void FixedUpdate()
    {
        // one job and one job only. MOVE
        if (able)
        {
            Move();
        }
    }

    public override void Move()
    {
        // If the input is moving the player right and the player is facing left...
        if ((!reversed && speed > 0 && !facingRight && grounded) || (reversed && speed < 0 && !facingRight && grounded))
        {
            // ... flip the player.
            negate = 1;
            facingRight = !facingRight;
            //Flip();
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if ((!reversed && speed < 0 && facingRight && grounded) || (reversed && speed > 0 && facingRight && grounded))
        {
            // ... flip the player.
            negate = -1;
            facingRight = !facingRight;
            //Flip();
        }

        // movement controls
        Vector2 targetVelocity = new Vector2(moveX * Time.fixedDeltaTime * 10f, player_body.velocity.y);
        player_body.velocity = targetVelocity;

        /*
        if (Mathf.Abs(targetVelocity.x) > Mathf.Abs(player_body.velocity.x) || !grounded)
        {
            player_body.velocity = Vector3.SmoothDamp(player_body.velocity, targetVelocity, ref v_Velocity, smoothing * .7f);
        }
        else
        {
            player_body.velocity = Vector3.SmoothDamp(player_body.velocity, targetVelocity, ref v_Velocity, smoothing * 1.5f);
        }*/

        Rotate();
        Eyes();
        Pad();
        Jump();
        Portal();

        // IF GROUNDED --> TURN OFF TRAIL
        if (grounded && Mathf.Abs(player_body.velocity.y) <= 0.01 && (!red_p && !yellow_p && !blue_p && !pink_p))
        {
            trail.emitting = false;
        }

        check_death = true;
    }

    //private bool resetRotation = true;

    public void Rotate()
    {
        //Debug.Log(Mathf.Abs(transform.rotation.eulerAngles.z % 90) <= .001f);

        if (grounded && Mathf.Abs(transform.rotation.eulerAngles.z % 90) <= .001f) { return; }
        
        //Vector3 axis = Vector3.forward;
        float step = -7.5f;

        if (reversed)
        {
            step = 7.5f;
        }

        //Debug.Log(grounded);

        if (!grounded)
        {
            Vector3 newAngle = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + step);
            transform.rotation = Quaternion.Euler(newAngle);
        }
        else
        {
            float difference = transform.rotation.eulerAngles.z % 90;
            if (difference >= 55)
            {
                transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, 
                    new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + (90 - difference)), .8f);
            }
            else
            {
                transform.eulerAngles = Vector3.Lerp(transform.eulerAngles,
                    new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - difference), .7f);
            }
        }
    }

    public void Eyes()
    {
        int rev = 1;
        if (reversed) { rev = -1; }
        eyes.transform.localPosition = Vector3.Lerp(eyes.transform.localPosition, new Vector3(rev * (moveX / 800), 0 * rev * (player_body.velocity.y / 80), 0), .4f);

        if (!grounded)
        {
            return;
        }
        else
        {
            if((int)Mathf.Abs(transform.rotation.eulerAngles.z / 90) == 0)
            {
                eyes.transform.Find("Eyes_Wide").gameObject.SetActive(false);
                eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
                eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
                eyes.transform.Find("Eyes_Normal").gameObject.SetActive(true);
            }
            else if ((int)Mathf.Abs(transform.rotation.eulerAngles.z / 90) == 1)
            {
                eyes.transform.Find("Eyes_Wide").gameObject.SetActive(false);
                eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
                eyes.transform.Find("Eyes_Normal").gameObject.SetActive(false);
                eyes.transform.Find("Eyes_Squint").gameObject.SetActive(true);
            }
            else if ((int)Mathf.Abs(transform.rotation.eulerAngles.z / 90) == 2)
            {
                eyes.transform.Find("Eyes_Wide").gameObject.SetActive(false);
                eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
                eyes.transform.Find("Eyes_Normal").gameObject.SetActive(false);
                eyes.transform.Find("Eyes_Squint").gameObject.SetActive(true);
            }
            else if ((int)Mathf.Abs(transform.rotation.eulerAngles.z / 90) == 3)
            {
                eyes.transform.Find("Eyes_Wide").gameObject.SetActive(false);
                eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
                eyes.transform.Find("Eyes_Normal").gameObject.SetActive(false);
                eyes.transform.Find("Eyes_Irked").gameObject.SetActive(true);
            }
        }
    }

    public override void Jump()
    {
        if (yellow && jump)
        {
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(true);

            fromGround = false;
            released = false;
            jump = false;
            yellow = false;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * 1.1f);
            trail.emitting = true;

            if (grav) { grav = false; }
            if (gravN) { gravN = false; }
        }
        else if (pink && jump)
        {
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(true);

            fromGround = false;
            released = false;
            jump = false;
            pink = false;
            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * .94f);

            if (grav) { grav = false; }
            if (gravN) { gravN = false; }
        }
        else if (red && jump)
        {
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(true);

            fromGround = false;
            released = false;
            jump = false;
            red = false;
            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * 1.35f);

            if (grav) { grav = false; }
            if (gravN) { gravN = false; }
        }
        else if (blue && jump)
        {
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(true);

            fromGround = false;
            released = false;
            jump = false;
            blue = false;
            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * .4f);
            reversed = !reversed;
            player_body.gravityScale *= -1;
            grav_scale *= -1;
            grounded = false;

            if (grav) { grav = false; }
            if (gravN) { gravN = false; }
        }
        else if (green && jump)
        {
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(true);

            fromGround = false;
            released = false;
            jump = false;
            green = false;
            reversed = !reversed;

            if (reversed)
            {
                jumpForce = -posJump;
            }
            else
            {
                jumpForce = posJump;
            }
            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * 1.1f);
            player_body.gravityScale *= -1;
            grav_scale *= -1;

            if (grav) { grav = false; }
            if (gravN) { gravN = false; }
        }
        else if (black && jump)
        {
            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(true);

            fromGround = false;
            black = false;
            released = false;
            jump = true;
            downjump = true;
            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, -jumpForce * 1.1f);
        }
        else if ((grounded || time < 0.05f) && jump_ground)
        {
            time = 1;
            trail.emitting = false;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce);
            grounded = false;

            checkGrounded = false;
            checkGrounded = true;

            fromGround = true;
            jump = false;
            downjump = false;
        }/*
        else if (fromGround && ((!reversed && released && player_body.velocity.y > 0) || (reversed && released && player_body.velocity.y < 0)))
        {
            player_body.velocity /= 2;
            released = false;
            fromGround = false;
        }*/
    }

    public override bool isJumping()
    {
        return isjumping;
    }

    public override void setIsJumping(bool j)
    {
        isjumping = j;
    }

    public override void Pad()
    {
        if (yellow_p)
        {
            //yellow_p = false;
            jump_ground = false;
            jump = false;
            checkGrounded = false;
            grounded = false;

            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(true);

            //animator.SetBool("Orb", true);
            //jump = false;
            fromGround = false;
            released = false;
            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * 1.55f);
            yellow_p = false;

            checkGrounded = true;
        }
        else if (pink_p)
        {
            jump_ground = false;
            jump = false;
            checkGrounded = false;
            grounded = false;

            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(true);

            //jump = false;
            fromGround = false;
            released = false;
            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * .9f);
            pink_p = false;

            checkGrounded = true;
        }
        else if (red_p)
        {
            jump_ground = false;
            jump = false;
            checkGrounded = false;
            grounded = false;

            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(true);

            fromGround = false;
            released = false;
            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * 1.7f);
            red_p = false;

            checkGrounded = true;
        }
        else if (blue_p)
        {
            jump_ground = false;
            jump = false;
            checkGrounded = false;
            blue_p = false;
            grounded = false;
            fromGround = false;
            released = false;

            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(true);

            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * .4f);
            reversed = !reversed;
            player_body.gravityScale *= -1;
            grav_scale *= -1;

            checkGrounded = true;
        }
    }

    public override void Portal()
    {
        if (grav)
        {
            grav = false;

            if (!reversed)
            {
                fromGround = false;
                released = false;

                reversed = true;
                jumpForce = -posJump;
                trail.emitting = true;
                /*
                if (Mathf.Abs(player_body.velocity.y) > maxSpeed * .6f)
                {
                    player_body.velocity = new Vector2(player_body.velocity.x, player_body.velocity.y * .6f);
                }
                else
                {
                    player_body.velocity = new Vector2(player_body.velocity.x, player_body.velocity.y * .8f);
                }*/
                if (player_body.velocity.y <= -15f)
                {
                    player_body.velocity = new Vector2(player_body.velocity.x, -15f);
                }

                player_body.gravityScale = -Mathf.Abs(player_body.gravityScale);
                grav_scale = player_body.gravityScale;
            }
        }
        else if (gravN)
        {
            gravN = false;

            if (reversed)
            {
                fromGround = false;
                released = false;

                reversed = false;
                jumpForce = posJump;
                trail.emitting = true;
                /*
                if (Mathf.Abs(player_body.velocity.y) > maxSpeed * .6f)
                {
                    player_body.velocity = new Vector2(player_body.velocity.x, player_body.velocity.y * .5f);
                }
                else
                {
                    player_body.velocity = new Vector2(player_body.velocity.x, player_body.velocity.y * .75f);
                }*/
                if (player_body.velocity.y >= 15f)
                {
                    player_body.velocity = new Vector2(player_body.velocity.x, 15f);
                }
                player_body.gravityScale = Mathf.Abs(player_body.gravityScale);
                grav_scale = player_body.gravityScale;
            }
        }
        else if (teleA)
        {
            //trail.emitting = false;
            //trail.Clear();
            trail.enabled = true;
            teleA = false;
            player_body.transform.position += teleB;
            //trail.enabled = true;
        }
    }


    // COROUTUNES
    // none needed

    public override void Flip()
    {
        // Switch the way the player is labelled as facing.
        //facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }


    private void OnTriggerEnter2D(Collider2D collision)
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

            //teleB = t.InverseTransformPoint(tb.position);
            teleB = tb.position - t.position;
            //teleB.z = transform.position.z;
            teleB.z = 0;

            //.35
            /*
            if (player_body.velocity.x > 0)
            {
                teleB.x -= .4f;
            }
            else if (player_body.velocity.x < 0)
            {
                teleB.x += .4f;
            }

            if (player_body.velocity.y < 0)
            {
                teleB.y += .4f;
            }
            else if (player_body.velocity.y > 0)
            {
                teleB.y -= .4f;
            }*/
        }
        if (collision.gameObject.tag == "Mini")
        {
            if(!mini)
            {
                mini = true;
                ChangeSize();
            }

        }
        if (collision.gameObject.tag == "Mega")
        {
            if (mini)
            {
                mini = false;
                ChangeSize();
            }
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
        if (collision.gameObject.tag == "BlueOrb")
        {
            blue = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!enabled) { return; }
        if (collision.gameObject.tag == "YellowOrb")
        {
            yellow = true;
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!enabled) { return; }
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

    private bool restartmusic = false;
    public override void Respawn()
    {
        able = false;
        if (restartmusic) { bgmusic.Stop(); }
        player_collider.enabled = false;
        circle_collider.enabled = false;
        StopAllCoroutines();
        player_body.velocity = Vector2.zero;
        trail.emitting = false;
        jump = false;
        jump_ground = false;
        yellow = false; pink = false; red = false; green = false; blue = false; black = false;
        reversed = respawn_rev;
        mini = respawn_mini;
        ChangeSize();
        if (reversed)
        {
            player_body.gravityScale = -Mathf.Abs(player_body.gravityScale);
            grav_scale = player_body.gravityScale;
            transform.rotation = new Quaternion(0, 0, 180, 0);
        }
        else
        {
            player_body.gravityScale = Mathf.Abs(player_body.gravityScale);
            grav_scale = player_body.gravityScale;
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }

        player_renderer.SetActive(false);
        //player_renderer.enabled = false;
        //death_animation.GetComponent<SpriteRenderer>().enabled = true;
        death_particles.Play();
        death_sfx.PlayOneShot(death_sfx.clip, 1f);
        player_body.gravityScale = 0;

        Invoke("reposition", 1f);
        //player_body.transform.position += respawn - transform.position;

    }

    public void reposition()
    {
        player_body.transform.position += respawn - transform.position;
        player_collider.enabled = true;
        Invoke("undead", .5f);
    }

    public void undead()
    {
        if (!enabled)
        {
            Debug.Log("KMY SORRY");
            jump = false;
            jump_ground = false;
            dead = true;
            player_renderer.SetActive(true);
            speed = respawn_speed;
            resetColliders();
            return;
        }

        player_collider.enabled = false;
        speed = respawn_speed;
        //player_collider.enabled = true;
        circle_collider.enabled = true;
        player_renderer.SetActive(true);
        //player_renderer.enabled = true;
        player_body.gravityScale = grav_scale;

        bgmusic.volume = 1;
        if (restartmusic) { bgmusic.Play(); }

        Vector2 targetVelocity = new Vector2(speed * Time.fixedDeltaTime * 10f, player_body.velocity.y);
        player_body.velocity = targetVelocity;

        dead = false;
        able = true;
    }

    public override void setRespawn(Vector3 pos, bool rev, bool min)
    {
        respawn = pos;
        respawn_rev = rev;
        respawn_mini = min;
        respawn.z = transform.position.z;
    }

    public override void resetBooleans()
    {
        StopAllCoroutines();
        reversed = false; jump = false; jump_ground = false; yellow = false; pink = false; red = false; green = false; blue = false; black = false; teleA = false;
    }

    public override void setBGMusic(AudioSource audio)
    {
        bgmusic = audio;
    }

    public override void setRepawnSpeed(float s)
    {
        if (s == 0) { respawn_speed = speed0; }
        else if (s == 1) { respawn_speed = speed1; }
        else if (s == 2) { respawn_speed = speed2; }
        else if (s == 3) { respawn_speed = speed3; }
        else if (s == 4) { respawn_speed = speed4; }
    }

    public override void setSpeed(float s)
    {
        if (s == 0 || s == speed0) { speed = speed0; }
        else if (s == 1 || s == speed1) { speed = speed1; }
        else if (s == 2 || s == speed2) { speed = speed2; }
        else if (s == 3 || s == speed3) { speed = speed3; }
        else if (s == 4 || s == speed4) { speed = speed4; }
    }

    public override float getSpeed()
    {
        return speed;
    }

    public override void resetColliders()
    {
        player_collider.isTrigger = false;
        player_collider.enabled = false;
        circle_collider.enabled = false;
        jump = false;
        jump_ground = false;
    }
    public override void setColliders()
    {
        player_body.gravityScale = 9.81f;
        if (reversed) { player_body.gravityScale *= -1; }
        grav_scale = player_body.gravityScale;

        check_death = false;

        circle_collider.enabled = true;
        player_collider.enabled = false;
        //player_collider.isTrigger = true;

    }
    public override void setRestartMusicOnDeath(bool r)
    {
        restartmusic = r;
    }
    public override void setAble(bool a)
    {
        able = a;
    }
    public override void setVariables(bool j, bool r, bool m)
    {
        jump_ground = false;
        jump = j;
        reversed = r;
        mini = m;
    }
    public override bool getMini()
    {
        return mini;
    }

    public override bool getReversed()
    {
        return reversed;
    }
    public override bool isDead()
    {
        return dead;
    }
    public override string getMode()
    {
        return "auto";
    }
}