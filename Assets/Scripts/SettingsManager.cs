using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles settings for the game.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager current;

    private void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            current = this;
        }
        DontDestroyOnLoad(gameObject);
    }


    [Header("Audio")]
    public float musicVolume = 100.0f;
    public float soundEffectVolume = 100.0f;
}
