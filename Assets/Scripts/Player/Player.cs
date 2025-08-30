using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IHittable
{
    public static Player pj;

    [Header("Inputs")]
    [SerializeField] private InputManagerSO inputManager;

    [Header("Player stats")]
    [SerializeField] private int totalHealth;
    [SerializeField] private int totalMana;

    [Header("Audios")]
    [SerializeField] private AudioClip cancelSound;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip cantSound;
    [SerializeField] private AudioClip ouchSound;
    [SerializeField] private AudioClip selectSound;
    [SerializeField] private AudioClip lostSound;
    [SerializeField] private AudioClip drawSound;
    [SerializeField] private AudioClip ouchEnemySound;

    private AudioSource audioSFX;

    private int health;
    private int mana;

    public int Health
    {
        get { return health; }
        set
        {
            this.health = value;
            OnLifeChanged?.Invoke(this.health, this.totalHealth);
        }
    }
    public int Mana
    {
        get { return mana; }
        set
        {
            this.mana = value;
            OnManaChanged?.Invoke(this.mana);
        }
    }

    public int TotalHealth { get => totalHealth; set => totalHealth = value; }
    public int TotalMana { get => totalMana; set => totalMana = value; }

    private Sprite[] images;

    public Sprite[] Images { get => images; set => images = value; }
    public AudioSource AudioSFX { get => audioSFX; set => audioSFX = value; }
    public AudioClip OuchEnemySound { get => ouchEnemySound; set => ouchEnemySound = value; }

    private PauseMenu pause;
    private GameObject pauseMenu;
    private GameObject settingsMenu;

    private List<Card> deck;
    private List<Card> cementery;

    [SerializeField] private int handSize = 3;

    [SerializeField] private Transform position;

    [SerializeField] private GameObject cardPrefab;

    private List<GameObject> hand;

    // Logica para la selección de las cartas con mando, teclado y raton.
    private int selectedCardIndex = 0;

    [Header("Card Configuration")]
    [SerializeField] private float selectedCardYOffset;
    [SerializeField] private float spacing;

    private float selectionCooldown = 0.25f;
    private float selectionCooldownEndTime = 0f;

    private GameObject selectedCard = null;

    [Header("Enemy selection configurator")]
    [SerializeField] private float selectedEnemyScaleModifier = 1.3f;
    private List<Enemy> enemies;
    private int selectedEnemyIndex = 0;
    private Enemy currentlySelectedEnemy = null;

    private List<StatusEffect> statusEffects;

    // Eventos y delegador para comunicarse con el gestor de los turnos
    public delegate void SelectedCard();
    public event SelectedCard OnSelectedCard;

    public delegate void EnemySelectionCanceled();
    public event EnemySelectionCanceled OnEnemySelectionCanceled;

    public delegate void CardPlayed(Card card, IHittable target, GameObject playedCard);
    public event CardPlayed OnCardPlayed;

    public delegate void TurnEnded();
    public event TurnEnded OnTurnEnded;

    public delegate void LifeChanged(int life, int totalLife);
    public event LifeChanged OnLifeChanged;

    public delegate void ManaChanged(int mana);
    public event ManaChanged OnManaChanged;

    public delegate void DamageTaken(int amount);
    public event DamageTaken OnDamageTaken;

    private void OnEnable()
    {
        inputManager.OnAction += HandleCardUsage;
        inputManager.OnMove += HandleSelection;
        inputManager.OnMouseLocation += HandleMouseSelection;
        inputManager.OnCancel += Cancel;
        inputManager.OnPause += Pause;
    }

    private void OnDisable()
    {
        inputManager.OnAction -= HandleCardUsage;
        inputManager.OnMove -= HandleSelection;
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
            this.statusEffects = new List<StatusEffect>();

            pj = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
            return;
        }

        Destroy(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TestScene" || scene.name == "Map")
        {
            audioSFX = GameObject.Find("AudioSFX").GetComponent<AudioSource>();
            pause = FindFirstObjectByType<PauseMenu>();

            if (pause != null)
            {
                pauseMenu = pause.PauseGameObject;
                settingsMenu = pause.SettingsGameObject;
            }

            if (scene.name == "TestScene")
            {
                enemies = new List<Enemy>(FindObjectsByType<Enemy>(FindObjectsSortMode.InstanceID));
                enemies.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
            }
            else
                enemies = null;
        }
        else
        {
            pause = null;
            pauseMenu = null;
            settingsMenu = null;
            this.Health = this.totalHealth;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
                if (cementery.Count > 0)
                {
                    ShuffleCementeryIntoDeck();
                    --i;
                }
            }
        }
        audioSFX.PlayOneShot(drawSound);
        UpdateCardPositions();
    }

    public void StartCombat()
    {
        this.Health = this.totalHealth;
        this.Mana = this.totalMana;
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

            CardVisualizer cv = hand[i].GetComponent<CardVisualizer>();
            SpriteRenderer sr = cv.GetComponent<SpriteRenderer>();

            sr.sortingOrder = (i == selectedCardIndex) ? 100 : i;
        }
    }

    private void Cancel()
    {
        if (DialogueSystem.ds.Active) return;
        if (SceneManager.GetActiveScene().name != "TestScene" && SceneManager.GetActiveScene().name != "Map") return;

        if (pause.IsPaused)
        {
            pause.Back();
        }
        else if (SceneManager.GetActiveScene().name == "TestScene" && TurnManager.tm.CurrentTurn == TurnManager.TurnState.SelectingTarget)
        {
            audioSFX.PlayOneShot(cancelSound);
            CancelEnemySelection();
        }
        else if (TurnManager.tm.CurrentTurn == TurnManager.TurnState.PlayerTurn)
        {
            EndTurn();
        }
    }

    public void StartEnemySelecitonMode()
    {
        selectedEnemyIndex = 0;
        UpdateEnemySelection();
    }

    private void CancelEnemySelection()
    {
        if (currentlySelectedEnemy != null)
        {
            currentlySelectedEnemy.StopFlashing();
            currentlySelectedEnemy = null;
        }

        OnEnemySelectionCanceled?.Invoke();
    }

    public void EndTurn()
    {
        HandleTurnEnd();
        OnTurnEnded?.Invoke();
    }

    public void HandleTurnEnd()
    {
        for (int i = hand.Count - 1; i >= 0; i--)
        {
            SendCardToCementery(hand[i]);
        }

        hand.Clear();

        Draw(handSize);
    }

    private void Pause()
    {
        if (DialogueSystem.ds.Active) return;
        if (SceneManager.GetActiveScene().name != "TestScene" && SceneManager.GetActiveScene().name != "Map") return;

        if (TurnManager.tm.ResultMenu != null && (TurnManager.tm.ResultMenu.SuccesMenu.activeSelf || TurnManager.tm.ResultMenu.FailMenu.activeSelf)) return;

        if (!pause.IsPaused)
        {
            pause.IsPaused = true;
            pauseMenu.SetActive(true);
            settingsMenu.SetActive(false);
            Time.timeScale = 0f;

            pause.ConfirmationMenu?.HideForPause();
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

        pause.ConfirmationMenu?.ShowAfterPause();
    }

    private void HandleSelection(Vector2 ctx)
    {
        if (DialogueSystem.ds.Active) return;
        if (SceneManager.GetActiveScene().name != "TestScene" || pause.IsPaused) return;

        if (ctx.x != 0 && Time.time >= selectionCooldownEndTime)
        {
            audioSFX.PlayOneShot(hoverSound);
            if (TurnManager.tm.CurrentTurn == TurnManager.TurnState.PlayerTurn)
            {
                if (ctx.x < 0 && selectedCardIndex > 0)
                {
                    selectedCardIndex--;
                }
                else if (ctx.x > 0 && selectedCardIndex < (hand.Count - 1))
                {
                    selectedCardIndex++;
                }

                UpdateCardPositions();
            }
            else if (TurnManager.tm.CurrentTurn == TurnManager.TurnState.SelectingTarget)
            {
                if (ctx.x < 0 && selectedEnemyIndex > 0)
                {
                    selectedEnemyIndex--;
                }
                else if (ctx.x > 0 && selectedEnemyIndex < (enemies.Count - 1))
                {
                    selectedEnemyIndex++;
                }

                UpdateEnemySelection();
            }

            selectionCooldownEndTime = Time.time + selectionCooldown;
        }
        else if (ctx.x == 0)
        {
            selectionCooldownEndTime = 0;
        }
    }

    private void HandleMouseSelection(Vector2 ctx)
    {
        if (DialogueSystem.ds.Active) return;
        if (SceneManager.GetActiveScene().name != "TestScene" || pause.IsPaused) return;
        if (TurnManager.tm.CurrentTurn != TurnManager.TurnState.PlayerTurn) return;

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
                    audioSFX.PlayOneShot(hoverSound);
                    UpdateCardPositions();
                }
            }
        }
    }

    private void HandleCardUsage()
    {
        if (DialogueSystem.ds.Active) return;
        if (SceneManager.GetActiveScene().name != "TestScene" || pause.IsPaused) return;

        if (TurnManager.tm.CurrentTurn == TurnManager.TurnState.PlayerTurn)
        {
            Debug.Log($"Carta Seleccionada {selectedCardIndex}");
            if (selectedCardIndex >= 0 && selectedCardIndex < hand.Count)
            {
                SelectCard();
            }
        }
        else if (TurnManager.tm.CurrentTurn == TurnManager.TurnState.SelectingTarget)
        {
            Debug.Log($"ATACANDO A {selectedEnemyIndex} ENTRE {enemies.Count}");
            if (selectedEnemyIndex >= 0 && selectedEnemyIndex < enemies.Count)
            {
                PlayCard(
                    hand[selectedCardIndex].GetComponent<CardVisualizer>().card,
                    enemies[selectedEnemyIndex],
                    hand[selectedCardIndex]
                );
            }
        }
    }

    private void SelectCard()
    {
        if (hand.Count > 0)
        {
            Card card = hand[selectedCardIndex].GetComponent<CardVisualizer>().card;

            if (card != null && Mana >= card.manaCost)
            {
                selectedCard = hand[selectedCardIndex];
                OnSelectedCard?.Invoke();
            }
            else
            {
                audioSFX.PlayOneShot(cantSound);
                Debug.Log("No tienes suficiente maná para juagr esta carta.");
            }
        }
    }

    private void PlayCard(Card card, IHittable target, GameObject playedCard)
    {
        Mana -= card.manaCost;
        OnCardPlayed?.Invoke(card, target, playedCard);
    }

    public void OnCardUsed(GameObject playedCard)
    {
        if (currentlySelectedEnemy != null)
        {
            currentlySelectedEnemy.StopFlashing();
            currentlySelectedEnemy = null;
        }

        SendCardToCementery(playedCard);

        if (selectedCardIndex >= hand.Count)
        {
            selectedCardIndex = hand.Count - 1;
        }

        UpdateCardPositions();
    }

    private void SendCardToCementery(GameObject card)
    {
        CardVisualizer cv = card.GetComponent<CardVisualizer>();

        if (cv != null && cv.card != null)
        {
            cementery.Add(cv.card);
        }

        hand.Remove(card);
        Destroy(card);
    }

    private void ShuffleCementeryIntoDeck()
    {
        deck.AddRange(cementery);
        cementery.Clear();

        for (int i = 0; i < deck.Count; i++)
        {
            Card tmp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);

            deck[i] = deck[randomIndex];
            deck[randomIndex] = tmp;
        }

        Debug.Log("Cementerio barajado al mazo");
    }

    public void ResetPlayer()
    {
        ClearHand();
        deck.AddRange(cementery);
        cementery.Clear();
        selectedCardIndex = 0;
        selectedCard = null;
        this.Health = this.totalHealth;
    }

    public void RemoveEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);

            selectedEnemyIndex = 0;

            UpdateEnemySelection();
        }
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

    public void ResetMana()
    {
        Mana = totalMana;
    }

    private void UpdateEnemySelection()
    {
        if (currentlySelectedEnemy != null)
        {
            currentlySelectedEnemy.StopFlashing();
        }

        audioSFX.PlayOneShot(selectSound);

        for (int i = 0; i < enemies.Count; i++)
        {
            if (i == selectedEnemyIndex)
            {
                currentlySelectedEnemy = enemies[i];
                currentlySelectedEnemy.StartFlashing();
            }
        }
    }

    public void TakeDamage(int amount, DamageType dt)
    {
        int damageTaken = amount;

        switch (dt)
        {
            default:
                break;
        }

        this.Health -= damageTaken;

        if (this.Health > totalHealth) this.Health = totalHealth;

        Debug.Log($"El jugador se come {damageTaken} par auna vida resultante de {this.Health}/{this.totalHealth}");

        OnDamageTaken(damageTaken);
        audioSFX.PlayOneShot(ouchSound);

        if (this.Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("OH NOOOOOOOOOOOOOOOOOOOOOOOOOOOOO! GAME OVER!!!!!!!!!!!!!!!");
        audioSFX.PlayOneShot(lostSound);
        TurnManager.tm.CurrentTurn = TurnManager.TurnState.NotPlayable;
        TurnManager.tm.ResultMenu.FailMenu.SetActive(true);
        TurnManager.tm.ResultMenu.FailSprite.sprite = Images[1];
        TurnManager.tm.ResultMenu.SelectButton();
    }

    public bool HasStatusEffect(StatusEffect status)
    {
        return statusEffects.Contains(status);
    }

    public void AddStatus(StatusEffect status)
    {
        if (HasStatusEffect(status))
            return;

        statusEffects.Add(status);
    }

    public void RemoveStatus(StatusEffect status)
    {
        if (!HasStatusEffect(status))
            return;

        statusEffects.Remove(status);
    }

    public StatusEffect HandleStatusEffects()
    {
        foreach (StatusEffect se in statusEffects)
        {
            switch (se)
            {
                case StatusEffect.Bleeding:
                    TakeDamage(1, DamageType.Bleed);
                    break;

                case StatusEffect.Numb:
                    return StatusEffect.Numb;

                default:
                    break;
            }

            if (this.Health <= 0)
                return StatusEffect.Death;
        }

        return StatusEffect.None;
    }
}
