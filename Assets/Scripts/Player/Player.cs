using System.Collections.Generic;
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

    public float spacing;

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

    private void OnEnable()
    {
        inputManager.OnCancel += Cancel;
        inputManager.OnPause += Pause;
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

            Vector2 cardPosition = new Vector2(nextPositionX, position.position.y);

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

    private void OnDisable()
    {
        inputManager.OnCancel -= Cancel;
        inputManager.OnPause -= Pause;
    }
}
