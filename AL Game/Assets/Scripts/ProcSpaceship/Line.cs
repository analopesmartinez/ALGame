using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Line
{
    Orientation orientation;
    Vector2 coordinates;

    public Line(Orientation orientation, Vector2 coordinates)
    {
        this.Orientation = orientation;
        this.Coordinates = coordinates;
    }

    public Orientation Orientation { get => orientation; set => orientation = value; }
    public Vector2 Coordinates { get => coordinates; set => coordinates = value; }
}
public enum Oritentation
{
    Horizontal = 0,
    Vertical = 1
}