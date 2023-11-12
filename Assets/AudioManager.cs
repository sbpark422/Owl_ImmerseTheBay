using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public AudioSource audienceAudioSource;
    public AudioSource refAudioSource;

    public AudioClip cheer_Sound;
    public AudioClip DAMN_Sound;
    public float damMeter = 0f;
    public AudioClip[] audience_Sounds;
    public AudioClip[] out_Sounds;
    public AudioClip[] catch_Sounds;
    public AudioClip whistle_Sound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayCheer()
    {

        if (!audienceAudioSource.isPlaying)
        {
            int ran = (int)(UnityEngine.Random.Range(0f, (float)(audience_Sounds.Length)));
            audienceAudioSource.clip = audience_Sounds[ran];
            audienceAudioSource.volume = .85f;
            audienceAudioSource.priority = 200;
            audienceAudioSource.Play();
        }

    }
    public void PlayDamn()
    {

        audienceAudioSource.clip = DAMN_Sound;
        audienceAudioSource.priority = 200;
        audienceAudioSource.volume = 0.75f;
        audienceAudioSource.pitch = 1f + damMeter;
        audienceAudioSource.Play();
        if (damMeter < 1f)
        {
            damMeter += .2f;
        }


    }



    public void PlayOuts()
    {
        int ran = (int)(UnityEngine.Random.Range(0f, (float)(out_Sounds.Length)));
        refAudioSource.clip = out_Sounds[ran];
        refAudioSource.priority = 120;
        refAudioSource.volume = 0.75f;
        refAudioSource.Play();
    }


    public void PlayCatches()
    {
        int ran = (int)(UnityEngine.Random.Range(0f, (float)(catch_Sounds.Length)));
        refAudioSource.clip = catch_Sounds[ran];
        refAudioSource.priority = 120;
        refAudioSource.volume = 0.75f;
        refAudioSource.Play();
    }


    public void PlayWhistle()
    {
        if (!refAudioSource.isPlaying)
        {
            refAudioSource.volume = 0.25f;
            refAudioSource.priority = 150;
            refAudioSource.clip = whistle_Sound;
            refAudioSource.Play();
        }

    }
}
