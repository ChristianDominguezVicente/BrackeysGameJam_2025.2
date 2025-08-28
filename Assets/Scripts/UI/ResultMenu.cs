using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject failMenu;
    [SerializeField] private GameObject succesMenu;

    private Player player;

    public GameObject FailMenu { get => failMenu; set => failMenu = value; }
    public GameObject SuccesMenu { get => succesMenu; set => succesMenu = value; }


    private void Start()
    {
        player = FindFirstObjectByType<Player>();
    }

    public void Retry()
    {
        SceneManager.LoadScene("TestScene");
    }

    public void ReturnMap()
    {
        player.ClearDeck();

        if (failMenu.activeSelf)
            Map.map.RevertSelection();

        SceneManager.LoadScene("Map");
    }
}
