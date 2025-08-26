using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private Map map;

    private SpriteRenderer sprite;
    private bool isSelected = false;

    private int level;
    private int index;

    private List<Node> connectedNodes = new List<Node>();

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
        }
    }
}
