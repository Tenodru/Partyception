using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Grabs setting data and feeds it into SettingsManager.
/// </summary>
public class SettingsListener : MonoBehaviour
{
    [Header("Audio")]
    public float musicVolume = 100.0f;
    public float soundEffectVolume = 100.0f;

    [SerializeField] GameObject settingsPanel;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider soundEffectSlider;

    public void Start()
    {
        settingsPanel.SetActive(false);
        musicVolume = SettingsManager.current.musicVolume;
        soundEffectVolume = SettingsManager.current.soundEffectVolume;

        musicSlider.value = musicVolume;
        soundEffectSlider.value = soundEffectVolume;
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

        SettingsManager.current.musicVolume = musicVolume;
        SettingsManager.current.soundEffectVolume = soundEffectVolume;

        AudioManager.current.music.volume = musicVolume / 100;
        AudioManager.current.soundEffects.volume = soundEffectVolume / 100;
    }
}
