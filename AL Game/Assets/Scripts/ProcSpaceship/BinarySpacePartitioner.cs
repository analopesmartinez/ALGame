using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;
public class BinarySpacePartitioner
{
    RoomNode rootNode;

    public RoomNode RootNode { get { return rootNode; } }

    public BinarySpacePartitioner(int shipWidth, int shipLength)
    {
        this.rootNode = new RoomNode(new Vector2(0, 0), new Vector2(shipWidth, shipLength), null, 0, false);
    }

    public List<RoomNode> PrepareNodesCollection(int maxIterations, int roomWidthMin, int roomLengthMin, GameObject[] premadeRooms, int premadeRoomOffset, out List<RoomNode> premadeRoomNodes)
    {
        Queue<RoomNode> graph = new Queue<RoomNode>();
        List<RoomNode> listToReturn = new List<RoomNode>();
        premadeRoomNodes = new List<RoomNode>();
        graph.Enqueue(this.rootNode);
        listToReturn.Add(this.rootNode);
        int iterations = 0;
        //Pre-Split Space
        RoomNode startNode = graph.Dequeue();
        SplitTheSpaceExact(startNode, listToReturn, startNode.TopRightCorner / 1.6f, roomWidthMin, roomLengthMin, false, graph);
        //Generate Predefined Rooms
        foreach (GameObject room in premadeRooms)
        {
            PremadeRoom roomData = room.GetComponent<PremadeRoom>();
            Vector2 premadeRoomDimensions = roomData.GetDimensions() + new Vector2(premadeRoomOffset, premadeRoomOffset);

            bool splitSuccessful = false;
            List<RoomNode> nodesToEnqueue = new List<RoomNode>();
            while (iterations < maxIterations && graph.Count > 0 && !splitSuccessful)
            {
                iterations++;
                RoomNode currentNode = graph.Dequeue();
                if ((currentNode.Width >= premadeRoomDimensions.x && currentNode.Length >= premadeRoomDimensions.y) && currentNode.isIndivisible == false)
                {
                    RoomNode nodeToAdd = SplitTheSpaceExact(currentNode, listToReturn, premadeRoomDimensions, roomWidthMin, roomLengthMin, true, graph);
                    premadeRoomNodes.Add(nodeToAdd);
                    splitSuccessful = true;
                }
                else
                {
                    graph.Enqueue(currentNode);
                }
            }
        }

        //Generate Remaining Rooms
        iterations = 0;
        while (iterations < maxIterations && graph.Count > 0)
        {
            iterations++;
            RoomNode currentNode = graph.Dequeue();
            if ((currentNode.Width >= roomWidthMin * 2 || currentNode.Length >= roomLengthMin * 2) && currentNode.isIndivisible == false)
            {
                SplitTheSpace(currentNode, listToReturn, roomLengthMin, roomWidthMin, graph);
            }
        }

        return listToReturn;
    }

    private void SplitTheSpace(RoomNode currentNode, List<RoomNode> listToReturn, int roomLengthMin, int roomWidthMin, Queue<RoomNode> graph)
    {
        Line line = GetLineDividingSpace(currentNode.BottomLeftCorner, currentNode.TopRightCorner, roomWidthMin, roomLengthMin);
        RoomNode node1, node2;
        if (line.Orientation == Orientation.Horizontal)
        {
            node1 = new RoomNode(currentNode.BottomLeftCorner, new Vector2(currentNode.TopRightCorner.x, line.Coordinates.y), currentNode, currentNode.TreeLayerIndex + 1, false);
            node2 = new RoomNode(new Vector2(currentNode.BottomLeftCorner.x, line.Coordinates.y), currentNode.TopRightCorner, currentNode, currentNode.TreeLayerIndex + 1, false);
        }
        else
        {
            node1 = new RoomNode(currentNode.BottomLeftCorner, new Vector2(line.Coordinates.x, currentNode.TopRightCorner.y), currentNode, currentNode.TreeLayerIndex + 1, false);
            node2 = new RoomNode(new Vector2(line.Coordinates.x, currentNode.BottomLeftCorner.y), currentNode.TopRightCorner, currentNode, currentNode.TreeLayerIndex + 1, false);
        }
        AddNewNodesToCollections(listToReturn, graph, node1);
        AddNewNodesToCollections(listToReturn, graph, node2);
    }

    private RoomNode SplitTheSpaceExact(RoomNode currentNode, List<RoomNode> listToReturn, Vector2 dimensions, float roomWidthMin, float roomLengthMin, bool isIndivisible, Queue<RoomNode> graph)
    {
        RoomNode node1;
        RoomNode node2;
        RoomNode indivisibleNode;
        int topOrBottom = Random.Range(0, 2);
        if (topOrBottom == 0)
        {
            node1 = new RoomNode(currentNode.BottomLeftCorner, new Vector2(currentNode.TopRightCorner.x, currentNode.TopRightCorner.y - dimensions.y), currentNode, currentNode.TreeLayerIndex + 1, false);
            node2 = new RoomNode(new Vector2(currentNode.BottomLeftCorner.x, currentNode.TopRightCorner.y - dimensions.y), currentNode.TopRightCorner, currentNode, currentNode.TreeLayerIndex + 1, false);
        }
        else
        {
            node1 = new RoomNode(new Vector2(currentNode.BottomLeftCorner.x, currentNode.BottomLeftCorner.y + dimensions.y), currentNode.TopRightCorner, currentNode, currentNode.TreeLayerIndex + 1, false);
            node2 = new RoomNode(currentNode.BottomLeftCorner, new Vector2(currentNode.TopRightCorner.x, currentNode.BottomLeftCorner.y + dimensions.y), currentNode, currentNode.TreeLayerIndex + 1, false);
        }
        listToReturn.Add(node2);
        int leftOrRight = Random.Range(0, 2);
        if (leftOrRight == 0)
        {
            indivisibleNode = new RoomNode(node2.BottomLeftCorner, new Vector2(node2.BottomLeftCorner.x + dimensions.x, node2.TopRightCorner.y), node2, node2.TreeLayerIndex + 1, isIndivisible);
            node2 = new RoomNode(new Vector2((node2.BottomLeftCorner.x + dimensions.x), node2.BottomLeftCorner.y), node2.TopRightCorner, node2, node2.TreeLayerIndex + 1, false);
        }
        else
        {
            indivisibleNode = new RoomNode(new Vector2(node2.TopRightCorner.x - dimensions.x, node2.BottomLeftCorner.y), node2.TopRightCorner, node2, node2.TreeLayerIndex + 1, isIndivisible);
            node2 = new RoomNode(node2.BottomLeftCorner, new Vector2(node2.TopRightCorner.x - dimensions.x, node2.TopRightCorner.y), node2, node2.TreeLayerIndex + 1, false);
        }
        if (node1.Width >= roomWidthMin && node1.Length >= roomLengthMin) AddNewNodesToCollections(listToReturn, graph, node1);
        if (node2.Width >= roomWidthMin && node2.Length >= roomLengthMin) AddNewNodesToCollections(listToReturn, graph, node2);
        AddNewNodesToCollections(listToReturn, graph, indivisibleNode);
        return indivisibleNode;
    }

    private void AddNewNodesToCollections(List<RoomNode> listToReturn, Queue<RoomNode> graph, RoomNode node)
    {
        listToReturn.Add(node);
        graph.Enqueue(node);
    }

    private Line GetLineDividingSpace(Vector2 bottomLeftCorner, Vector2 topRightCorner, int roomWidthMin, int roomLengthMin)
    {
        Orientation orientation;
        bool lengthStatus = (topRightCorner.y - bottomLeftCorner.y) >= 2 * roomLengthMin;
        bool widthStatus = (topRightCorner.x - bottomLeftCorner.x) >= 2 * roomWidthMin;
        if (lengthStatus && widthStatus)
        {
            orientation = (Orientation)(Random.Range(0, 2));
        }
        else if (widthStatus)
        {
            orientation = Orientation.Vertical;
        }
        else
        {
            orientation = Orientation.Horizontal;
        }
        return new Line(orientation, GetCoordinatesForOrientation(orientation, bottomLeftCorner, topRightCorner, roomWidthMin, roomLengthMin));
    }

    private Vector2 GetCoordinatesForOrientation(Orientation orientation, Vector2 bottomLeftCorner, Vector2 topRightCorner, int roomWidthMin, int roomLengthMin)
    {
        Vector2 coordinates = Vector2.zero;
        if (orientation == Orientation.Horizontal)
        {
            coordinates = new Vector2(0, Random.Range(bottomLeftCorner.y + roomLengthMin, topRightCorner.y - roomLengthMin));
        }
        else
        {
            coordinates = new Vector2(Random.Range(bottomLeftCorner.x + roomWidthMin, topRightCorner.x - roomWidthMin), 0);
        }
        return coordinates;
    }
}