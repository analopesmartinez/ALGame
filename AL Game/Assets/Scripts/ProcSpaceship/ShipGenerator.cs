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

    public List<Node> CalculateShip(int maxIterations, int roomWidthMin, int roomLengthMin, float roomBottomCornerModifier, float roomTopCornerModifier, int roomOffset, int corridorWidth, GameObject[] premadeRooms, int premadeRoomOffset, out List<RoomNode> premadeRoomSpaces)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(shipWidth, shipLength);
        allNodesCollection = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin, premadeRooms, premadeRoomOffset, out premadeRoomSpaces);
        List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeaves(bsp.RootNode);
        foreach (RoomNode premadeRoom in premadeRoomSpaces)
        {
            roomSpaces.Remove((Node)premadeRoom);
        }

        RoomGenerator roomGenerator = new RoomGenerator(maxIterations, roomWidthMin, roomLengthMin);
        List<RoomNode> roomList = roomGenerator.GenerateRoomsInGivenSpace(roomSpaces, roomBottomCornerModifier, roomTopCornerModifier, roomOffset, maxIterations);
        roomList = RemoveSmallRooms(roomList, corridorWidth, out List<RoomNode> roomsRemoved);
        foreach (RoomNode room in roomsRemoved)
        {
            allNodesCollection.Remove(room);
        }
        List<RoomNode> premadeRoomNodes = roomGenerator.GeneratePremadeRoomsInGivenSpace(premadeRoomSpaces, premadeRoomOffset);

        CorridorGenerator corridorGenerator = new CorridorGenerator();
        List<Node> corridorList = corridorGenerator.CreateCorridors(allNodesCollection, corridorWidth);

        return new List<Node>(roomList).Concat(corridorList).ToList().Concat(premadeRoomNodes).ToList();
    }

    private List<RoomNode> RemoveSmallRooms(List<RoomNode> rooms, float minimumSize, out List<RoomNode> roomsRemoved)
    {
        List<RoomNode> listToReturn = new List<RoomNode>();
        roomsRemoved = new List<RoomNode>();
        foreach (RoomNode room in rooms)
        {
            if (room.Width >= minimumSize && room.Length >= minimumSize)
            {
                listToReturn.Add(room);
            }
            else
            {
                roomsRemoved.Add(room);
            }
        }
        return listToReturn;
    }
}