using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI lifeTag;
    [SerializeField] private TextMeshProUGUI manaTag;
    [SerializeField] private GameObject rowanFace;
    [SerializeField] private TextMeshProUGUI combatInfoTag;
    private TurnManager tm;
    private Player player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tm = TurnManager.tm;

        if (tm == null)
        {
            Debug.LogError("NO SE PUDO OBTENER EL TURN MANAGER");
            return;
        }

        player = tm.Player;

        if (player == null)
        {
            Debug.LogError("No se pudo obtener al jugador");
        }

        player.OnLifeChanged += OnLifeChanged;
        player.OnManaChanged += OnManaChanged;

        tm.OnTurnStateChanged += OnTurnStateChanged;

        endTurnButton.onClick.AddListener(OnEndTurnButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        player.OnLifeChanged -= OnLifeChanged;
        player.OnManaChanged -= OnManaChanged;

        tm.OnTurnStateChanged -= OnTurnStateChanged;
    }

    private void OnLifeChanged(int life, int totalLife)
    {
        this.lifeTag.text = String.Format("{0}/{1}", life, totalLife);
    }

    private void OnManaChanged(int mana)
    {
        this.manaTag.text = String.Format("{0}", mana);
    }

    private void OnTurnStateChanged(TurnManager.TurnState state)
    {
        switch (state)
        {
            case TurnManager.TurnState.PlayerTurn:
                endTurnButton.enabled = true;
                combatInfoTag.text = "YOUR TURN!";
                break;

            case TurnManager.TurnState.EnemyTurn:
                endTurnButton.enabled = false;
                combatInfoTag.text = "ENEMY TURN!";
                break;

            case TurnManager.TurnState.SelectingTarget:
                endTurnButton.enabled = true;
                combatInfoTag.text = "SELECT A TARGET!";
                break;

            case TurnManager.TurnState.NotPlayable:
                endTurnButton.enabled = false;
                combatInfoTag.text = "COMBAT ENDED!";
                break;
        }
    }

    private void OnEndTurnButtonClicked()
    {
        if (tm.CurrentTurn == TurnManager.TurnState.PlayerTurn)
        {
            player.EndTurn();
        }
    }
}
