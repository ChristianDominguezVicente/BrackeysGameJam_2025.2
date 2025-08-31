using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject failMenu;
    [SerializeField] private GameObject succesMenu;
    [SerializeField] private GameObject failButton;
    [SerializeField] private Image failSprite;
    [SerializeField] private Image successSprite;
    [SerializeField] private Image failEnemySprite;
    [SerializeField] private Image successEnemySprite;

    [Header("Rewards")]
    [SerializeField] private Transform buttonsContainer;
    [SerializeField] private GameObject rewardsButtonPrefab;

    private Player player;

    public GameObject FailMenu { get => failMenu; set => failMenu = value; }
    public GameObject SuccesMenu { get => succesMenu; set => succesMenu = value; }
    public Image FailSprite { get => failSprite; set => failSprite = value; }
    public Image SuccessSprite { get => successSprite; set => successSprite = value; }
    public Image FailEnemySprite { get => failEnemySprite; set => failEnemySprite = value; }
    public Image SuccessEnemySprite { get => successEnemySprite; set => successEnemySprite = value; }

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
    }

    public void SelectButton()
    {
        if (failMenu.activeSelf)
            EventSystem.current.SetSelectedGameObject(failButton);
        else
        {
            if (buttonsContainer.childCount > 0)
            {
                GameObject firstButton = buttonsContainer.GetChild(0).gameObject;
                EventSystem.current.SetSelectedGameObject(firstButton);
            }
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene("TestScene");
    }

    public void ReturnMap()
    {
        Map.map.RevertSelection();
        SceneManager.LoadScene("Map");
    }

    public void ShowRewards(Node.Difficulty difficulty, int level)
    {
        foreach (Transform child in buttonsContainer)
        {
            Destroy(child.gameObject);
        }

        if (level == 6)
        {
            CreateRewardButton("Final", () =>
            {
                SceneManager.LoadScene("Final");
            });
            return;
        }

        if (difficulty == Node.Difficulty.Human)
        {
            switch (level)
            {
                case 1:
                    CreateRewardButton("+1 Health", () => { player.TotalHealth += 1; GoMap(); });
                    break;
                case 2:
                    CreateRewardButton("+1 Health", () => { player.TotalHealth += 1; GoMap(); });
                    CreateRewardButton("+1 Mana", () => { player.TotalMana += 1; GoMap(); });
                    break;
                case 3:
                case 4:
                    CreateRewardButton("+2 Health", () => { player.TotalHealth += 2; GoMap(); });
                    CreateRewardButton("+1 Mana", () => { player.TotalMana += 1; GoMap(); });
                    break;
                case 5:
                    CreateRewardButton("+3 Health", () => { player.TotalHealth += 3; GoMap(); });
                    CreateRewardButton("+1 Mana", () => { player.TotalMana += 1; GoMap(); });
                    break;
            }
        }
        else if (difficulty == Node.Difficulty.NoHuman)
        {
            switch (level)
            {
                case 1:
                    CreateRewardButton("+2 Health", () => { player.TotalHealth += 2; GoMap(); });
                    CreateRewardButton("+1 Mana", () => { player.TotalMana += 1; GoMap(); });
                    break;
                case 2:
                    CreateRewardButton("+3 Health", () => { player.TotalHealth += 3; GoMap(); });
                    CreateRewardButton("+2 Mana", () => { player.TotalMana += 2; GoMap(); });
                    break;
                case 3:
                    CreateRewardButton("-2 Health", () => { player.TotalHealth -= 2; GoMap(); });
                    CreateRewardButton("-1 Mana", () => { player.TotalMana -= 1; GoMap(); });
                    break;
                case 4:
                    CreateRewardButton("+3 Health", () => { player.TotalHealth += 3; GoMap(); });
                    CreateRewardButton("+3 Mana", () => { player.TotalMana += 3; GoMap(); });
                    break;
                case 5:
                    CreateRewardButton("+3 Health and +1 Mana", () => { player.TotalHealth += 3; player.TotalMana += 1; GoMap(); });
                    CreateRewardButton("+3 Mana and +1 Health", () => { player.TotalMana += 3; player.TotalHealth += 1; GoMap(); });
                    break;
            }
        }

        SelectButton();
    }

    private void CreateRewardButton(string text, UnityEngine.Events.UnityAction action)
    {
        GameObject btn = Instantiate(rewardsButtonPrefab, buttonsContainer);
        btn.GetComponentInChildren<TextMeshProUGUI>().text = text;
        btn.GetComponent<Button>().onClick.AddListener(action);
    }

    public void GoMap()
    {
        SceneManager.LoadScene("Map");
    }
}
