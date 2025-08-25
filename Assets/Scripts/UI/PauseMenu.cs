using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;

    [Header("Buttons")]
    [SerializeField] private GameObject buttonPause;
    [SerializeField] private GameObject buttonSettings;

    private Player player;

    private GameObject uiObject;
    private GameObject lastSelectedObject;

    private bool isPaused = false;

    public bool IsPaused { get => isPaused; set => isPaused = value; }
    public GameObject PauseGameObject { get => pauseMenu; }
    public GameObject SettingsGameObject { get => settingsMenu; }

    private void Start()
    {
        player = FindFirstObjectByType<Player>();

        uiObject = buttonPause;
        SelectButton();
    }


    private void SelectButton()
    {
        EventSystem.current.SetSelectedGameObject(uiObject);
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(lastSelectedObject);
        else
            lastSelectedObject = EventSystem.current.currentSelectedGameObject;
    }

    public void Resume()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Settings()
    {
        uiObject = buttonSettings;
        EventSystem.current.SetSelectedGameObject(uiObject);

        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void Exit()
    {
        player.ClearDeck();
        SceneManager.LoadScene("Menu");
    }

    public void Back()
    {
        uiObject = buttonPause;
        EventSystem.current.SetSelectedGameObject(uiObject);

        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
}
