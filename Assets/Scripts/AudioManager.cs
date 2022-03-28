using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager current;

    private void Start()
    {
        current = this;
    }

    public void PlaySound(AudioClip audioClip)
    {
        GetComponent<AudioSource>().clip = audioClip;
        GetComponent<AudioSource>().Play();
    }
}
