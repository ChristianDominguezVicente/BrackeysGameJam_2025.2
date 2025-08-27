using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Node : MonoBehaviour
{
    public enum Difficulty
    {
        Human,
        NoHuman, 
        Non
    }

    private Map map;

    private SpriteRenderer sprite;
    private bool isSelected = false;

    private int level;
    private int index;

    private List<Node> connectedNodes = new List<Node>();

    private Difficulty difficulty;

    public Difficulty RoomDifficulty { get => difficulty; set => difficulty = value; }

    public bool IsSelected { get => isSelected; set => isSelected = value; }
    public int Level { get => level; set => level = value; }
    public int Index { get => index; set => index = value; }
    public List<Node> ConnectedNodes { get => connectedNodes; set => connectedNodes = value; }
    public Map Map { get => map; set => map = value; }

    private void Start()
    {
        map = GetComponentInParent<Map>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnMouseEnter()
    {
        if (!isSelected && map.CanSelectNode(this))
            map.SetHoveredNode(this);
    }

    private void OnMouseDown()
    {
        if (map.CanSelectNode(this))
        {
            map.SelectNode(this);
            isSelected = true;
            sprite.color = Color.green;

            TurnManager.tm.SelectedNodeLevel = this.Level;
            TurnManager.tm.SelectedNodeDifficulty = this.RoomDifficulty;

            SceneManager.LoadScene("TestScene");
        }
    }

    public Color GetDefaultColor()
    {
        switch (difficulty)
        {
            case Difficulty.Human:
                return new Color(1f, 0.5f, 0f);
            case Difficulty.NoHuman:
                return new Color(0.5f, 0f, 0.5f);
            case Difficulty.Non:
                return Color.gray;
            default:
                return Color.white;
        }
    }
}
