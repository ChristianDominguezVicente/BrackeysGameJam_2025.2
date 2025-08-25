using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player pj;

    private List<Card> deck;
    private List<Card> cementery;

    public int handSize = 3;

    public Transform position;

    public GameObject cardPrefab;

    private List<GameObject> hand;

    [SerializeField]
    private float spacing;

    // Logica para la selección de las cartas con mando, teclado y raton.
    private int selectedCardIndex = 0;

    [SerializeField]
    private float selectedCardYOffset;
    [SerializeField]
    private InputActionAsset actions;

    private InputAction ia_SelectCard;
    private InputAction ia_Select;

    private Vector2 userSelection;

    private float selectionCooldown = 0.25f;
    private float selectionCooldownEndTime = 0f;

    void OnEnable()
    {
        actions.FindActionMap("Gameplay").Enable();
    }

    void OnDisable()
    {
        actions.FindActionMap("Gameplay").Disable();
    }

    public void AddNewCard(Card newCard)
    {
        this.deck.Add(newCard);
    }

    public void AddNewCards(List<Card> cards)
    {
        this.deck.AddRange(cards);
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

            ia_SelectCard = actions.FindActionMap("Gameplay").FindAction("Move");
            ia_Select = actions.FindActionMap("Gameplay").FindAction("Action");

            return;
        }

        Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DrawHand();
    }

    // Update is called once per frame
    void Update()
    {
        HandleCardSelection();
        HandleCardUsage();

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

    private void HandleCardSelection()
    {
        userSelection = ia_SelectCard.ReadValue<Vector2>();

        if (userSelection.x != 0 && Time.time >= selectionCooldownEndTime)
        {
            if (userSelection.x < 0 && selectedCardIndex > 0)
            {
                selectedCardIndex--;
            }
            else if (userSelection.x > 0 && selectedCardIndex < (hand.Count - 1))
            {
                selectedCardIndex++;
            }

            selectionCooldownEndTime = Time.time + selectionCooldown;
            UpdateCardPositions();
        }
        else if (userSelection.x == 0)
        {
            selectionCooldownEndTime = 0;
        }
    }

    private void HandleCardUsage()
    {

        if (ia_Select.WasPressedThisFrame())
        {
            PlayCard();

        }
    }

    private void PlayCard()
    {
        if (hand.Count > 0)
        {
            GameObject selectedCard = hand[selectedCardIndex];
            CardPlayer cardPlayer = selectedCard.GetComponent<CardPlayer>();

            if (cardPlayer != null)
            {
                cardPlayer.Play();
            }
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
}
