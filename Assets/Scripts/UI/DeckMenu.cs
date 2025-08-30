using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckMenu : MonoBehaviour
{
    [Header("Decks")]
    [SerializeField] private List<Card> badassDeck;

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
        player.AddNewCards(badassDeck);
        player.Images = badass;
        SceneManager.LoadScene("Map");
    }

    public void ClearheadedDeck()
    {
        player.Images = clearheaded;
    }

    public void CreepyDeck()
    {
        player.Images = creepy;
    }
}
