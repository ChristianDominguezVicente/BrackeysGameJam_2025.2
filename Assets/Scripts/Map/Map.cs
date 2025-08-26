using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Map : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputManagerSO inputManager;

    [Header("Settings")]
    [SerializeField] private float xSpacing;
    [SerializeField] private float ySpacing;
    [SerializeField] private float lineWidth;
    [SerializeField] private Vector2 offset;

    [Header("Prefabs")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject linePrefab;

    private int[] nodesPerLevel = new int[] { 1, 2, 3, 3, 3, 3, 2, 1 };

    private List<List<Node>> graph = new List<List<Node>>();
    private List<Line> lines = new List<Line>();

    private int currentLevel = 0;
    private Node hoveredNode;
    private int hoveredIndex = 0;

    private void OnEnable()
    {
        inputManager.OnMove += Move;
        inputManager.OnAction += Action;
    }

    private void OnDisable()
    {
        inputManager.OnMove -= Move;
        inputManager.OnAction -= Action;
    }

    public void GenerateMap()
    {
        foreach (var level in graph) 
            foreach(var node in level)
                Destroy(node);
        foreach (var line in lines) Destroy(line.LineRenderer);
        graph.Clear();
        lines.Clear();
        currentLevel = 0;

        for (int i = 0; i < nodesPerLevel.Length; i++)
        {
            int nodeCount = nodesPerLevel[i];
            List<Node> levelNodes = new List<Node>();

            for (int j = 0; j < nodeCount; j++)
            {
                Vector3 pos = new Vector3(i * xSpacing + offset.x, j * ySpacing - (nodeCount - 1) * ySpacing / 2f + offset.y, 0);

                GameObject node = Instantiate(nodePrefab, pos, Quaternion.identity, transform);
                node.name = $"Node L{i} N{j}";

                Node script = node.AddComponent<Node>();
                script.Level = i;
                script.Index = j;
                script.Map = this;
                levelNodes.Add(script);

                node.GetComponent<CircleCollider2D>().enabled = (i == 0);
            }

            graph.Add(levelNodes);
        }

        GenerateConnections();

        Node firstNode = graph[0][0];
        SelectNode(firstNode);
        firstNode.GetComponent<SpriteRenderer>().color = Color.green;

        if (firstNode.ConnectedNodes.Count > 0)
        {
            hoveredIndex = 0;
            SetHoveredNode(firstNode.ConnectedNodes[0]);
        }
    }

    private void GenerateConnections()
    {
        var connections = new (int level, int index, (int level, int index)[] targets)[]
        {
            (0,0,new[]{(1,0),(1,1)}),
            (1,0,new[]{(2,0),(2,1)}),
            (1,1,new[]{(2,2)}),
            (2,0,new[]{(3,0)}),
            (2,1,new[]{(3,1),(3,2)}),
            (2,2,new[]{(3,1),(3,2)}),
            (3,0,new[]{(4,0),(4,1)}),
            (3,1,new[]{(4,0),(4,1)}),
            (3,2,new[]{(4,1),(4,2)}),
            (4,0,new[]{(5,0),(5,1)}),
            (4,1,new[]{(5,0),(5,1)}),
            (4,2,new[]{(5,2)}),
            (5,0,new[]{(6,0)}),
            (5,1,new[]{(6,1)}),
            (5,2,new[]{(6,1)}),
            (6,0,new[]{(7,0)}),
            (6,1,new[]{(7,0)}),
        };

        foreach (var c in connections)
        {
            Node origin = graph[c.level][c.index];
            foreach (var targetPos in c.targets)
            {
                Node destination = graph[targetPos.level][targetPos.index];
                origin.ConnectedNodes.Add(destination);
                Connection(origin, destination);
            }
        }
    }

    private void Connection(Node origin, Node destination)
    {
        GameObject line = Instantiate(linePrefab, transform);
        LineRenderer lr = line.GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.SetPosition(0, origin.transform.position);
        lr.SetPosition(1, destination.transform.position);

        lines.Add(new Line(origin, destination, lr));
    }

    public bool CanSelectNode(Node node)
    {
        if (currentLevel == 0 && node.Level == 0 && !node.IsSelected) return true;

        if(node.Level == currentLevel + 1)
        {
            foreach (Node n in graph[currentLevel])
                if (n.IsSelected && n.ConnectedNodes.Contains(node)) return true;
        }
        return false;
    }

    public void SelectNode(Node node)
    {
        node.IsSelected = true;
        node.GetComponent<SpriteRenderer>().color = Color.green;
        currentLevel = node.Level;

        if (currentLevel + 1 < graph.Count)
        {
            foreach (Node n in graph[currentLevel + 1])
                n.GetComponent<CircleCollider2D>().enabled = false;

            foreach (Node c in node.ConnectedNodes)
                c.GetComponent<CircleCollider2D>().enabled = true;
        }

        if (node.ConnectedNodes.Count > 0)
        {
            hoveredIndex = 0;
            SetHoveredNode(node.ConnectedNodes[0]);
        }

        UpdateLines();
    }

    public void SetHoveredNode(Node node)
    {
        if (hoveredNode != null && !hoveredNode.IsSelected)
            hoveredNode.GetComponent<SpriteRenderer>().color = Color.white;

        hoveredNode = node;

        if (hoveredNode != null && !hoveredNode.IsSelected)
            hoveredNode.GetComponent<SpriteRenderer>().color = Color.yellow;

        UpdateLines();
    }

    public void ClearHoveredNode(Node node)
    {
        if (hoveredNode == node)
            hoveredNode = null;
        UpdateLines();
    }

    private void UpdateLines()
    {
        foreach (Line line in lines)
        {
            if (line.Origin.IsSelected && line.Destination.IsSelected)
            {
                line.LineRenderer.startColor = Color.green;
                line.LineRenderer.endColor = Color.green;
            }
            else if (hoveredNode != null && ((line.Origin.IsSelected && line.Destination == hoveredNode) || (line.Destination.IsSelected && line.Origin == hoveredNode)))
            {
                line.LineRenderer.startColor = Color.yellow;
                line.LineRenderer.endColor = Color.yellow;
            }
            else
            {
                line.LineRenderer.startColor = Color.magenta;
                line.LineRenderer.endColor = Color.magenta;
            }
        }
    }

    private void Move(Vector2 ctx)
    {
        if (currentLevel >= graph.Count - 1) return;

        Node selected = graph[currentLevel].Find(n => n.IsSelected);
        if (selected == null || selected.ConnectedNodes.Count == 0) return;

        if (Mathf.Abs(ctx.y) > 0.5f)
        {
            if (ctx.y > 0) hoveredIndex++;
            else hoveredIndex--;

            hoveredIndex = Mathf.Clamp(hoveredIndex, 0, selected.ConnectedNodes.Count - 1);

            SetHoveredNode(selected.ConnectedNodes[hoveredIndex]);
        }
    }

    private void Action()
    {
        if (hoveredNode != null && CanSelectNode(hoveredNode))
        {
            SelectNode(hoveredNode);
            hoveredIndex = 0;
        }
    }
}
