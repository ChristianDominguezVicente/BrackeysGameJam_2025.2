using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private int turnNumber = 0;

    [Header("Enemies Human Prefab")]
    [SerializeField] private GameObject salesmanPrefab;
    [SerializeField] private GameObject drunkPrefab;
    [SerializeField] private GameObject teacherPrefab;
    [SerializeField] private GameObject exConvictPrefab;
    [SerializeField] private GameObject junkiesPrefab;
    [SerializeField] private GameObject yakuzaPrefab;

    [Header("Enemies NoHuman Prefab")]
    [SerializeField] private GameObject racoonPrefab;
    [SerializeField] private GameObject ratPrefab;
    [SerializeField] private GameObject mutantRatPrefab;
    [SerializeField] private GameObject mutantDogPrefab;
    [SerializeField] private GameObject wtfPrefab;

    private int selectedNodeLevel;
    private Node.Difficulty selectedNodeDifficulty;

    public int SelectedNodeLevel { get => selectedNodeLevel; set => selectedNodeLevel = value; }
    public Node.Difficulty SelectedNodeDifficulty { get => selectedNodeDifficulty; set => selectedNodeDifficulty = value; }

    public int TurnNumber
    {
        get { return turnNumber; }
    }

    private TurnState currentTurn;

    public TurnState CurrentTurn
    {
        get { return currentTurn; }
    }

    private List<Enemy> enemies;

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
        }
    }

    public void GenerateEnemies(Node.Difficulty difficulty, int level)
    {
        switch (difficulty)
        {
            case Node.Difficulty.Human:
                InstantiateHumanEnemy(level);
                break;
            case Node.Difficulty.NoHuman:
                InstantiateNoHumanEnemy(level);
                break;
        }
    }

    private void SpawnEnemies(GameObject[] prefabs)
    {
        if (enemies == null)
            enemies = new List<Enemy>();

        Vector3 centerPosition = new Vector3(0, 1.5f, 0);
        float spacing = 2f;
        int enemyCount = prefabs.Length;

        for (int i = 0; i < enemyCount; i++)
        {
            Enemy newEnemy = Instantiate(prefabs[i]).GetComponent<Enemy>();
            float offset = (i - (enemyCount - 1) / 2f) * spacing;
            newEnemy.transform.position = centerPosition + new Vector3(offset, 0, 0);
            enemies.Add(newEnemy);
        }
    }

    private void InstantiateHumanEnemy(int level)
    {
        switch (level)
        {
            case 1:
                SpawnEnemies(new GameObject[] { salesmanPrefab });
                break;
            case 2:
                SpawnEnemies(new GameObject[] { drunkPrefab });
                break;
            case 3:
                SpawnEnemies(new GameObject[] { teacherPrefab });
                break;
            case 4:
                SpawnEnemies(new GameObject[] { exConvictPrefab });
                break;
            case 5:
                SpawnEnemies(new GameObject[] { junkiesPrefab, junkiesPrefab });
                break;
            case 6:
                SpawnEnemies(new GameObject[] { yakuzaPrefab, yakuzaPrefab });
                break;
        }
    }

    private void InstantiateNoHumanEnemy(int level)
    {
        switch (level)
        {
            case 1:
                SpawnEnemies(new GameObject[] { racoonPrefab });
                break;
            case 2:
                if (Random.value < 0.5f)
                    SpawnEnemies(new GameObject[] { ratPrefab, racoonPrefab });
                else
                    SpawnEnemies(new GameObject[] { ratPrefab, mutantRatPrefab, ratPrefab });
                break;
            case 3:
                if (Random.value < 0.5f)
                    SpawnEnemies(new GameObject[] { ratPrefab, racoonPrefab, ratPrefab });
                else
                    SpawnEnemies(new GameObject[] { mutantRatPrefab, mutantDogPrefab, mutantRatPrefab });
                break;
            case 4:
                if (Random.value < 0.5f)
                    SpawnEnemies(new GameObject[] { ratPrefab, racoonPrefab, ratPrefab, racoonPrefab, ratPrefab });
                else
                    SpawnEnemies(new GameObject[] { mutantRatPrefab, mutantDogPrefab, mutantRatPrefab, mutantRatPrefab });
                break;
            case 5:
                if (Random.value < 0.5f)
                    SpawnEnemies(new GameObject[] { ratPrefab, ratPrefab, ratPrefab, ratPrefab, ratPrefab });
                else
                    SpawnEnemies(new GameObject[] { mutantDogPrefab, mutantRatPrefab, ratPrefab, mutantRatPrefab, mutantDogPrefab });
                break;
            case 6:
                SpawnEnemies(new GameObject[] { wtfPrefab, mutantDogPrefab, mutantRatPrefab, ratPrefab, racoonPrefab });
                break;
        }
    }

    private void InitializeTurnManager(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TestScene")
        {
            Debug.Log("EMPEZANDO EL MANEJO DE TURNOS");

            PrepareVariables();
            GenerateEnemies(selectedNodeDifficulty, selectedNodeLevel);
            StartPlayerTurn();
        }
        else
        {
            Debug.Log("EN ESTA ESCENA NO SE JUEGA");

            currentTurn = TurnState.NotPlayable;
            if (enemies != null)
            {
                enemies.Clear();
                enemies = null;
            }
        }
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
        if (currentTurn == TurnState.NotPlayable) return;

        Debug.Log("LE TOCA AL JUGADOR");
        currentTurn = TurnState.PlayerTurn;

        this.turnNumber++;

        if (turnNumber == 1)
        {
            player.DrawHand();
        }
    }

    private void EndPlayerTurn()
    {
        if (currentTurn == TurnState.NotPlayable) return;

        Debug.Log("TURNO DE JUGADOR FINALIZADO");
        StartEnemyTurn();
    }

    private void CardPlayed(Card card, IHittable target, GameObject playedCard)
    {
        if (currentTurn != TurnState.SelectingTarget) return;

        card.OnActivated(target);

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i].Health <= 0)
            {
                player.RemoveEnemy(enemies[i]);
                enemies.RemoveAt(i);
            }
        }

        Debug.Log("NUMERO DE ENEMIGOS RESTANTES " + enemies.Count);

        player.OnCardUsed(playedCard);

        if (enemies.Count == 0)
        {
            Debug.Log("GANASTE!");
            currentTurn = TurnState.NotPlayable;
            player.ResetPlayer();
            turnNumber = 0;

            if (selectedNodeLevel == 6)
            {
                SceneManager.LoadScene("Menu");
            }
            else
            {
                SceneManager.LoadScene("Map");
            }
        }
        else
        {
            EndPlayerTurn();
        }
    }

    private void StartEnemySelection()
    {
        if (currentTurn != TurnState.PlayerTurn) return;
        currentTurn = TurnState.SelectingTarget;
        Debug.Log("Seleccionando enemigo");
    }

    private void CancelEnemySelection()
    {
        if (currentTurn != TurnState.SelectingTarget) return;
        currentTurn = TurnState.PlayerTurn;
    }

    private void StartEnemyTurn()
    {
        if (currentTurn == TurnState.NotPlayable) return;
        currentTurn = TurnState.EnemyTurn;

        if (enemies == null || enemies.Count == 0)
        {
            Debug.Log("ENEMIGOS DERROTADOS, NO HAY MAS ENEMIGOS");
            StartPlayerTurn();
            return;
        }

        foreach (Enemy enemy in enemies)
        {
            if (enemy.Health > 0)
            {
                Debug.Log("EL ENEMIGO " + enemy.name + " ESTA ATACANDO!");
            }
        }

        Debug.Log("FIN DEL TURNO DE LOS ENEMIGOS");
        StartPlayerTurn();
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
        currentTurn = TurnState.PlayerTurn;
        turnNumber = 0;
    }
}
