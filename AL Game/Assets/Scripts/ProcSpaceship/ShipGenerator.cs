using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShipGenerator
{
    List<RoomNode> allNodesCollection = new List<RoomNode>();
    private int shipWidth;
    private int shipLength;

    public ShipGenerator(int shipWidth, int shipLength)
    {
        this.shipWidth = shipWidth;
        this.shipLength = shipLength;
    }

    public List<Node> CalculateShip(int maxIterations, int roomWidthMin, int roomLengthMin, float roomBottomCornerModifier, float roomTopCornerModifier, int roomOffset, int corridorWidth)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(shipWidth, shipLength);
        allNodesCollection = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);
        List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeaves(bsp.RootNode);

        RoomGenerator roomGenerator = new RoomGenerator(maxIterations, roomWidthMin, roomLengthMin);
        List<RoomNode> roomList = roomGenerator.GenerateRoomsInGivenSpace(roomSpaces, roomBottomCornerModifier, roomTopCornerModifier, roomOffset);

        CorridorGenerator corridorGenerator = new CorridorGenerator();
        var corridorList = corridorGenerator.CreateCorridors(allNodesCollection, corridorWidth);

        return new List<Node>(roomList).Concat(corridorList).ToList();
    }
}