using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StructureHelper
{
    public static List<Node> TraverseGraphToExtractLowestLeaves(Node parentNode)
    {
        Queue<Node> nodesToCheck = new Queue<Node>();
        List<Node> listToReturn = new List<Node>();
        if (parentNode.Children.Count == 0)
        {
            return new List<Node>() { parentNode };
        }
        foreach (Node child in parentNode.Children)
        {
            nodesToCheck.Enqueue(child);
        }
        while (nodesToCheck.Count > 0)
        {
            Node currentNode = nodesToCheck.Dequeue();
            if (currentNode.Children.Count == 0)
            {
                listToReturn.Add(currentNode);
            }
            else
            {
                foreach (Node child in currentNode.Children)
                {
                    nodesToCheck.Enqueue(child);
                }
            }
        }
        return listToReturn;
    }

    public static Vector2 GenerateBottomLeftCornerBetween(Vector2 leftBoundary, Vector2 rightBoundary, float pointModifier, float offset)
    {
        float minX = leftBoundary.x + offset;
        float maxX = rightBoundary.x - offset;
        float minY = leftBoundary.y + offset;
        float maxY = rightBoundary.y - offset;
        return new Vector2(
            Random.Range(minX, (minX + (maxX - minX) * pointModifier)),
            Random.Range(minY, (minY + (maxY - minY) * pointModifier)));
    }

    public static Vector2 GenerateTopRightCornerBetween(Vector2 leftBoundary, Vector2 rightBoundary, float pointModifier, int offset)
    {
        float minX = leftBoundary.x + offset;
        float maxX = rightBoundary.x - offset;
        float minY = leftBoundary.y + offset;
        float maxY = rightBoundary.y - offset;
        return new Vector2(
            Random.Range((int)(minX + (maxX - minX) * pointModifier), maxX),
            Random.Range((int)(minY + (maxY - minY) * pointModifier), maxY));
    }

    public static Vector2 CalculateMiddlePoint(Vector2 vector1, Vector2 vector2)
    {
        Vector2 sum = vector1 + vector2;
        Vector2 temptVector = sum / 2;
        return new Vector2((int)temptVector.x, (int)temptVector.y);
    }
}

public enum RelativePosition
{
    Up,
    Down,
    Left,
    Right
}