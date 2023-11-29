using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class Node
{
    private List<Node> _children;
    public List<Node> Children { get { return _children; } }
    public bool Visited { get; set; }
    public Vector2Int BottomLeftCorner { get; set; }
    public Vector2Int BottomRightCorner { get; set; }
    public Vector2Int TopLeftCorner { get; set; }
    public Vector2Int TopRightCorner { get; set; }

    public Node Parent { get; set; }

    public int TreeLayerIndex { get; set; }

    public Node(Node parentNode)
    {
        _children = new List<Node>();
        this.Parent = parentNode;
        if (parentNode != null)
        {
            parentNode.AddChild(this);
        }
    }

    public void AddChild(Node node)
    {
        _children.Add(node);
    }

    public void RemoveChild(Node node)
    {
        _children.Remove(node);
    }
}