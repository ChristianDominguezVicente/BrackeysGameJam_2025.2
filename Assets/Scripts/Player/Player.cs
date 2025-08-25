using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public static Player pj;

    [Header("Inputs")]
    [SerializeField] private InputManagerSO inputManager;

    private PauseMenu pause;
    private GameObject pauseMenu;
    private GameObject settingsMenu;

    private List<Card> deck;
    private List<Card> cementery;

    public int handSize = 3;

    public Transform position;

    public GameObject cardPrefab;

    private List<GameObject> hand;

    // Logica para la selección de las cartas con mando, teclado y raton.
    private int selectedCardIndex = 0;

    [Header("Card Configuration")]
    [SerializeField] private float selectedCardYOffset;
    [SerializeField] private float spacing;

    private float selectionCooldown = 0.25f;
    private float selectionCooldownEndTime = 0f;

    private GameObject selectedCard = null;

    // Eventos y delegador para comunicarse con el gestor de los turnos
    public delegate void SelectedCard();
    public event SelectedCard OnSelectedCard;

    public delegate void EnemySelectionCanceled();
    public event EnemySelectionCanceled OnEnemySelectionCanceled;

    private void OnEnable()
    {
        inputManager.OnAction += HandleCardUsage;
        inputManager.OnMove += HandleCardSelection;
        inputManager.OnMouseLocation += HandleMouseSelection;
        inputManager.OnCancel += Cancel;
        inputManager.OnPause += Pause;
    }

    private void OnDisable()
    {
        inputManager.OnAction -= HandleCardUsage;
        inputManager.OnMove -= HandleCardSelection;
        inputManager.OnMouseLocation -= HandleMouseSelection;
        inputManager.OnCancel -= Cancel;
        inputManager.OnPause -= Pause;
    }

    public void AddNewCard(Card newCard)
    {
        this.deck.Add(newCard);
    }

    public void AddNewCards(List<Card> cards)
    {
        this.deck.AddRange(cards);
    }

    public void ClearDeck()
    {
        this.deck.Clear();
        ClearHand();
        this.cementery.Clear();
        UpdateCardPositions();
    }

    void Awake()
    {
        if (pj == null)
        {
            this.deck = new List<Card>();
            this.hand = new List<GameObject>();
            this.cementery = new List<Card>();

            pj = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
            return;
        }

        Destroy(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TestScene")
        {
            pause = FindFirstObjectByType<PauseMenu>();
            if (pause != null)
            {
                pauseMenu = pause.PauseGameObject;
                settingsMenu = pause.SettingsGameObject;
            }
        }
        else
        {
            pause = null;
            pauseMenu = null;
            settingsMenu = null;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DrawHand();
    }

    // Update is called once per frame
    void Update()
    {

        // TODO DEBUG ELIMINAR ELIMINAR TODO
        if (Input.GetKeyDown(KeyCode.X))
            DrawHand();
    }

    private void Draw(int cardNum)
    {
        for (int i = 0; i < cardNum; i++)
        {
            if (deck.Count > 0)
            {
                int newCardIndex = Random.Range(0, deck.Count);
                Card drewCard = deck[newCardIndex];
                deck.Remove(drewCard);

                GameObject card = Instantiate(cardPrefab, position);

                CardVisualizer cv = card.GetComponent<CardVisualizer>();

                cv.card = drewCard;

                hand.Add(card);
            }
            else
            {
                Debug.Log("TE QUEDASTE SIN CARTAS");
            }
        }

        UpdateCardPositions();
    }

    private void DrawHand()
    {
        Draw(handSize);
    }

    private void UpdateCardPositions()
    {
        float firstCardPosition = -((hand.Count - 1) * spacing) / 2f;

        for (int i = 0; i < hand.Count; i++)
        {
            float nextPositionX = firstCardPosition + i * spacing;
            float yOffset = (i == selectedCardIndex) ? selectedCardYOffset : 0;

            Vector2 cardPosition = new Vector2(nextPositionX, position.position.y + yOffset);

            hand[i].transform.position = cardPosition;
        }
    }

    private void Cancel()
    {
        if (SceneManager.GetActiveScene().name != "TestScene") return;

        if (pause.IsPaused)
        {
            pause.Back();
        }
        else if (TurnManager.tm.CurrentTurn == TurnManager.TurnState.SelectingTarget)
        {
            OnEnemySelectionCanceled?.Invoke();
        }
    }

    private void Pause()
    {
        if (SceneManager.GetActiveScene().name != "TestScene") return;

        if (!pause.IsPaused)
        {
            pause.IsPaused = true;
            pauseMenu.SetActive(true);
            settingsMenu.SetActive(false);
            Time.timeScale = 0f;
        }
        else
        {
            if (settingsMenu.activeSelf)
            {
                settingsMenu.SetActive(false);
                Resume();
            }
            else if (pauseMenu.activeSelf)
            {
                Resume();
            }
        }

    }

    private void Resume()
    {
        pause.IsPaused = false;
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    private void HandleCardSelection(Vector2 ctx)
    {
        if (SceneManager.GetActiveScene().name != "TestScene" || pause.IsPaused) return;

        if (ctx.x != 0 && Time.time >= selectionCooldownEndTime)
        {
            if (ctx.x < 0 && selectedCardIndex > 0)
            {
                selectedCardIndex--;
            }
            else if (ctx.x > 0 && selectedCardIndex < (hand.Count - 1))
            {
                selectedCardIndex++;
            }

            selectionCooldownEndTime = Time.time + selectionCooldown;
            UpdateCardPositions();
        }
        else if (ctx.x == 0)
        {
            selectionCooldownEndTime = 0;
        }
    }

    private void HandleMouseSelection(Vector2 ctx)
    {
        if (SceneManager.GetActiveScene().name != "TestScene" || pause.IsPaused) return;

        Vector2 globalLocation = Camera.main.ScreenToWorldPoint(ctx);

        RaycastHit2D isCard = Physics2D.Raycast(globalLocation, Vector2.zero);

        if (isCard.collider != null)
        {
            CardPlayer cardPlayer = isCard.collider.GetComponent<CardPlayer>();

            if (cardPlayer != null)
            {
                int i = hand.IndexOf(cardPlayer.gameObject);

                if (i != selectedCardIndex && (i >= 0 || i < hand.Count))
                {
                    selectedCardIndex = i;

                    UpdateCardPositions();
                }
            }
        }
    }

    private void HandleCardUsage()
    {
        if (SceneManager.GetActiveScene().name != "TestScene" || pause.IsPaused) return;

        if (selectedCardIndex >= 0 && selectedCardIndex < hand.Count)
            SelectCard();
    }

    private void SelectCard()
    {
        if (hand.Count > 0)
        {
            selectedCard = hand[selectedCardIndex];
            OnSelectedCard?.Invoke();
        }
    }

    public void OnCardPlayed(GameObject playedCard)
    {
        CardVisualizer cv = playedCard.GetComponent<CardVisualizer>();

        if (cv != null && cv.card != null)
        {
            cementery.Add(cv.card);
        }

        hand.Remove(playedCard);
        Destroy(playedCard);

        if (selectedCardIndex >= hand.Count)
        {
            selectedCardIndex = hand.Count - 1;
        }

        UpdateCardPositions();
    }

    private void ClearHand()
    {
        for (int i = hand.Count - 1; i >= 0; i--)
        {
            GameObject tmp = hand[i];
            hand.Remove(tmp);
            Destroy(tmp);
        }
    }
}
