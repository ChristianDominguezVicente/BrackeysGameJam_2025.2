using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BeginMenu : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputManagerSO inputManager;

    [Header("Menus")]
    [SerializeField] private GameObject beginMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject deckMenu;
    [SerializeField] private GameObject credits;

    [Header("Buttons")]
    [SerializeField] private GameObject buttonBegin;
    [SerializeField] private GameObject buttonSettings;
    [SerializeField] private GameObject buttonDeck;
    [SerializeField] private GameObject buttonCredits;

    [Header("Dialogue Config")]
    [SerializeField, TextArea(1, 5)] private string[] deckDialogue;

    [Header("Sounds")]
    [SerializeField] private AudioClip backSound;

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSFX;

    [Header("Sprites")]
    [SerializeField] private Sprite[] sprites;

    private GameObject uiObject;
    private GameObject lastSelectedObject;

    private static bool deckDialogueShown = false;

    private void OnEnable()
    {
        inputManager.OnCancel += Cancel;

        uiObject = buttonBegin;
        SelectButton();
    }
    private void SelectButton()
    {
        EventSystem.current.SetSelectedGameObject(uiObject);
    }

    private void Cancel()
    {
        if (!beginMenu.activeSelf)
        {
            audioSFX.PlayOneShot(backSound);
            Back();
        }      
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(lastSelectedObject);
        else
            lastSelectedObject = EventSystem.current.currentSelectedGameObject;
    }

    public void DeckMenu()
    {
        uiObject = buttonDeck;
        EventSystem.current.SetSelectedGameObject(uiObject);

        beginMenu.SetActive(false);
        deckMenu.SetActive(true);

        if (!deckDialogueShown)
        {
            deckDialogueShown = true;

            SetMenuInteractuable(false);

            Sprite[] spritesShow = new Sprite[deckDialogue.Length];
            for (int i = 0; i < deckDialogue.Length; i++)
            {
                if (i < 2)
                    spritesShow[i] = sprites[1];
                else if (i < 5)
                    spritesShow[i] = sprites[2];
                else
                    spritesShow[i] = sprites[3];
            }

            DialogueSystem.ds.StartDialogue(deckDialogue, spritesShow, () => { SetMenuInteractuable(true); });
        }
    }

    private void SetMenuInteractuable(bool state)
    {
        CanvasGroup cg = deckMenu.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.interactable = state;
            cg.blocksRaycasts = state;
        }
    }

    public void Settings()
    {
        uiObject = buttonSettings;
        EventSystem.current.SetSelectedGameObject(uiObject);

        beginMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void Credits()
    {
        uiObject = buttonCredits;
        EventSystem.current.SetSelectedGameObject(uiObject);

        beginMenu.SetActive(false);
        credits.SetActive(true);
    }

    public void Back()
    {
        uiObject = buttonBegin;
        EventSystem.current.SetSelectedGameObject(uiObject);

        settingsMenu.SetActive(false);
        if (deckMenu != null)
            deckMenu.SetActive(false);
        credits.SetActive(false);
        beginMenu.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Return()
    {
        SceneManager.LoadScene("Menu");
    }

    private void OnDisable()
    {
        inputManager.OnCancel -= Cancel;
    }
}
