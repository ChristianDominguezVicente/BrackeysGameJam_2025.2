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

        if (cardVisualizer.card.areaEffect)
        {
            if (TurnManager.tm != null && TurnManager.tm.Enemies != null)
            {
                foreach (Enemy enemy in TurnManager.tm.Enemies)
                {
                    if (enemy.Health > 0)
                    {
                        cardVisualizer.card.OnActivated(enemy);
                    }
                }
            }
        }
        else
        {
            if (target != null)
            {
                cardVisualizer.card.OnActivated(target);
            }
        }

        Player.pj.OnCardUsed(this.gameObject);
    }
}
