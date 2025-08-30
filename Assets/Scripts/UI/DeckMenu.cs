using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckMenu : MonoBehaviour
{
    [Header("Decks")]
    [SerializeField] private List<Card> badassDeck;
    [SerializeField] private List<Card> clearheadedDeck;
    [SerializeField] private List<Card> creepyDeck;

    [Header("Sprites")]
    [SerializeField] private Sprite[] badass;
    [SerializeField] private Sprite[] clearheaded;
    [SerializeField] private Sprite[] creepy;

    private Player player;

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
    }

    public void BadassDeck()
    {
        player.TotalHealth = 15;
        player.TotalMana = 6;

        player.AddNewCards(badassDeck);
        player.Images = badass;
        SceneManager.LoadScene("Map");
    }

    public void ClearheadedDeck()
    {
        player.TotalHealth = 8;
        player.TotalMana = 7;

        player.AddNewCards(clearheadedDeck);
        player.Images = clearheaded;
        SceneManager.LoadScene("Map");
    }

    public void CreepyDeck()
    {
        player.TotalHealth = 11;
        player.TotalMana = 5;

        player.AddNewCards(creepyDeck);
        player.Images = creepy;
        SceneManager.LoadScene("Map");
    }
}
