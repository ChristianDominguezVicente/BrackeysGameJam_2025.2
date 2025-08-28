using UnityEngine;

public class CardVisualizer : MonoBehaviour
{
    public Card card;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (spriteRenderer == null)
        {
            Debug.LogError("Algo esta mal no hay sprite render en la carta WTF ha pasado xD lol");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (card != null && card.cardSprite != null)
        {
            spriteRenderer.sprite = card.cardSprite;

            if (boxCollider != null)
            {
                boxCollider.size = spriteRenderer.bounds.size * 2;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
