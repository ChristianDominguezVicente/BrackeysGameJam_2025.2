using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConfirmationMenu : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputManagerSO inputManager;

    [Header("References")]
    [SerializeField] private GameObject confirmationMenu;
    [SerializeField] private Image nodeImage;
    [SerializeField] private Sprite humanSprite;
    [SerializeField] private Sprite noHumanSprite;
    [SerializeField] private Image confirmationSprite;

    [Header("Buttons")]
    [SerializeField] private GameObject buttonAccept;
    [SerializeField] private GameObject buttonPause;

    [Header("Sounds")]
    [SerializeField] private AudioClip backSound;

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSFX;

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

        switch (node.RoomDifficulty)
        {
            case Node.Difficulty.Human:
                nodeImage.sprite = humanSprite;
                break;
            case Node.Difficulty.NoHuman:
                nodeImage.sprite = noHumanSprite;
                break;
        }

        uiObject = buttonAccept;
        SelectButton();
        confirmationSprite.sprite = Player.pj.Images[2];
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
        audioSFX.PlayOneShot(backSound);
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
