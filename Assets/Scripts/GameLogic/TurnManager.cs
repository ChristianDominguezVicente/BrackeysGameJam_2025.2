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

    }

    private void InitializeTurnManager(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TestScene")
        {
            Debug.Log("EMPEZANDO EL MANEJO DE TURNOS");

            PrepareVariables();
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
        enemies = new List<Enemy>(FindObjectsByType<Enemy>(FindObjectsSortMode.None));
        currentTurn = TurnState.PlayerTurn;
        turnNumber = 0;
    }
}
