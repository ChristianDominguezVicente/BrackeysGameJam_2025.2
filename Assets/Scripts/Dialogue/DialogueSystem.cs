using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class DialogueSystem : MonoBehaviour, IPointerClickHandler
{
    public static DialogueSystem ds;

    [Header("UI")]
    [SerializeField] private GameObject dialogueFrame;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image dialogueSprite;

    [Header("Config")]
    [SerializeField] private float timeBtwLetter;

    [Header("Inputs")]
    [SerializeField] private InputManagerSO inputManager;

    [Header("Sounds")]
    [SerializeField] private AudioClip clickSound;

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSFX;

    private string[] phrases;
    private Sprite[] sprites;
    private int index;
    private bool talking;

    private System.Action onDialogueFinished;

    private bool active = false;

    public bool Active { get => active; set => active = value; }

    private void Awake()
    {
        if (ds == null) ds = this;
        else Destroy(gameObject);

        dialogueFrame.SetActive(false);
    }

    private void OnEnable()
    {
        inputManager.OnAction += OnActionPressed;
    }

    private void OnDisable()
    {
        inputManager.OnAction -= OnActionPressed;
    }

    public void StartDialogue(string[] phrases, Sprite[] sprites,System.Action onFinish = null)
    {
        this.phrases = phrases;
        this.sprites = sprites;
        this.index = -1;
        this.talking = false;
        this.onDialogueFinished = onFinish;

        dialogueFrame.SetActive(true);

        dialogueSprite.sprite = sprites[0];
        dialogueSprite.gameObject.SetActive(true);

        active = true;

        NextPhrase();
    }

    public void OnActionPressed()
    {
        if (phrases == null || phrases.Length == 0)
            return;

        if(dialogueFrame.activeSelf)
            audioSFX.PlayOneShot(clickSound);

        if (!talking)
        {
            NextPhrase();
        }
        else
        {
            CompletePhrase();
        }
    }

    private void NextPhrase()
    {
        index++;
        if (index >= phrases.Length)
        {
            EndDialogue();
        }
        else
        {
            if (sprites != null && index < sprites.Length && dialogueSprite != null)
                dialogueSprite.sprite = sprites[index];

            StartCoroutine(WritePhrase());
        }
    }

    private void EndDialogue()
    {
        talking = false;
        dialogueText.text = "";
        index = -1;
        dialogueFrame.SetActive(false);

        dialogueSprite.gameObject.SetActive(false);

        active = false;
        onDialogueFinished?.Invoke();
    }

    private IEnumerator WritePhrase()
    {
        talking = true;
        dialogueText.text = "";
        foreach (char caracter in phrases[index])
        {
            dialogueText.text += caracter;
            yield return new WaitForSeconds(timeBtwLetter);
        }
        talking = false;
    }

    private void CompletePhrase()
    {
        StopAllCoroutines();
        dialogueText.text = phrases[index];
        talking = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnActionPressed();
    }
}
