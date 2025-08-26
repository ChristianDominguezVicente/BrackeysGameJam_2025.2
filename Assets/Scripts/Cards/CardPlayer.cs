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

    public void Play(IHittable target)
    {

        Debug.Log("Carta jugada");

        if (target != null)
        {
            cardVisualizer.card.OnActivated(target);
            Player.pj.OnCardUsed(this.gameObject);
        }
    }
}
