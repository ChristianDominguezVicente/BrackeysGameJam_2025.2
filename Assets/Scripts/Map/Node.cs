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

    private void Start()
    {
        map = GetComponentInParent<Map>();
        sprite = GetComponent<SpriteRenderer>();
        audioSFX = GameObject.Find("AudioSFX").GetComponent<AudioSource>();
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
