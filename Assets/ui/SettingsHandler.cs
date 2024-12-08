using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsHandler : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    public Toggle fullscreen;
    private Resolution[] resolutions;

    // Start is called before the first frame update
    void Start()
    {
        // Set toggle value based on whether or not screen is currently fullscreen
        fullscreen.isOn = Screen.fullScreen;

        // Get list of available resolutions
        resolutions = Screen.resolutions;
        // Set drop down options based on acquired list of options
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentRes = 0;
        for (int i = 0; i != resolutions.Length; ++i) {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height) currentRes = i;
        }
        resolutionDropdown.AddOptions(options);
        // Ensure current resolution value is correct
        resolutionDropdown.value = currentRes;
        resolutionDropdown.RefreshShownValue();

        // Ensure current quality value is correct
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();
    }

    public void UpMusic()
    {
        musicSlider.value += 0.1f;
    }

    public void DownMusic()
    {
        musicSlider.value -= 0.1f; 
    }

    public void SetMusic()
    {
        float volume = Mathf.Log10(musicSlider.value)*20;
        mixer.SetFloat("musicVolume", volume);
    }

    public void UpSFX()
    {
        sfxSlider.value += 0.1f;
    }

    public void DownSFX()
    {
        sfxSlider.value -= 0.1f;
    }

    public void SetSFX()
    {
        float volume = Mathf.Log10(sfxSlider.value)*20;
        mixer.SetFloat("sfxVolume", volume);
    }

    public void SetQuality(int quality) {
        QualitySettings.SetQualityLevel(quality);
    }

    public void SetFullscreen(bool fullscreen) {
        Screen.fullScreen = fullscreen;
    }

    public void SetResolution(int resIndex) {
        Screen.SetResolution(resolutions[resIndex].width, resolutions[resIndex].height,Screen.fullScreen);
    }
}
