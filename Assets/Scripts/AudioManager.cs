using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource _source;

    public void Collect()
    {
        Play(AudioAction.Collect, ref _source);
    }
    public void Click()
    {
        Play(AudioAction.Click, ref _source);
    }
    public static void Play(AudioAction action, ref AudioSource source)
    {
        AudioClip clip;
        if (action == AudioAction.Jump)
        {
            clip = Resources.Load<AudioClip>("Jump");
            source.Stop();
            source.volume = .4f;
            source.PlayOneShot(clip);
            return;
        }
        if (action == AudioAction.Collect)
        {
            clip = Resources.Load<AudioClip>("Collect");
            source.Stop();
            source.volume = .7f;
            source.PlayOneShot(clip);
            return;
        }
        if (action == AudioAction.Click)
        {
            clip = Resources.Load<AudioClip>("Click");
            source.Stop();
            source.volume = .4f;
            source.PlayOneShot(clip);
            return;
        }
        if (action == AudioAction.Win)
        {
            clip = Resources.Load<AudioClip>("Win");
            source.Stop();
            source.volume = .4f;
            source.PlayOneShot(clip);
            return;
        }
    }
}
