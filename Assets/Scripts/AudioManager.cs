using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager current;
    public AudioSource music;
    public AudioSource soundEffects;

    [Header("Music")]
    public AudioClip mainMenuMusic;
    public AudioClip inGameMusic;

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        GameManager.current.StartCoroutine(GameManager.current.Timer(x => music.Play(), 1f));
    }

    public void PlayMusic(string type)
    {
        if (music.isPlaying)
        {
            music.Stop();
        }
        if (type == "mainMenuMusic")
        {
            music.clip = mainMenuMusic;
            music.Play();
        }
        else if (type == "inGameMusic")
        {
            music.clip = inGameMusic;
            music.Play();
        }
    }

    public void PlaySound(AudioClip audioClip)
    {
        soundEffects.clip = audioClip;
        soundEffects.Play();
    }
}
