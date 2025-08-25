using UnityEngine;
using UnityEngine.EventSystems;
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

    private GameObject uiObject;
    private GameObject lastSelectedObject;

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
            Back();
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
        deckMenu.SetActive(false);
        credits.SetActive(false);
        beginMenu.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void OnDisable()
    {
        inputManager.OnCancel -= Cancel;
    }
}
