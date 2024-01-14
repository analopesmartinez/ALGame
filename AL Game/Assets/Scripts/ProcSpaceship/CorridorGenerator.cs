using System;
using System.Collections.Generic;
using System.Linq;

public class CorridorGenerator
{
    public List<Node> CreateCorridors(List<RoomNode> allNodesCollection, int corridorWidth)
    {
        List<Node> corridorList = new List<Node>();
        Queue<RoomNode> structuresToCheck = new Queue<RoomNode>(allNodesCollection.OrderByDescending(node => node.TreeLayerIndex).ToList());
        while(structuresToCheck.Count > 0)
        {
            Node node = structuresToCheck.Dequeue();
            if(node.Children.Count == 0)
            {
                continue;
            }
            CorridorNode corridor = new CorridorNode(node.Children[0], node.Children[1], corridorWidth);
            /*if(corridor.Width >= corridorWidth && corridor.Length >= corridorWidth)*/ corridorList.Add(corridor);
        }
        return corridorList;
    }
}