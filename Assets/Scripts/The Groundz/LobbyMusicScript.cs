using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMusicScript : MonoBehaviour
{
    public AudioSource themeAudioSource;

    public bool shuffle;
    public AudioClip[] lobbyMusicList;
    bool isPlaying;
    float vol;

    int index;

    bool isMuting;
    bool muted;
    

    void Start()
    {
        if (!themeAudioSource)
        {
            themeAudioSource = gameObject.GetComponent<AudioSource>();
        }

        if (!themeAudioSource.isPlaying)
        {
            if (lobbyMusicList.Length > 0)
            {
                themeAudioSource.clip = lobbyMusicList[0];
                themeAudioSource.Play();
            }
        }

        else
        {
            if (shuffle)
            {
                PlayRandom();
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
       if (!themeAudioSource.isPlaying && themeAudioSource.enabled)
        {
            if (shuffle)
            {
                PlayRandom();
            }
            else
            {
                PlayNext();
            }
        }

       if (isMuting)
        {
            LowerVolume(.01f,true);
        }
    }

    private void LowerVolume(float x, bool isTurnOff)
    {

       // print("Muting");

        vol = themeAudioSource.volume;

        //print("vol "+ vol);

        if (vol > 0)
        {
            themeAudioSource.volume -= x;
        }
        else
        {
            if (isTurnOff)
            {
                themeAudioSource.Stop();
                themeAudioSource.enabled = false;
                muted = true;
            }
        }

    }

    private void PlayRandom()
    {
        themeAudioSource.clip = lobbyMusicList[UnityEngine.Random.Range(0, lobbyMusicList.Length)] as AudioClip;
        themeAudioSource.Play();
    }

    private void PlayNext()
    {
       if (index+1 >= lobbyMusicList.Length)
        {
            index = 0;
        }
       else
        {
            index++;
        }

        themeAudioSource.clip = lobbyMusicList[index] as AudioClip;
        themeAudioSource.Play();
    }

    public void PauseMusic()
    {

    }

    public void SkipMusic()
    {

    }
    public void PrevMusic()
    {

    }

    public void EndTheme()
    {
        // isMuting = true;

        float decRate = .001f;

        vol = themeAudioSource.volume;

        //print("vol "+ vol);

        while (vol > 0)
        {
            themeAudioSource.volume -= decRate;
            vol = themeAudioSource.volume;
        }

        themeAudioSource.Stop();
        themeAudioSource.enabled = false;
        muted = true;
    }

    public void StartTheme()
    {
        isMuting = false;

        if (!themeAudioSource.isPlaying)
        {
            if (lobbyMusicList.Length > 0)
            {
                themeAudioSource.clip = lobbyMusicList[0];
                themeAudioSource.Play();
            }
        }

    }

    internal bool GetIsThemeOff()
    {
        return muted;
    }
}
