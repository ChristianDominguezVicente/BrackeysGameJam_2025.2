using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckMenu : MonoBehaviour
{
    [Header("Decks")]
    [SerializeField] private List<Card> badassDeck;

    private Player player;

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
    }

    public void BadassDeck()
    {
        player.AddNewCards(badassDeck);

        SceneManager.LoadScene("Map");
    }

    public void ClearheadedDeck()
    {

    }

    public void CreepyDeck()
    {
        
    }
}
