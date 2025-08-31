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

    private AudioClip hoverSound;
    private AudioClip clickSound;

    private AudioSource audioSFX;

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
    public AudioClip HoverSound { get => hoverSound; set => hoverSound = value; }
    public AudioClip ClickSound { get => clickSound; set => clickSound = value; }
    public AudioSource AudioSFX { get => audioSFX; set => audioSFX = value; }

    private void Start()
    {
        map = GetComponentInParent<Map>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnMouseEnter()
    {
        if (!isSelected && map.CanSelectNode(this) && !map.InputLocked)
        {
            audioSFX.PlayOneShot(hoverSound);
            map.SetHoveredNode(this);
        }
            
    }

    private void OnMouseDown()
    {
        if (!map.InputLocked && map.CanSelectNode(this))
        {
            audioSFX.PlayOneShot(clickSound);
            map.ConfirmationMenu.Initialize(map, this);
        }      
    }

    public Color GetDefaultColor()
    {
        switch (difficulty)
        {
            case Difficulty.Human:
                return new Color(249f / 255f, 86f / 255f, 79f / 255f);
            case Difficulty.NoHuman:
                return new Color(123f / 255f, 30f / 255f, 122f / 255f);
            case Difficulty.Non:
                return new Color(12f / 255f, 10f / 255f, 62f / 255f);
            default:
                return Color.white;
        }
    }
}
