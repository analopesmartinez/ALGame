using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipGenerator
{
    List<RoomNode> allSpaceNodes = new List<RoomNode>();
    private int shipWidth;
    private int shipLength;

    public ShipGenerator(int shipWidth, int shipLength)
    {
        this.shipWidth = shipWidth;
        this.shipLength = shipLength;
    }

    public List<Node> CalculateRooms(int maxIterations, int roomWidthMin, int roomLengthMin)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(shipWidth, shipLength);
        allSpaceNodes = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);
        List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeaves(bsp.RootNode);
        RoomGenerator roomGenerator = new RoomGenerator(maxIterations, roomWidthMin, roomLengthMin);
        List<RoomNode> roomList = roomGenerator.GenerateRoomsInGivenSpace(roomSpaces);
        return new List<Node>(roomList);
    }
}