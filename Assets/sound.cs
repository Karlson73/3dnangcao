using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages audio volume settings only.
/// - Volume is controlled by a UI Slider
/// - Volume updates immediately when slider changes
/// - Save button stores volume using PlayerPrefs
/// </summary>
public class AudioOnlySettings : MonoBehaviour
{
    [Header("Audio UI")]
    public Slider volumeSlider;
    public TMP_Text volumePercentText;

    private const string VOLUME_KEY = "MASTER_VOLUME";

    void Start()
    {
        LoadVolume();
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    // ================= AUDIO =================

    void LoadVolume()
    {
        float volume = PlayerPrefs.GetFloat(VOLUME_KEY, 1f);
        AudioListener.volume = volume;
        volumeSlider.value = volume;
        UpdateVolumeText(volume);
    }

    void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        UpdateVolumeText(value);
    }

    void UpdateVolumeText(float value)
    {
        int percent = Mathf.RoundToInt(value * 100f);
        volumePercentText.text = percent + "%";
    }

    // ================= SAVE =================

    /// <summary>
    /// Called by Save Button
    /// Saves audio volume only
    /// </summary>
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(VOLUME_KEY, volumeSlider.value);
        PlayerPrefs.Save();
    }
}