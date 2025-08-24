using UnityEngine;

[RequireComponent(typeof(CardVisualizer))]
public class CardPlayer : MonoBehaviour
{
    private CardVisualizer cardVisualizer;

    void Awake()
    {
        cardVisualizer = GetComponent<CardVisualizer>();
    }

    void OnMouseDown()
    {
        Play();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Play()
    {

        Debug.Log("Carta jugada");

        cardVisualizer.card.OnActivated(null);
    }
}
