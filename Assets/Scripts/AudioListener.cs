using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListener : MonoBehaviour
{
    public void PlaySound(AudioClip audioClip)
    {
        AudioManager.current.PlaySound(audioClip);
    }
}
