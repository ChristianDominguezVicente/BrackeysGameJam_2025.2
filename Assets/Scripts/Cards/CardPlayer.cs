using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CardVisualizer))]
public class CardPlayer : MonoBehaviour
{
    private CardVisualizer cardVisualizer;

    void Awake()
    {
        cardVisualizer = GetComponent<CardVisualizer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Play()
    {

        Debug.Log("Carta jugada");

        IHittable ih = FindObjectsByType<MonoBehaviour>(sortMode: FindObjectsSortMode.None).OfType<IHittable>().ToArray()[0];

        if (ih != null)
        {
            cardVisualizer.card.OnActivated(ih);
            Player.pj.OnCardPlayed(this.gameObject);
        }
    }
}
