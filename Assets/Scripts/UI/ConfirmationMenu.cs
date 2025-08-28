using UnityEngine;
using UnityEngine.EventSystems;

public class ConfirmationMenu : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputManagerSO inputManager;

    [Header("References")]
    [SerializeField] private GameObject confirmationMenu;

    [Header("Buttons")]
    [SerializeField] private GameObject buttonAccept;
    [SerializeField] private GameObject buttonPause;

    private Map map;
    private Node node;
    private bool isActive;

    private GameObject uiObject;

    private void OnEnable()
    {
        inputManager.OnCancel += Cancel;
    }

    private void OnDisable()
    {
        inputManager.OnCancel -= Cancel;
    }

    private void SelectButton()
    {
        EventSystem.current.SetSelectedGameObject(uiObject);
    }

    public void Initialize(Map map, Node node)
    {
        this.map = map;
        this.node = node;
        map.InputLocked = true;
        uiObject = buttonAccept;
        SelectButton();
        confirmationMenu.SetActive(true);
    }

    public void Accept()
    {
        map.ConfirmNodeSelection(node);
        map.InputLocked = false;
        confirmationMenu.SetActive(false);
    }

    public void Cancel()
    {
        node = null;
        map.InputLocked = false;
        confirmationMenu.SetActive(false);
    }
    public void HideForPause()
    {
        if (confirmationMenu.activeSelf)
        {
            isActive = true;
            uiObject = buttonPause;
            SelectButton();
            confirmationMenu.SetActive(false);
        }
    }

    public void ShowAfterPause()
    {
        if (isActive)
        {
            uiObject = buttonAccept;
            SelectButton();
            confirmationMenu.SetActive(true);
            map.InputLocked = true;
            isActive = false;
        }
    }
}
