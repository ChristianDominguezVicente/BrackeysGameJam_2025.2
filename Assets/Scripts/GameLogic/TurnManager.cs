using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class TurnManager : MonoBehaviour
{
    public static TurnManager tm;

    public enum TurnState
    {
        PlayerTurn,
        SelectingTarget,
        EnemyTurn,
        NotPlayable,
    }

    [Header("Dependencias necesarias")]
    [SerializeField] private Player player;

    public Player Player { get { return player; } }

    private int turnNumber = 0;

    [Header("Enemies Human Prefab")]
    [SerializeField] private GameObject salesmanPrefab;
    [SerializeField] private GameObject drunkPrefab;
    [SerializeField] private GameObject teacherPrefab;
    [SerializeField] private GameObject exConvictPrefab;
    [SerializeField] private GameObject junkiePrefab1;
    [SerializeField] private GameObject junkiePrefab2;
    [SerializeField] private GameObject yakuzaPrefab1;
    [SerializeField] private GameObject yakuzaPrefab2;

    [Header("Enemies NoHuman Prefab")]
    [SerializeField] private GameObject racoonPrefab;
    [SerializeField] private GameObject ratPrefab;
    [SerializeField] private GameObject mutantRatPrefab;
    [SerializeField] private GameObject mutantDogPrefab;
    [SerializeField] private GameObject wtfPrefab;

    [Header("DialoguesHuman")]
    [SerializeField] private string[] hCombat1;
    [SerializeField] private string[] hCombat2;
    [SerializeField] private string[] hCombat3;
    [SerializeField] private string[] hCombat4;
    [SerializeField] private string[] hCombat5;
    [SerializeField] private string[] hCombat6;

    [Header("DialoguesNoHuman")]
    [SerializeField] private string[] nhCombat1;
    [SerializeField] private string[] nhCombat2a;
    [SerializeField] private string[] nhCombat2b;
    [SerializeField] private string[] nhCombat3a;
    [SerializeField] private string[] nhCombat3b;
    [SerializeField] private string[] nhCombat4a;
    [SerializeField] private string[] nhCombat4b;
    [SerializeField] private string[] nhCombat5a;
    [SerializeField] private string[] nhCombat5b;
    [SerializeField] private string[] nhCombat6;

    [Header("Images")]
    [SerializeField] private Sprite humanBG;
    [SerializeField] private Sprite noHumanBG;

    private TextMeshProUGUI turnChangeFeedback;

    private int selectedNodeLevel;
    private int selectedNodeIndex;
    private Node.Difficulty selectedNodeDifficulty;

    public int SelectedNodeLevel { get => selectedNodeLevel; set => selectedNodeLevel = value; }
    public int SelectedNodeIndex { get => selectedNodeIndex; set => selectedNodeIndex = value; }
    public Node.Difficulty SelectedNodeDifficulty { get => selectedNodeDifficulty; set => selectedNodeDifficulty = value; }

    public int TurnNumber
    {
        get { return turnNumber; }
    }

    private TurnState currentTurn;

    public delegate void TurnStateChanged(TurnState state);
    public event TurnStateChanged OnTurnStateChanged;

    public TurnState CurrentTurn
    {
        get { return currentTurn; }
        set
        {
            currentTurn = value;
            OnTurnStateChanged?.Invoke(currentTurn);
        }
    }

    private List<Enemy> enemies;
    public List<Enemy> Enemies { get { return enemies; } }

    private ResultMenu resultMenu;
    public ResultMenu ResultMenu { get => resultMenu; }

    private void Awake()
    {
        if (tm == null)
        {
            tm = this;
            DontDestroyOnLoad(tm);
            return;
        }

        Destroy(this.gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += InitializeTurnManager;

        if (player != null)
        {
            player.OnSelectedCard += StartEnemySelection;
            player.OnEnemySelectionCanceled += CancelEnemySelection;
            player.OnCardPlayed += CardPlayed;
            player.OnTurnEnded += EndPlayerTurn;
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= InitializeTurnManager;

        if (player != null)
        {
            player.OnSelectedCard -= StartEnemySelection;
            player.OnEnemySelectionCanceled -= CancelEnemySelection;
            player.OnCardPlayed -= CardPlayed;
            player.OnTurnEnded -= EndPlayerTurn;
        }
    }

    public void GenerateEnemies(Node.Difficulty difficulty, int level, int index)
    {
        switch (difficulty)
        {
            case Node.Difficulty.Human:
                InstantiateHumanEnemy(level, index);
                break;
            case Node.Difficulty.NoHuman:
                InstantiateNoHumanEnemy(level, index);
                break;
        }
    }

    private void SpawnEnemies(GameObject[] prefabs)
    {
        if (enemies == null)
            enemies = new List<Enemy>();

        Vector3 centerPosition = new Vector3(0, 1.5f, 0);
        float spacing = 3f;
        int enemyCount = prefabs.Length;

        for (int i = 0; i < enemyCount; i++)
        {
            Enemy newEnemy = Instantiate(prefabs[i]).GetComponent<Enemy>();
            float offsetX = (i - (enemyCount - 1) / 2f) * spacing;

            float offsetY = 0f;
            if (enemyCount >= 3)
            {
                if (i % 2 == 0)
                    offsetY = 0.5f;
                else
                    offsetY = -0.5f;
            }

            newEnemy.transform.position = centerPosition + new Vector3(offsetX, offsetY, 0);
            enemies.Add(newEnemy);
        }
    }

    private void InstantiateHumanEnemy(int level, int index)
    {
        switch (level)
        {
            case 1:
                SpawnEnemies(new GameObject[] { salesmanPrefab });
                resultMenu.FailEnemySprite.sprite = resultMenu.SuccessEnemySprite.sprite = salesmanPrefab.GetComponent<SpriteRenderer>().sprite;
                break;
            case 2:
                SpawnEnemies(new GameObject[] { drunkPrefab });
                resultMenu.FailEnemySprite.sprite = resultMenu.SuccessEnemySprite.sprite = drunkPrefab.GetComponent<SpriteRenderer>().sprite;
                break;
            case 3:
                SpawnEnemies(new GameObject[] { teacherPrefab });
                resultMenu.FailEnemySprite.sprite = resultMenu.SuccessEnemySprite.sprite = teacherPrefab.GetComponent<SpriteRenderer>().sprite;
                break;
            case 4:
                SpawnEnemies(new GameObject[] { exConvictPrefab });
                resultMenu.FailEnemySprite.sprite = resultMenu.SuccessEnemySprite.sprite = exConvictPrefab.GetComponent<SpriteRenderer>().sprite;
                break;
            case 5:
                SpawnEnemies(new GameObject[] { junkiePrefab1, junkiePrefab2 });
                resultMenu.FailEnemySprite.sprite = resultMenu.SuccessEnemySprite.sprite = junkiePrefab1.GetComponent<SpriteRenderer>().sprite;
                break;
            case 6:
                SpawnEnemies(new GameObject[] { yakuzaPrefab1, yakuzaPrefab2 });
                resultMenu.FailEnemySprite.sprite = resultMenu.SuccessEnemySprite.sprite = yakuzaPrefab1.GetComponent<SpriteRenderer>().sprite;
                break;
        }
    }

    private void InstantiateNoHumanEnemy(int level, int index)
    {
        switch (level)
        {
            case 1:
                SpawnEnemies(new GameObject[] { racoonPrefab });
                resultMenu.FailEnemySprite.sprite = resultMenu.SuccessEnemySprite.sprite = racoonPrefab.GetComponent<SpriteRenderer>().sprite;
                break;
            case 2:
                if (index == 0)
                {
                    SpawnEnemies(new GameObject[] { ratPrefab, racoonPrefab });
                    resultMenu.FailEnemySprite.sprite = resultMenu.SuccessEnemySprite.sprite = ratPrefab.GetComponent<SpriteRenderer>().sprite;
                }
                else
                {
                    SpawnEnemies(new GameObject[] { ratPrefab, mutantRatPrefab, ratPrefab });
                    resultMenu.FailEnemySprite.sprite = resultMenu.SuccessEnemySprite.sprite = mutantRatPrefab.GetComponent<SpriteRenderer>().sprite;
                }
                break;
            case 3:
                if (index == 0)
                {
                    SpawnEnemies(new GameObject[] { ratPrefab, racoonPrefab, ratPrefab });
                    resultMenu.FailEnemySprite.sprite = resultMenu.SuccessEnemySprite.sprite = ratPrefab.GetComponent<SpriteRenderer>().sprite;
                }
                else
                {
                    SpawnEnemies(new GameObject[] { mutantRatPrefab, mutantDogPrefab, mutantRatPrefab });
                    resultMenu.FailEnemySprite.sprite = resultMenu.SuccessEnemySprite.sprite = mutantDogPrefab.GetComponent<SpriteRenderer>().sprite;
                }
                break;
            case 4:
                if (index == 0)
                {
                    SpawnEnemies(new GameObject[] { ratPrefab, racoonPrefab, ratPrefab, racoonPrefab, ratPrefab });
                    resultMenu.FailEnemySprite.sprite = resultMenu.SuccessEnemySprite.sprite = ratPrefab.GetComponent<SpriteRenderer>().sprite;
                }
                else
                {
                    SpawnEnemies(new GameObject[] { mutantRatPrefab, mutantDogPrefab, mutantRatPrefab, mutantRatPrefab });
                    resultMenu.FailEnemySprite.sprite = resultMenu.SuccessEnemySprite.sprite = mutantDogPrefab.GetComponent<SpriteRenderer>().sprite;
                }
                break;
            case 5:
                if (index == 0)
                {
                    SpawnEnemies(new GameObject[] { ratPrefab, ratPrefab, ratPrefab, ratPrefab, ratPrefab });
                    resultMenu.FailEnemySprite.sprite = resultMenu.SuccessEnemySprite.sprite = ratPrefab.GetComponent<SpriteRenderer>().sprite;
                }
                else
                {
                    SpawnEnemies(new GameObject[] { mutantDogPrefab, mutantRatPrefab, ratPrefab, mutantRatPrefab, mutantDogPrefab });
                    resultMenu.FailEnemySprite.sprite = resultMenu.SuccessEnemySprite.sprite = mutantDogPrefab.GetComponent<SpriteRenderer>().sprite;
                }
                break;
            case 6:
                SpawnEnemies(new GameObject[] { wtfPrefab, mutantDogPrefab, mutantRatPrefab, ratPrefab, racoonPrefab });
                resultMenu.FailEnemySprite.sprite = resultMenu.SuccessEnemySprite.sprite = wtfPrefab.GetComponent<SpriteRenderer>().sprite;
                break;
        }
    }

    private void InitializeTurnManager(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TestScene")
        {
            Debug.Log("EMPEZANDO EL MANEJO DE TURNOS");

            GameObject fto = GameObject.Find("TurnFeedbackText");

            resultMenu = FindFirstObjectByType<ResultMenu>();

            if (fto != null)
            {
                turnChangeFeedback = fto.GetComponent<TextMeshProUGUI>();

                if (turnChangeFeedback != null)
                {
                    turnChangeFeedback.gameObject.SetActive(false);
                }
            }

            PrepareVariables();
            GenerateEnemies(selectedNodeDifficulty, selectedNodeLevel, selectedNodeIndex);

            string[] phrases = GetDialogueForNode(selectedNodeDifficulty, selectedNodeLevel, selectedNodeIndex);

            if (selectedNodeDifficulty == Node.Difficulty.Human)
                GameObject.Find("Image").GetComponent<SpriteRenderer>().sprite = humanBG;
            else
                GameObject.Find("Image").GetComponent<SpriteRenderer>().sprite = noHumanBG;

            Sprite[] spritesShow = new Sprite[phrases.Length];

            switch (selectedNodeDifficulty)
            {
                case Node.Difficulty.Human:
                    switch (selectedNodeLevel)
                    {
                        case 1:
                            for (int i = 0; i < phrases.Length; i++)
                            {
                                if (i < 3)
                                    spritesShow[i] = Player.Images[2];
                                else
                                    spritesShow[i] = Player.Images[0];
                            }
                            break;
                        case 2:
                            for (int i = 0; i < phrases.Length; i++)
                            {
                                if (i < 2)
                                    spritesShow[i] = Player.Images[3];
                                else if (i < 3)
                                    spritesShow[i] = Player.Images[2];
                                else
                                    spritesShow[i] = Player.Images[0];
                            }
                            break;
                        case 3:
                            for (int i = 0; i < phrases.Length; i++)
                                spritesShow[i] = Player.Images[3];
                            break;
                        case 4:
                            for (int i = 0; i < phrases.Length; i++)
                            {
                                if (i < 4)
                                    spritesShow[i] = Player.Images[2];
                                else
                                    spritesShow[i] = Player.Images[1];
                            }
                            break;
                        case 5:
                            for (int i = 0; i < phrases.Length; i++)
                            {
                                if (i < 3)
                                    spritesShow[i] = Player.Images[2];
                                else if (i < 4)
                                    spritesShow[i] = Player.Images[0];
                                else
                                    spritesShow[i] = Player.Images[2];
                            }
                            break;
                        case 6:
                            for (int i = 0; i < phrases.Length; i++)
                            {
                                if (i < 2)
                                    spritesShow[i] = Player.Images[2];
                                else if (i < 4)
                                    spritesShow[i] = Player.Images[1];
                                else
                                    spritesShow[i] = Player.Images[0];
                            }
                            break;
                    }
                    break;

                case Node.Difficulty.NoHuman:
                    switch (selectedNodeLevel)
                    {
                        case 1:
                            for (int i = 0; i < phrases.Length; i++)
                            {
                                if (i < 2)
                                    spritesShow[i] = Player.Images[2];
                                else
                                    spritesShow[i] = Player.Images[0];
                            }
                            break;
                        case 2:
                            if (selectedNodeIndex == 0)
                            {
                                for (int i = 0; i < phrases.Length; i++)
                                    spritesShow[i] = Player.Images[2];
                                break;
                            }
                            else
                            {
                                for (int i = 0; i < phrases.Length; i++)
                                    spritesShow[i] = Player.Images[3];
                                break;
                            }
                        case 3:
                            if (selectedNodeIndex == 0)
                            {
                                for (int i = 0; i < phrases.Length; i++)
                                    spritesShow[i] = Player.Images[3];
                                break;
                            }
                            else
                            {
                                for (int i = 0; i < phrases.Length; i++)
                                {
                                    if (i < 1)
                                        spritesShow[i] = Player.Images[2];
                                    else
                                        spritesShow[i] = Player.Images[3];
                                }
                                break;
                            }
                        case 4:
                            if (selectedNodeIndex == 0)
                            {
                                for (int i = 0; i < phrases.Length; i++)
                                    spritesShow[i] = Player.Images[0];
                                break;
                            }
                            else
                            {
                                for (int i = 0; i < phrases.Length; i++)
                                    spritesShow[i] = Player.Images[0];
                                break;
                            }
                        case 5:
                            if (selectedNodeIndex == 0)
                            {
                                for (int i = 0; i < phrases.Length; i++)
                                    spritesShow[i] = Player.Images[1];
                                break;
                            }
                            else
                            {
                                for (int i = 0; i < phrases.Length; i++)
                                    spritesShow[i] = Player.Images[1];
                                break;
                            }
                        case 6:
                            for (int i = 0; i < phrases.Length; i++)
                            {
                                if (i < 4)
                                    spritesShow[i] = Player.Images[2];
                                else
                                    spritesShow[i] = Player.Images[1];
                            }
                            break;
                    }
                    break;
            }

            DialogueSystem.ds.StartDialogue(phrases, spritesShow, () => { StartPlayerTurn(); });
        }
        else
        {
            Debug.Log("EN ESTA ESCENA NO SE JUEGA");

            CurrentTurn = TurnState.NotPlayable;
            if (enemies != null)
            {
                enemies.Clear();
                enemies = null;
            }

            turnChangeFeedback = null;
        }
    }

    private string[] GetDialogueForNode(Node.Difficulty difficulty, int level, int index)
    {
        switch (difficulty)
        {
            case Node.Difficulty.Human:
                switch (level)
                {
                    case 1: return hCombat1;
                    case 2: return hCombat2;
                    case 3: return hCombat3;
                    case 4: return hCombat4;
                    case 5: return hCombat5;
                    case 6: return hCombat6;
                }
                break;

            case Node.Difficulty.NoHuman:
                switch (level)
                {
                    case 1: return nhCombat1;
                    case 2:
                        if (index == 0)
                            return nhCombat2a;
                        else
                            return nhCombat2b;
                    case 3:
                        if (index == 0)
                            return nhCombat3a;
                        else
                            return nhCombat3b;
                    case 4:
                        if (index == 0)
                            return nhCombat4a;
                        else
                            return nhCombat4b;
                    case 5:
                        if (index == 0)
                            return nhCombat5a;
                        else
                            return nhCombat5b;
                    case 6: return nhCombat6;
                }
                break;
        }
        return hCombat1;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void StartPlayerTurn()
    {
        if (CurrentTurn == TurnState.NotPlayable) return;

        Debug.Log("LE TOCA AL JUGADOR");

        player.ResetMana();

        if (player.HandleStatusEffects() == StatusEffect.Numb)
        {
            Debug.Log("El jugador estaba inmovil ha pasado el turno");
            player.RemoveStatus(StatusEffect.Numb);
            EndPlayerTurn();
            return;
        }

        this.turnNumber++;
        CurrentTurn = TurnState.PlayerTurn;
        ShowTurnFeedback("YOUR TURN!");

        if (turnNumber == 1)
        {
            player.StartCombat();
        }
    }

    private void EndPlayerTurn()
    {
        if (CurrentTurn == TurnState.NotPlayable) return;

        Debug.Log("TURNO DE JUGADOR FINALIZADO");
        StartEnemyTurn();
    }

    private void CardPlayed(Card card, IHittable target, GameObject playedCard)
    {
        if (currentTurn != TurnState.SelectingTarget) return;

        playedCard.GetComponent<CardPlayer>().Play(target);

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i].Health <= 0)
            {
                player.RemoveEnemy(enemies[i]);
                enemies.RemoveAt(i);
            }
        }

        Debug.Log("NUMERO DE ENEMIGOS RESTANTES " + enemies.Count);

        if (enemies.Count == 0)
        {
            Debug.Log("GANASTE!");
            CurrentTurn = TurnState.NotPlayable;
            player.ResetPlayer();
            turnNumber = 0;

            if (selectedNodeLevel == 6)
            {
                player.ClearDeck();
                SceneManager.LoadScene("Final");
            }
            else
            {
                resultMenu.ShowRewards(selectedNodeDifficulty, SelectedNodeLevel);
                resultMenu.SuccessSprite.sprite = player.Images[2];
                resultMenu.SuccesMenu.SetActive(true);
            }
        }
        else
        {
            CurrentTurn = TurnState.PlayerTurn;
        }
    }

    private void StartEnemySelection()
    {
        if (CurrentTurn != TurnState.PlayerTurn) return;
        CurrentTurn = TurnState.SelectingTarget;
        player.StartEnemySelecitonMode();
        Debug.Log("Seleccionando enemigo");
    }

    private void CancelEnemySelection()
    {
        if (CurrentTurn != TurnState.SelectingTarget) return;
        CurrentTurn = TurnState.PlayerTurn;
    }

    private void StartEnemyTurn()
    {
        if (CurrentTurn == TurnState.NotPlayable) return;
        CurrentTurn = TurnState.EnemyTurn;

        ShowTurnFeedback("ENEMY TURN!");

        if (enemies == null || enemies.Count == 0)
        {
            Debug.Log("ENEMIGOS DERROTADOS, NO HAY MAS ENEMIGOS");
            StartPlayerTurn();
            return;
        }

        StartCoroutine(HandleEnemiesTurns());
    }

    private IEnumerator HandleEnemiesTurns()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy.Health > 0)
            {
                StatusEffect se = enemy.HandleStatusEffects(player.Health);

                if (se == StatusEffect.None)
                {
                    ShowTurnFeedback($"{enemy.EnemyName} is attacking!!");
                    yield return new WaitForSeconds(1f);
                    Debug.Log("EL ENEMIGO " + enemy.EnemyName + " ESTA ATACANDO!");
                    enemy.Attack(player);
                }
                else if (se == StatusEffect.Numb)
                {
                    ShowTurnFeedback($"{enemy.EnemyName} couldn't move and did nothing.");
                    yield return new WaitForSeconds(1f);
                    Debug.Log("El enemigo no se puede mover. No ataca");
                    enemy.RemoveStatus(StatusEffect.Numb);
                }
                else if (se == StatusEffect.Torment)
                {
                    ShowTurnFeedback($"{enemy.EnemyName} feared you and backed-up.");
                    yield return new WaitForSeconds(1f);
                    Debug.Log("El enemigo te tiene miedo y retrocede. No ataca");
                }
                else if (se == StatusEffect.Death)
                {
                    ShowTurnFeedback($"{enemy.EnemyName} ended up dying.");
                    yield return new WaitForSeconds(1f);
                    Debug.Log("El enemigo se murio a causa de un efecto");
                }

                yield return new WaitForSeconds(1.5f);
            }
        }

        Debug.Log("FIN DEL TURNO DE LOS ENEMIGOS");
        StartPlayerTurn();
    }

    private void ShowTurnFeedback(string msg)
    {
        if (turnChangeFeedback != null)
        {
            turnChangeFeedback.text = msg;
            turnChangeFeedback.gameObject.SetActive(true);
            StartCoroutine(HideFeedbackTextAfterDelay(2f));
        }
    }

    private IEnumerator HideFeedbackTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (turnChangeFeedback != null)
        {
            turnChangeFeedback.text = "";
            turnChangeFeedback.gameObject.SetActive(false);
        }
    }

    private bool AreEnemiesDead()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy.Health > 0)
            {
                return false;
            }
        }

        return true;
    }

    private void PrepareVariables()
    {
        CurrentTurn = TurnState.PlayerTurn;
        turnNumber = 0;
    }
}
