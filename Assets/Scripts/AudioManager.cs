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
        GameManager.current.StartCoroutine(GameManager.current.Timer(x => music.Play(), 0.5f));
        GameManager.current.StartCoroutine(GameManager.current.Timer(x => TransitionCanvas.current.Animate("BlackFadeIn"), 1.5f));
        if (GameObject.Find("OpeningAnim") != null)
        {
            GameManager.current.StartCoroutine(GameManager.current.Timer(x => GameObject.Find("OpeningAnim").SetActive(false), 2f));
        }
        GameManager.current.StartCoroutine(GameManager.current.Timer(x => TransitionCanvas.current.Animate("BlackFadeOut"), 2f));
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
