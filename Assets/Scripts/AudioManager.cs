using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource _source;
    public float baseVolume = .1f;
    public bool isMusic = true;
    public AudioAction currentAction = AudioAction.None;

    void Start()
    {
        ChangeVolume();
    }

    public void ChangeVolume()
    {
        float mod = PlayerPrefs.GetFloat("VolumeAmbience");
        if (isMusic)
        {
            mod = PlayerPrefs.GetFloat("VolumeMusic");
        }
        _source.volume = baseVolume * mod;
    }

    public void Collect()
    {
        PlayEffect(AudioAction.Collect, ref _source);
    }
    public void Click()
    {
        PlayEffect(AudioAction.Click, ref _source);
    }

    public static void PlayAmbience(AudioAction action, ref AudioSource source)
    {
        AudioClip clip;
        float mod = PlayerPrefs.GetFloat("VolumeAmbience");
        if (action == AudioAction.Hover)
        {
            clip = Resources.Load<AudioClip>("Whir");
            if (source.clip ==  clip && source.isPlaying)
            {
                source.pitch = 1.0f;
            }
            else
            {
                source.Stop();
                source.pitch = 1.0f;
                source.clip = clip;
                source.volume = .7f * mod;
                source.loop = true;
                source.Play();
            }
            return;
        }
        if (action == AudioAction.Drive)
        {
            clip = Resources.Load<AudioClip>("Whir");
            if (source.clip == clip && source.isPlaying)
            {
                source.pitch = .8f;
            }
            else
            {
                source.Stop();
                source.pitch = .8f;
                source.clip = clip;
                source.volume = .6f * mod;
                source.loop = true;
                source.Play();
            }
            return;
        }
        if (action == AudioAction.Idle)
        {
            clip = Resources.Load<AudioClip>("Whir");
            if (source.clip == clip && source.isPlaying)
            {
                source.pitch = .6f;
            }
            else
            {
                source.Stop();
                source.pitch = .6f;
                source.clip = clip;
                source.volume = .5f * mod;
                source.loop = true;
                source.Play();
            }
            return;
        }
    }

    public static void PlayEffect(AudioAction action, ref AudioSource source)
    {
        AudioClip clip;
        float mod = PlayerPrefs.GetFloat("VolumeEffects");
        if (action == AudioAction.Jump)
        {
            clip = Resources.Load<AudioClip>("Jump");
            source.Stop();
            source.volume = .8f * mod;
            source.PlayOneShot(clip);
            return;
        }
        if (action == AudioAction.Collect)
        {
            clip = Resources.Load<AudioClip>("Collect");
            source.Stop();
            source.volume = 1f * mod;
            source.PlayOneShot(clip);
            return;
        }
        if (action == AudioAction.Click)
        {
            clip = Resources.Load<AudioClip>("Click");
            source.Stop();
            source.volume = .8f * mod;
            source.PlayOneShot(clip);
            return;
        }
        if (action == AudioAction.Win)
        {
            clip = Resources.Load<AudioClip>("Win");
            source.Stop();
            source.volume = .8f * mod;
            source.PlayOneShot(clip);
            return;
        }
        if (action == AudioAction.Battery)
        {
            clip = Resources.Load<AudioClip>("BatteryPickup");
            source.Stop();
            source.volume = .15f * mod;
            source.PlayOneShot(clip);
            return;
        }
    }
}
