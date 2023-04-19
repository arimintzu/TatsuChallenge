using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource source;

    public void Play(AudioClip clip)
    {
        if (source) source.PlayOneShot(clip);
    }
}
