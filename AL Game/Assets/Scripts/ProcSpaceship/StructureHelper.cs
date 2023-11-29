using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StructureHelper
{
    public static List<Node> TraverseGraphToExtractLowestLeaves(RoomNode parentNode)
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

    public static Vector2Int GenerateBottomLeftCornerBetween(Vector2Int leftBoundary, Vector2Int rightBoundary, float pointModifier, int offset)
    {
        int minX = leftBoundary.x + offset;
        int maxX = rightBoundary.x - offset;
        int minY = leftBoundary.y + offset;
        int maxY = rightBoundary.y - offset;
        return new Vector2Int(
            Random.Range(minX, (int)(minX + (maxX - minX) * pointModifier)),
            Random.Range(minY, (int)(minY + (maxY - minY) * pointModifier)));
    }

    public static Vector2Int GenerateTopRightCornerBetween(Vector2Int leftBoundary, Vector2Int rightBoundary, float pointModifier, int offset)
    {
        int minX = leftBoundary.x + offset;
        int maxX = rightBoundary.x - offset;
        int minY = leftBoundary.y + offset;
        int maxY = rightBoundary.y - offset;
        return new Vector2Int(
            Random.Range((int)(minX + (maxX - minX) * pointModifier), maxX),
            Random.Range((int)(minY + (maxY - minY) * pointModifier), maxY));
    }
}