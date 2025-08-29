using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Map : MonoBehaviour
{
    public static Map map;

    [Header("Inputs")]
    [SerializeField] private InputManagerSO inputManager;

    [Header("Settings")]
    [SerializeField] private float lineWidth;
    [SerializeField] private Vector3[] nodePositions;

    [Header("Prefabs")]
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject linePrefab;

    [Header("Line Materials")]
    [SerializeField] private Material solidLineMat;
    [SerializeField] private Material dashedLineMat;

    [Header("Sounds")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;

    private int[] nodesPerLevel = new int[] { 1, 2, 3, 3, 3, 3, 2, 1 };

    private List<List<Node>> graph = new List<List<Node>>();
    private List<Line> lines = new List<Line>();

    private int currentLevel = 0;
    private Node hoveredNode;
    private int hoveredIndex = 0;
    private bool mapGenerated = false;

    private PauseMenu pause;
    private ConfirmationMenu confirmationMenu;

    private bool inputLocked = false;

    private Stack<Node> selectionHistory = new Stack<Node>();

    private AudioSource audioSFX;

    public ConfirmationMenu ConfirmationMenu { get => confirmationMenu; set => confirmationMenu = value; }
    public bool InputLocked { get => inputLocked; set => inputLocked = value; }

    void Awake()
    {
        if (map == null)
        {
            map = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
            return;
        }

        Destroy(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Map")
        {
            pause = FindFirstObjectByType<PauseMenu>();
            confirmationMenu = FindFirstObjectByType<ConfirmationMenu>();
            audioSFX = GameObject.Find("AudioSFX").GetComponent<AudioSource>();
            gameObject.SetActive(true);
            if (!mapGenerated)
            {
                GenerateMap();
                mapGenerated = true;
            }
            inputLocked = false;
        }
        else if (scene.name == "Menu")
        {
            ClearMap();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
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
        ClearMap();

        int globalIndex = 0;

        for (int i = 0; i < nodesPerLevel.Length; i++)
        {
            int nodeCount = nodesPerLevel[i];
            List<Node> levelNodes = new List<Node>();

            for (int j = 0; j < nodeCount; j++)
            {
                Vector3 pos = nodePositions[globalIndex];

                GameObject node = Instantiate(nodePrefab, pos, Quaternion.identity, transform);
                node.name = $"Node L{i} N{j}";

                Node script = node.AddComponent<Node>();
                script.Level = i;
                script.Index = j;
                script.Map = this;
                script.RoomDifficulty = GetNodeDifficulty(i, j);
                script.HoverSound = hoverSound;
                script.ClickSound = clickSound;

                node.GetComponent<SpriteRenderer>().color = script.GetDefaultColor();

                levelNodes.Add(script);

                node.GetComponent<CircleCollider2D>().enabled = (i == 0 && GetNodeDifficulty(i, j) != Node.Difficulty.Non);

                globalIndex++;
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
        if (node.RoomDifficulty == Node.Difficulty.Non) return false;
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
        selectionHistory.Push(node);

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
            hoveredNode.GetComponent<SpriteRenderer>().color = hoveredNode.GetDefaultColor();

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
                line.LineRenderer.material = solidLineMat;
                line.LineRenderer.startColor = Color.green;
                line.LineRenderer.endColor = Color.green;
            }
            else if (hoveredNode != null && ((line.Origin.IsSelected && line.Destination == hoveredNode) || (line.Destination.IsSelected && line.Origin == hoveredNode)))
            {
                line.LineRenderer.material = solidLineMat;
                line.LineRenderer.startColor = Color.yellow;
                line.LineRenderer.endColor = Color.yellow;
            }
            else if ((line.Origin.IsSelected && !line.Destination.IsSelected && CanSelectNode(line.Destination)) || (line.Destination.IsSelected && !line.Origin.IsSelected && CanSelectNode(line.Origin)))
            {
                line.LineRenderer.material = dashedLineMat;
                line.LineRenderer.startColor = Color.white;
                line.LineRenderer.endColor = Color.white;
            }
            else
            {
                line.LineRenderer.material = solidLineMat;
                line.LineRenderer.startColor = new Color(0, 0, 0, 0);
                line.LineRenderer.endColor = new Color(0, 0, 0, 0);
            }
        }
    }

    private Node.Difficulty GetNodeDifficulty(int level, int index)
    {
        if (level == 7 && index == 0) return Node.Difficulty.Non;

        switch (level)
        {
            case 1:
                return (index == 0) ? Node.Difficulty.Human : Node.Difficulty.NoHuman;
            case 2:
                if (index == 0) return Node.Difficulty.NoHuman;
                if (index == 1) return Node.Difficulty.Human;
                return Node.Difficulty.NoHuman;
            case 3:
                if (index == 2) return Node.Difficulty.Human;
                return Node.Difficulty.NoHuman;
            case 4:
                if (index == 2) return Node.Difficulty.Human;
                return Node.Difficulty.NoHuman;
            case 5:
                if (index == 2) return Node.Difficulty.Human;
                return Node.Difficulty.NoHuman;
            case 6:
                if (index == 1) return Node.Difficulty.Human;
                return Node.Difficulty.NoHuman;
        }
        return Node.Difficulty.NoHuman;
    }

    private void Move(Vector2 ctx)
    {
        if (pause != null && pause.IsPaused) return;
        if (inputLocked) return;
        if (currentLevel >= graph.Count - 1) return;

        Node selected = graph[currentLevel].Find(n => n.IsSelected);
        if (selected == null || selected.ConnectedNodes.Count == 0) return;

        if (Mathf.Abs(ctx.x) > 0.5f)
        {
            audioSFX.PlayOneShot(hoverSound);
            if (ctx.x > 0) hoveredIndex++;
            else hoveredIndex--;

            hoveredIndex = Mathf.Clamp(hoveredIndex, 0, selected.ConnectedNodes.Count - 1);

            SetHoveredNode(selected.ConnectedNodes[hoveredIndex]);
        }
    }

    private void Action()
    {
        if (pause != null && pause.IsPaused) return;
        if (inputLocked) return;

        if (hoveredNode != null && CanSelectNode(hoveredNode))
        {
            audioSFX.PlayOneShot(clickSound);
            confirmationMenu.Initialize(this, hoveredNode);
        }    
    }

    public void ConfirmNodeSelection(Node node)
    {
        inputLocked = true;
        SelectNode(node);
        hoveredIndex = 0;

        TurnManager.tm.SelectedNodeLevel = node.Level;
        TurnManager.tm.SelectedNodeIndex = node.Index;
        TurnManager.tm.SelectedNodeDifficulty = node.RoomDifficulty;

        SceneManager.LoadScene("TestScene");
    }

    private void ClearMap()
    {
        foreach (var level in graph)
            foreach (var node in level)
                if (node != null)
                    Destroy(node.gameObject);

        foreach (var line in lines)
            if (line.LineRenderer != null)
                Destroy(line.LineRenderer.gameObject);

        graph.Clear();
        lines.Clear();
        currentLevel = 0;
        hoveredNode = null;
        hoveredIndex = 0;
        mapGenerated = false;
    }

    public void RevertSelection()
    {
        if (selectionHistory.Count > 1)
        {
            Node lastNode = selectionHistory.Pop();
            lastNode.IsSelected = false;
            lastNode.GetComponent<SpriteRenderer>().color = lastNode.GetDefaultColor();

            Node previousNode = selectionHistory.Peek();
            currentLevel = previousNode.Level;

            foreach (Node n in graph[currentLevel + 1])
                n.GetComponent<CircleCollider2D>().enabled = false;

            foreach (Node c in previousNode.ConnectedNodes)
                c.GetComponent<CircleCollider2D>().enabled = true;

            hoveredIndex = 0;
            if (previousNode.ConnectedNodes.Count > 0)
                SetHoveredNode(previousNode.ConnectedNodes[0]);

            UpdateLines();
        }
    }
}
