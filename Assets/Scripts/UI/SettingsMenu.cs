using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle screen;

    private void Start()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
            LoadMusic();
        else
            ChangeMusic();

        if (PlayerPrefs.HasKey("SFXVolume"))
            LoadSFX();
        else
            ChangeSFX();

        if (PlayerPrefs.HasKey("Screen"))
            LoadScreen();
        else
            FullScreen();
    }

    public void FullScreen()
    {
        Screen.fullScreen = screen.isOn;
        if (screen.isOn == false)
            PlayerPrefs.SetInt("Screen", 1);
        else
            PlayerPrefs.SetInt("Screen", 0);
    }

    public void ChangeMusic()
    {
        float volume = musicSlider.value;
        if (musicSlider.value == 0)
            audioMixer.SetFloat("Music", -80f);   
        else
            audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);   
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void ChangeSFX()
    {
        float volume = sfxSlider.value;
        if (sfxSlider.value == 0)
            audioMixer.SetFloat("SFX", -80f);  
        else
            audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);  
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void LoadMusic()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        ChangeMusic();
    }

    private void LoadSFX()
    {
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        ChangeSFX();
    }

    private void LoadScreen()
    {
        if (PlayerPrefs.GetInt("Screen") == 1)
            screen.isOn = false;
        else
            screen.isOn = true;
        FullScreen();
    }
}
