using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckMenu : MonoBehaviour
{
    [SerializeField]
    private Player player;

    [SerializeField]
    private List<Card> badassDeck;

    public void BadassDeck()
    {

        player.AddNewCards(badassDeck);

        SceneManager.LoadScene("TestScene");
    }

    public void ClearheadedDeck()
    {

    }

    public void CreepyDeck()
    {
        
    }
}
