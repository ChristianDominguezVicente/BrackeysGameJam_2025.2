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

    }

    private void OnDisable()
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
    }

    private void EndPlayerTurn()
    {
        if (currentTurn == TurnState.NotPlayable) return;

        Debug.Log("TURNO DE JUGADOR FINALIZADO");
        currentTurn = TurnState.EnemyTurn;
    }

    private void PrepareVariables()
    {
        enemies = new List<Enemy>(FindObjectsByType<Enemy>(FindObjectsSortMode.None));
        currentTurn = TurnState.PlayerTurn;
    }
}
