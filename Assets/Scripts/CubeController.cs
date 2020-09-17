using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CubeController : PlayerController
{
    //  reference velocity
    private Vector3 v_Velocity;

    private Rigidbody2D player_body;
    public Collider2D player_collider, crouch_collider;

    private AudioSource bgmusic;

    public LayerMask groundLayer;
    public LayerMask deathLayer;
    public TrailRenderer trail;

    public Transform HEIGHT;
    private GameObject eyes;
    private GameObject icon;
    public GameObject jetpack;
    public GameObject ship;
    public GameObject ufo;
    public GameObject wave;
    public GameObject ball;
    public GameObject spider;

    public ParticleSystem death_particles;
    public GameObject player_renderer;

    private float jumpForce = 21f;
    private float speed, speed0 = 40f, speed1 = 55f, speed2 = 75f, speed3 = 90f, speed4 = 110f, respawn_speed;
    private float posJump, negate = 1, regate = 1;

    private float moveX, grav_scale, prev_velocity = 0;
    private float smoothing;

    private float time = 0;
    private bool grounded = false, prev_grounded = true, fromGround = false, reversed = false, jump = false, downjump = false, released = false, checkGrounded = true;
    private bool yellow = false, blue = false, red = false, pink = false, green = false, black = false;
    private bool yellow_p = false, blue_p = false, red_p = false, pink_p = false;
    private bool grav = false, gravN = false, teleA = false;
    private Vector3 teleB, respawn;
    private bool respawn_rev = false, respawn_mini = false;
    private bool crouch = false, crouched = false, facingRight = true, dead = false, able = true;

    private bool isjumping = false;

    private float maxSpeed = 110f;

    private bool mini = false;

    void Awake()
    {
        speed = speed1;
        moveX = 0;
        smoothing = .05f;
        v_Velocity = Vector3.zero;
        posJump = jumpForce;
        player_body = GetComponent<Rigidbody2D>();
        player_collider = GetComponent<Collider2D>();

        grav_scale = player_body.gravityScale;

        crouch_collider.enabled = false;
        setRespawn(transform.position, reversed, mini);
        setRepawnSpeed(1f);

        eyes = GameObject.Find("Icon_Eyes");
        setAnimation();
    }

    private void Start()
    {
        bgmusic.Play();
    }

    public override void setAnimation()
    {
        player_body.freezeRotation = true;
        player_body.gravityScale = 9.81f;
        if (reversed) { player_body.gravityScale *= -1; }
        grav_scale = player_body.gravityScale;

        icon.transform.localScale = new Vector3(1f, 1f, 1f);
        icon.transform.localPosition = new Vector3(0, 0, 0);
        jetpack.SetActive(false);
        ship.SetActive(false);
        ufo.SetActive(false);
        ball.SetActive(false);
        spider.SetActive(false);
        wave.SetActive(false);
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
            transform.localScale = new Vector2(1.05f, 1.05f);
            jumpForce = 21f;
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
            dead = Physics2D.IsTouchingLayers(player_collider, deathLayer) || Physics2D.IsTouchingLayers(crouch_collider, deathLayer);
            //grounded = Physics2D.Raycast(player_body.transform.position, Vector2.down, .51f, groundLayer);

            // CHECK IF GROUNDED
            if (reversed)
            {
                grounded = Physics2D.BoxCast(player_body.transform.position, new Vector2(.95f, .1f), 0f, Vector2.up, .51f, groundLayer) && checkGrounded
                        && (Physics2D.IsTouchingLayers(player_collider, groundLayer) || Physics2D.IsTouchingLayers(crouch_collider, groundLayer));
                regate = -1;
            }
            else
            {//.9
                grounded = Physics2D.BoxCast(player_body.transform.position, new Vector2(.95f, .1f), 0f, Vector2.down, .51f, groundLayer) && checkGrounded
                        && (Physics2D.IsTouchingLayers(player_collider, groundLayer) || Physics2D.IsTouchingLayers(crouch_collider, groundLayer));
                regate = 1;
            }

            Debug.Log("Grounded: " + grounded);

            // IF GROUNDED --> TURN OFF TRAIL
            if (grounded)
            {
                trail.emitting = false;
                eyes.transform.Find("Eyes_Normal").gameObject.SetActive(true);
                eyes.transform.Find("Eyes_Wide").gameObject.SetActive(false);
                eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
                eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);

                if (!prev_grounded && prev_velocity > 13)
                {
                    HEIGHT.GetComponent<Animator>().ResetTrigger("Crouch");
                    HEIGHT.GetComponent<Animator>().ResetTrigger("Default");
                    HEIGHT.GetComponent<Animator>().ResetTrigger("Stretch");
                    HEIGHT.GetComponent<Animator>().SetTrigger("Squash");
                }
            }

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
            moveX = Input.GetAxisRaw("Horizontal") * speed;

            

            // JUMP!
            if (Input.GetButtonDown("Jump") || Input.GetKeyDown("space"))
            {
                jump = true;
                released = false;
                fromGround = ((grounded || time < .07f) && jump);

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
                released = true;
            }

            // CROUCH
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                crouch = true;
            }
            else
            {
                crouch = false;
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
            prev_velocity = Mathf.Abs(player_body.velocity.y);

            time += Time.deltaTime;
            if(prev_grounded) { time = 0; }
        }
    }

    void FixedUpdate()
    {
        // one job and one job only. MOVE
        if(able)
        {
            Move();
        }
    }

    public override void Move()
    {
        // If the input is moving the player right and the player is facing left...
        if ((!reversed && moveX > 0 && !facingRight) || (reversed && moveX < 0 && !facingRight && (fromGround || grounded)))
        {
            // ... flip the player.
            negate = 1;
            facingRight = !facingRight;
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if ((!reversed && moveX < 0 && facingRight) || (reversed && moveX > 0 && facingRight && (fromGround || grounded)))
        {
            // ... flip the player.
            negate = -1;
            facingRight = !facingRight;
        }

        // if crouching, change movement controls
        if (crouch && grounded)
        {
            if (!crouched)
            {
                HEIGHT.GetComponent<Animator>().ResetTrigger("Default");
                HEIGHT.GetComponent<Animator>().ResetTrigger("Squash");
                HEIGHT.GetComponent<Animator>().ResetTrigger("Stretch");
                HEIGHT.GetComponent<Animator>().SetTrigger("Crouch");
            }

            moveX = 0;
            crouch_collider.enabled = true;
            player_collider.enabled = false;
            Vector2 targetVelocity = new Vector2(moveX * Time.fixedDeltaTime * 10f, player_body.velocity.y);

            if (!crouched)
            {
                player_body.velocity = Vector3.SmoothDamp(player_body.velocity * 1.6f, targetVelocity, ref v_Velocity, smoothing * 7f);
                crouched = true;
            }
            else
            {
                player_body.velocity = Vector3.SmoothDamp(player_body.velocity, targetVelocity, ref v_Velocity, smoothing * 7f);
            }

            //crouched = true;
            //player_body.velocity = Vector3.SmoothDamp(player_body.velocity, targetVelocity, ref v_Velocity, smoothing * 7);
            //speed 55      *7      *1      *1.5
        }
        else
        {
            if (crouched)
            {
                crouched = false;
                HEIGHT.GetComponent<Animator>().ResetTrigger("Crouch");
                HEIGHT.GetComponent<Animator>().ResetTrigger("Squash");
                HEIGHT.GetComponent<Animator>().ResetTrigger("Stretch");
                HEIGHT.GetComponent<Animator>().SetTrigger("Default");
            }
            player_collider.enabled = true;
            crouch_collider.enabled = false;
            Vector2 targetVelocity = new Vector2(moveX * Time.fixedDeltaTime * 10f, player_body.velocity.y);

            if (Mathf.Abs(targetVelocity.x) > Mathf.Abs(player_body.velocity.x) || !grounded)
            {
                player_body.velocity = Vector3.SmoothDamp(player_body.velocity, targetVelocity, ref v_Velocity, smoothing * .7f);
            }
            else
            {
                player_body.velocity = Vector3.SmoothDamp(player_body.velocity, targetVelocity, ref v_Velocity, smoothing * 1.2f); //1.5
            }
        }

        Eyes();
        Jump();     // check if jumping
        Pad();      // check if hit pad
        Portal();   // check if on portal
    }

    public void Eyes()
    {
        int rev = 1;
        if (reversed) { rev = -1; }
        eyes.transform.localPosition = Vector3.Lerp(eyes.transform.localPosition, new Vector3(rev * (moveX/800), 0 * rev * (player_body.velocity.y/200), 0), .4f);
    }

    public override void Jump()
    {
        if (yellow && jump)
        {
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(true);

            fromGround = false;
            released = false;
            jump = false;
            yellow = false;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * 1.1f);
            trail.emitting = true;
            StartCoroutine(RotateArc(Vector3.forward, negate * -30.0f, 0.5f));

            if (grav) { grav = false; }
            if (gravN) { gravN = false; }
        }
        else if (pink && jump)
        {
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(true);

            fromGround = false;
            released = false;
            jump = false;
            pink = false;
            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * .85f);
            StartCoroutine(RotateArc(Vector3.forward, negate * -25.0f, 0.5f));

            if (grav) { grav = false; }
            if (gravN) { gravN = false; }
        }
        else if (red && jump)
        {
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(true);

            fromGround = false;
            released = false;
            jump = false;
            red = false;
            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * 1.35f);

            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > .6)
            {
                StartCoroutine(RotateAround(Vector3.forward, negate * -360.0f, 0.5f));
            }
            else
            {
                StartCoroutine(RotateArc(Vector3.forward, negate * -40.0f, 0.5f));
            }

            if (grav) { grav = false; }
            if (gravN) { gravN = false; }
        }
        else if (blue && jump)
        {
            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(false);
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
            StartCoroutine(RotateAround(Vector3.forward, regate * negate * -180.0f, 0.4f));

            if (grav) { grav = false; }
            if (gravN) { gravN = false; }
        }
        else if (green && jump)
        {
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(false);
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
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce);
            player_body.gravityScale *= -1;
            grav_scale *= -1;

            StartCoroutine(RotateAround(Vector3.forward, regate * negate * 180.0f, 0.5f));

            if (grav) { grav = false; }
            if (gravN) { gravN = false; }
        }
        else if (black && jump)
        {
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(true);

            fromGround = false;
            black = false;
            released = false;
            jump = true;
            downjump = true;
            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, -jumpForce * 1.1f);
        }
        else if ((grounded || time < .07f) && jump && downjump)
        {
            time = 1;
            HEIGHT.GetComponent<Animator>().ResetTrigger("Default");
            HEIGHT.GetComponent<Animator>().ResetTrigger("Squash");
            HEIGHT.GetComponent<Animator>().SetTrigger("Stretch");
            trail.emitting = false;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce);
            grounded = false;

            /* fuck me
            if (Mathf.Abs(moveX) > 0)
            {
                checkGrounded = false;
                StartCoroutine(RotateArc(Vector3.forward, negate * -10.0f, 0.2f));
                checkGrounded = true;
            }//*/

            fromGround = true;
            jump = false;
            downjump = false;
        }
        else if (fromGround && ((!reversed && released && player_body.velocity.y > 0) || (reversed && released && player_body.velocity.y < 0)))
        {
            HEIGHT.GetComponent<Animator>().ResetTrigger("Crouch");
            HEIGHT.GetComponent<Animator>().ResetTrigger("Squash");
            HEIGHT.GetComponent<Animator>().ResetTrigger("Stretch");
            HEIGHT.GetComponent<Animator>().SetTrigger("Default");
            player_body.velocity /= 2;
            released = false;
            fromGround = false;
        }
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
            yellow_p = false;
            checkGrounded = false;
            grounded = false;

            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(true);

            fromGround = false;
            released = false;
            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * 1.4f);

            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > .6)
            {
                StartCoroutine(RotateAround(Vector3.forward, negate * -360.0f, .7f));
            }
            else
            {
                StartCoroutine(RotateArc(Vector3.forward, negate * -10.0f, 0.5f));
            }
            checkGrounded = true;
        }
        else if (pink_p)
        {
            pink_p = false;
            checkGrounded = false;
            grounded = false;

            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(true);

            fromGround = false;
            released = false;
            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * .8f);

            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > .6)
            {
                StartCoroutine(RotateArc(Vector3.forward, negate * -45f, 0.7f));
            }
            else
            {
                StartCoroutine(RotateArc(Vector3.forward, negate * -5.0f, 0.5f));
            }
            checkGrounded = true;
        }
        else if (red_p)
        {
            red_p = false;
            checkGrounded = false;
            grounded = false;

            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(true);

            fromGround = false;
            released = false;
            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * 1.5f);
            grounded = false;

            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > .6)
            {
                StartCoroutine(RotateAround(Vector3.forward, negate * -360.0f, .7f));
            }
            else
            {
                StartCoroutine(RotateArc(Vector3.forward, negate * -45.0f, .5f));
            }
            checkGrounded = true;
        }
        else if (blue_p)
        {
            blue_p = false;
            checkGrounded = false;
            grounded = false;
            fromGround = false;
            released = false;

            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(true);

            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * .4f);
            reversed = !reversed;
            player_body.gravityScale *= -1;
            grav_scale *= -1;

            StartCoroutine(RotateAround(Vector3.forward, regate * negate * -180.0f, 0.4f));
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
                StartCoroutine(RotateAround(Vector3.forward, regate * negate * -180.0f, 0.5f));
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
                StartCoroutine(RotateAround(Vector3.forward, regate * negate * -180.0f, 0.5f));
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
    private IEnumerator RotateAround(Vector3 axis, float angle, float duration)
    {
        float elapsed = 0.0f;
        float rotated = 0.0f;
        while (elapsed < duration)
        {
            float step = angle / duration * Time.deltaTime;
            transform.RotateAround(transform.position, axis, step);
            elapsed += Time.deltaTime;
            rotated += step;

            if (reversed)
            {
                grounded = Physics2D.BoxCast(player_body.transform.position, new Vector2(.9f, .1f), 0f, Vector2.up, .59f, groundLayer) && checkGrounded;
            }
            else
            {
                grounded = Physics2D.BoxCast(player_body.transform.position, new Vector2(.9f, .1f), 0f, Vector2.down, .59f, groundLayer) && checkGrounded;
            }

            if (grounded)
            {
                transform.RotateAround(transform.position, axis, angle - rotated);
                yield break;
            }

            yield return null;
        }
        transform.RotateAround(transform.position, axis, angle - rotated);
    }

    private IEnumerator RotateArc(Vector3 axis, float angle, float duration)
    {
        float elapsed = 0.0f;
        float rotated = 0.0f;
        duration /= 2;

        while (elapsed < duration)
        {
            float step = angle / duration * Time.deltaTime;
            transform.RotateAround(transform.position, axis, step);
            elapsed += Time.deltaTime;
            rotated += step;


            if (reversed)
            {
                grounded = Physics2D.BoxCast(player_body.transform.position, new Vector2(.9f, .1f), 0f, Vector2.up, .59f, groundLayer) && checkGrounded;
            }
            else
            {
                grounded = Physics2D.BoxCast(player_body.transform.position, new Vector2(.9f, .1f), 0f, Vector2.down, .59f, groundLayer) && checkGrounded;
            }

            if (grounded)
            {
                transform.RotateAround(transform.position, axis, -rotated);
                yield break;
            }

            yield return null;
        }
        transform.RotateAround(transform.position, axis, angle - rotated);

        angle *= -1;
        elapsed = 0.0f;
        rotated = 0.0f;

        while (elapsed < duration)
        {
            float step = angle / duration * Time.deltaTime;
            transform.RotateAround(transform.position, axis, step);
            elapsed += Time.deltaTime;
            rotated += step;

            if (reversed)
            {
                grounded = Physics2D.BoxCast(player_body.transform.position, new Vector2(.9f, .1f), 0f, Vector2.up, 1f, groundLayer) && checkGrounded;
            }
            else
            {
                grounded = Physics2D.BoxCast(player_body.transform.position, new Vector2(.9f, .1f), 0f, Vector2.down, 1f, groundLayer) && checkGrounded;
            }

            if (grounded)
            {
                transform.RotateAround(transform.position, axis, angle - rotated);
                yield break;
            }

            yield return null;
        }
        transform.RotateAround(transform.position, axis, angle - rotated);
    }

    public override void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

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
            /*
            if (player_body.velocity.x > 0)
            {
                teleB.x -= .35f;
            }
            else if (player_body.velocity.x < 0)
            {
                teleB.x += .35f;
            }

            if (player_body.velocity.y < 0)
            {
                teleB.y += .35f;
            }
            else if (player_body.velocity.y > 0)
            {
                teleB.y -= .35f;
            }*/
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!enabled) { return; }
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
        crouch_collider.enabled = false;
        player_collider.enabled = false;
        StopAllCoroutines();
        player_body.velocity = Vector2.zero;
        trail.emitting = false;
        jump = false;
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

        //player_renderer.enabled = false;
        player_renderer.SetActive(false);
        //death_animation.GetComponent<SpriteRenderer>().enabled = true;
        death_particles.Play();
        death_sfx.PlayOneShot(death_sfx.clip, 1f);
        player_body.gravityScale = 0;

        Invoke("undead", 1);
    }

    public void undead()
    {
        speed = respawn_speed;
        //transform.position += respawn - transform.position;
        transform.position = respawn;

        crouch = false;
        HEIGHT.GetComponent<Animator>().ResetTrigger("Crouch");
        HEIGHT.GetComponent<Animator>().ResetTrigger("Squash");
        HEIGHT.GetComponent<Animator>().ResetTrigger("Stretch");
        HEIGHT.GetComponent<Animator>().SetTrigger("Default");
        HEIGHT.localPosition = new Vector3(0, 0, 0);

        player_renderer.SetActive(true);
        player_collider.enabled = true;
        player_body.gravityScale = grav_scale;

        //bgmusic.volume = 1;
        if (restartmusic) { bgmusic.Play(); }

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
        reversed = false;  yellow = false; pink = false; red = false; green = false; blue = false; black = false; teleA = false;
    }

    public override void setBGMusic(AudioSource audio)
    {
        bgmusic = audio;
    }

    public override void setRepawnSpeed(float s)
    {
        if(s == 0) { respawn_speed = speed0; }
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
        player_collider.enabled = false;
        crouch_collider.enabled = false;
    }

    public override void setColliders()
    {
        player_collider.enabled = true;
        crouch_collider.enabled = false;
        transform.rotation = new Quaternion(0, 0, 0, 0);
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
        return "cube";
    }
}