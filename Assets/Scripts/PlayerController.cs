using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerController : MonoBehaviour
{
    public AudioSource death_sfx;
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
    public abstract void setBGMusic(AudioSource audio);
    public abstract void setRepawnSpeed(float s);
    public abstract void setSpeed(float s);
    public abstract float getSpeed();
    public abstract void resetColliders();
    public abstract void setColliders();
    public abstract void setRestartMusicOnDeath(bool r);
    public abstract void setAnimation();
    public abstract void setIcons(GameObject i);
    public abstract void setAble(bool a);
    public abstract string getMode();
    public abstract bool getMini();
    public abstract bool getReversed();
    public abstract bool isDead();
    public abstract void setVariables(bool j, bool r, bool m);
}
