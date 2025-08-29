using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class HighlghtSelect : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, ISelectHandler, ISubmitHandler
{
    [Header("Sounds")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSFX;

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);

        audioSFX.PlayOneShot(hoverSound);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        audioSFX.PlayOneShot(clickSound);
    }

    public void OnSelect(BaseEventData eventData)
    {
        audioSFX.PlayOneShot(hoverSound);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        audioSFX.PlayOneShot(clickSound);
    }
}
