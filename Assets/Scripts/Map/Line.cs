using UnityEngine;

[System.Serializable]
public class Line
{
    private Node origin;
    private Node destination;
    private LineRenderer lineRenderer;

    public Node Origin { get => origin; set => origin = value; }
    public Node Destination { get => destination; set => destination = value; }
    public LineRenderer LineRenderer { get => lineRenderer; set => lineRenderer = value; }

    public Line(Node origin, Node destination, LineRenderer line)
    {
        this.origin = origin;
        this.destination = destination;
        this.lineRenderer = line;
    }
}
