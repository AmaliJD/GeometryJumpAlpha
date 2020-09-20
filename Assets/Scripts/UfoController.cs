﻿using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class UfoController : PlayerController
{
    public Collider2D player_collider;
    public CircleCollider2D ufo_collider;

    public TrailRenderer trail;
    public GameObject ufo;

    private float jumpForce = 12.5f;
    private float posJump;

    private float moveX, grav_scale;
    private float smoothing;

    private float maxSpeed = 17f;

    public override void Awake2()
    {
        speed = getSpeed();
        moveX = speed;
        smoothing = .05f;
        v_Velocity = Vector3.zero;
        posJump = jumpForce;
        player_collider = GetComponent<Collider2D>();

        grav_scale = player_body.gravityScale;

        ufo_collider.enabled = false;
        setRespawn(transform.position, reversed, mini);
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
        player_body.freezeRotation = true;
        player_body.gravityScale = 5f;
        upright = true;

        if (reversed) { player_body.gravityScale *= -1; }
        grav_scale = player_body.gravityScale;

        Vector3 newAngle = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.Euler(newAngle);

        ChangeSize();

        icon.transform.localScale = new Vector3(.65f, .65f, 1f);
        icon.transform.localPosition = new Vector3(0, .2f, 0);
        ufo.SetActive(true);
        icon.SetActive(true);

        eyes = GameObject.Find("Icon_Eyes");
        trail.transform.localPosition = new Vector3(0, 0, 0);

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
            jumpForce = 8f;
        }
        else
        {
            transform.localScale = new Vector2(1f, 1f);
            jumpForce = 12.5f;
        }

        posJump = jumpForce;
    }

    void Update()
    {
        if (able)
        {
            // CHECK IF DEAD
            dead = /*Physics2D.IsTouchingLayers(player_collider, deathLayer) || */Physics2D.IsTouchingLayers(ufo_collider, deathLayer);
            //grounded = Physics2D.Raycast(player_body.transform.position, Vector2.down, .51f, groundLayer);

            // CHECK IF GROUNDED
            if (reversed)
            {
                grounded = /*Physics2D.BoxCast(player_body.transform.position, new Vector2(.95f, .1f), 0f, Vector2.up, .51f, groundLayer) &&*/ checkGrounded
                        && (Physics2D.IsTouchingLayers(player_collider, groundLayer) || Physics2D.IsTouchingLayers(ufo_collider, groundLayer));
                regate = -1;
            }
            else
            {//.9
                grounded = /*Physics2D.BoxCast(player_body.transform.position, new Vector2(.95f, .1f), 0f, Vector2.down, .51f, groundLayer) &&*/ checkGrounded
                        && (Physics2D.IsTouchingLayers(player_collider, groundLayer) || Physics2D.IsTouchingLayers(ufo_collider, groundLayer));
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
            moveX = Input.GetAxisRaw("Horizontal") * speed;

            // JUMP!
            if (Input.GetButtonDown("Jump") || Input.GetKeyDown("space"))
            {
                if (!grounded || yellow || pink || red || green || blue || black)
                {
                    isjumping = true;
                }

                jump = true;
            }

            // RELEASE JUMP
            if (Input.GetButtonUp("Jump") || Input.GetKeyUp("space"))
            {
                isjumping = false;
                jump = false;
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
        if (!reversed && !upright)
        {
            // ... flip the player.
            negate = 1;
            upright = !upright;
            Flip();
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (reversed && upright)
        {
            // ... flip the player.
            negate = -1;
            upright = !upright;
            Flip();
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
        Jump();     // check if jumping
        Pad();      // check if hit pad
        Portal();   // check if on portal

        // IF GROUNDED --> TURN OFF TRAIL
        trail.emitting = true;
        if (grounded && Mathf.Abs(player_body.velocity.y) <= 0.01 && (!red_p && !yellow_p && !blue_p && !pink_p))
        {
            trail.emitting = false;
        }
    }

    //private bool resetRotation = true;

    public void Rotate()
    {
        float norm = 15f, rev = 5f;
        if (reversed) { norm = 5f; rev = 10f; }

        if((Mathf.Abs(player_body.velocity.x) < 0.2f) || grounded)
        {
            if (transform.rotation.z > 0 && transform.rotation.z <= 180)
            {
                Vector3 newAngle = new Vector3(0, 0, transform.rotation.z - 1);
                transform.rotation = Quaternion.Euler(newAngle);
            }
            else if (transform.rotation.z < 360 && transform.rotation.z > 180)
            {
                Vector3 newAngle = new Vector3(0, 0, transform.rotation.z + 1);
                transform.rotation = Quaternion.Euler(newAngle);
            }
        }
        else if(player_body.velocity.x > 0)
        {
            if (player_body.velocity.y >= 0)
            {
                Vector3 newAngle = new Vector3(0, 0, 360f - (Mathf.Abs(player_body.velocity.y) * (norm / maxSpeed)));
                transform.rotation = Quaternion.Euler(newAngle);
            }
            else
            {
                Vector3 newAngle = new Vector3(0, 0, (Mathf.Abs(player_body.velocity.y) * (rev / maxSpeed)));
                transform.rotation = Quaternion.Euler(newAngle);
            }
        }
        else if (player_body.velocity.x < 0)
        {
            if (player_body.velocity.y >= 0)
            {
                Vector3 newAngle = new Vector3(0, 0, (Mathf.Abs(player_body.velocity.y) * (norm / maxSpeed)));
                transform.rotation = Quaternion.Euler(newAngle);
            }
            else
            {
                Vector3 newAngle = new Vector3(0, 0, 360f - (Mathf.Abs(player_body.velocity.y) * (rev / maxSpeed)));
                transform.rotation = Quaternion.Euler(newAngle);
            }
        }
    }

    public void Eyes()
    {
        int rev = 1;
        if (reversed) { rev = -1; }
        eyes.transform.localPosition = Vector3.Lerp(eyes.transform.localPosition, new Vector3(rev * (moveX / 800), 0 * rev * (player_body.velocity.y / 80), 0), .4f);
    }

    public override void Jump()
    {
        if (yellow && jump)
        {
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(true);

            jump = false;
            yellow = false;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * 1.3f);
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

            jump = false;
            pink = false;
            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * .6f);

            if (grav) { grav = false; }
            if (gravN) { gravN = false; }
        }
        else if (red && jump)
        {
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(true);

            jump = false;
            red = false;
            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * 1.6f);

            if (grav) { grav = false; }
            if (gravN) { gravN = false; }
        }
        else if (blue && jump)
        {
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(true);

            jump = false;
            blue = false;
            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * .4f);
            reversed = !reversed;
            player_body.gravityScale *= -1;
            grav_scale *= -1;

            if (grav) { grav = false; }
            if (gravN) { gravN = false; }
        }
        else if (green && jump)
        {
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(true);

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
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * 1.3f);
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

            black = false;
            jump = false;
            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, -jumpForce * 1.2f);
        }
        else if (jump)
        {
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(true);

            //trail.emitting = false;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce);

            jump = false;
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
            //yellow_p = false;
            checkGrounded = false;
            grounded = false;

            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(true);

            //animator.SetBool("Orb", true);
            //jump = false;
            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * 1.2f);
            yellow_p = false;

            checkGrounded = true;
        }
        else if (pink_p)
        {
            checkGrounded = false;
            grounded = false;

            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(true);

            //jump = false;
            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * .8f);
            pink_p = false;

            checkGrounded = true;
        }
        else if (red_p)
        {
            checkGrounded = false;
            grounded = false;

            eyes.transform.Find("Eyes_Squint").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Normal").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Irked").gameObject.SetActive(false);
            eyes.transform.Find("Eyes_Wide").gameObject.SetActive(true);

            trail.emitting = true;
            player_body.velocity = new Vector2(player_body.velocity.x, jumpForce * 1.5f);
            red_p = false;

            checkGrounded = true;
        }
        else if (blue_p)
        {

            checkGrounded = false;
            blue_p = false;
            grounded = false;

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
                reversed = true;
                jumpForce = -posJump;
                trail.emitting = true;
                if (Mathf.Abs(player_body.velocity.y) > maxSpeed * .6f)
                {
                    player_body.velocity = new Vector2(player_body.velocity.x, player_body.velocity.y * .6f);
                }
                else
                {
                    player_body.velocity = new Vector2(player_body.velocity.x, player_body.velocity.y * .8f);
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
                reversed = false;
                jumpForce = posJump;
                trail.emitting = true;
                if (Mathf.Abs(player_body.velocity.y) > maxSpeed * .6f)
                {
                    player_body.velocity = new Vector2(player_body.velocity.x, player_body.velocity.y * .5f);
                }
                else
                {
                    player_body.velocity = new Vector2(player_body.velocity.x, player_body.velocity.y * .75f);
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

    public override void Flip()
    {
        // Switch the way the player is labelled as facing.
        //upright = !upright;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.y *= -1;
        transform.localScale = theScale;
    }

    public override void Respawn()
    {
        able = false;
        if (restartmusic) { bgmusic.Stop(); }
        player_collider.enabled = false;
        ufo_collider.enabled = false;
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
        player_collider.enabled = false;
        speed = respawn_speed;
        player_collider.enabled = true;
        ufo_collider.enabled = true;
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
        reversed = false; jump = false; yellow = false; pink = false; red = false; green = false; blue = false; black = false; teleA = false;
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
        ufo.SetActive(false);

        player_collider.isTrigger = false;
        player_collider.enabled = true;
        ufo_collider.enabled = false;
    }
    public override void setColliders()
    {
        //player_collider.enabled = false;
        player_collider.isTrigger = true;
        ufo_collider.enabled = true;
    }

    public override string getMode()
    {
        return "ufo";
    }
}