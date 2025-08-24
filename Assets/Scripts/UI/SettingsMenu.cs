using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputManagerSO inputManager;

    [Header("Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle screen;

    private void OnEnable()
    {
        inputManager.OnAction += Action;
    }

    private void Action()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            Toggle toggle = EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>();
            if (toggle != null)
            {
                toggle.isOn = !toggle.isOn;
            }
        }
    }
    public void FullScreen()
    {
        Screen.fullScreen = screen.isOn;
    }

    public void ChangeMusic()
    {
        if (musicSlider.value == 0)
            audioMixer.SetFloat("Music", -80f);
        else
            audioMixer.SetFloat("Music", Mathf.Log10(musicSlider.value) * 20);
    }

    public void ChangeSFX()
    {
        if (sfxSlider.value == 0)
            audioMixer.SetFloat("SFX", -80f);
        else
            audioMixer.SetFloat("SFX", Mathf.Log10(sfxSlider.value) * 20);
    }

    private void OnDisable()
    {
        inputManager.OnAction -= Action;
    }
}
