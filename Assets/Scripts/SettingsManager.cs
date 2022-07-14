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

    [SerializeField] GameObject settingsPanel;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider soundEffectSlider;

    public void Start()
    {
        settingsPanel.SetActive(false);
        AudioManager.current.music.volume = musicVolume;
        AudioManager.current.soundEffects.volume = soundEffectVolume;
    }

    /// <summary>
    /// Toggles the settings panel on or off.
    /// </summary>
    public void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void AdjustSettings()
    {
        musicVolume = musicSlider.value;
        soundEffectVolume = soundEffectSlider.value;
        AudioManager.current.music.volume = musicVolume/100;
        AudioManager.current.soundEffects.volume = soundEffectVolume/100;
    }
}
