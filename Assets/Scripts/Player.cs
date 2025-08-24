using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public List<Card> deck;
    private List<Card> cementery;

    public int handSize = 3;

    public Transform position;

    public GameObject cardPrefab;

    private List<GameObject> hand;

    public float spacing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hand = new List<GameObject>();
        cementery = new List<Card>();
        DrawHand();
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
}
