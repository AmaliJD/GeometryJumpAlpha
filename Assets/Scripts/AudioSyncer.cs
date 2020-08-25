using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSyncer : MonoBehaviour
{
    private float previousAudioValue;
    private float audioValue;
    private float timer;

    public float bias, timeStep, timeToBeat, restSmoothTime;

    protected bool isBeat;

    // Update is called once per frame
    void Update()
    {
        OnUpdate();
    }

    public virtual void OnUpdate()
    {
        previousAudioValue = audioValue;
        audioValue = AudioSpectrum.spectrumValue;

        if(previousAudioValue > bias &&
            audioValue <= bias)
        {
            if(timer > timeStep)
            {
                OnBeat();
            }
        }

        if (previousAudioValue <= bias &&
            audioValue > bias)
        {
            if (timer > timeStep)
            {
                OnBeat();
            }
        }

        timer += Time.deltaTime;
    }

    public virtual void OnBeat()
    {
        //Debug.Log("beat");
        timer = 0;
        isBeat = true;
    }
}
