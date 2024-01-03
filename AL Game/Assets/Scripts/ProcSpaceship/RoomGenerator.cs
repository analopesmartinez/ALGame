using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator
{
    private int maxIterations;
    private int roomWidthMin;
    private int roomLengthMin;

    public RoomGenerator(int maxIterations, int roomWidthMin, int roomLengthMin)
    {
        this.maxIterations = maxIterations;
        this.roomWidthMin = roomWidthMin;
        this.roomLengthMin = roomLengthMin;
    }

    public List<RoomNode> GenerateRoomsInGivenSpace(List<Node> roomSpaces, float roomBottomCornerModifier, float roomTopCornerModifier, int roomOffset)
    {
        List<RoomNode> roomsToReturn = new List<RoomNode>();
        foreach (Node space in roomSpaces)
        {
            Vector2Int newBottomLeftCorner = StructureHelper.GenerateBottomLeftCornerBetween(space.BottomLeftCorner, space.TopRightCorner, roomBottomCornerModifier, roomOffset);
            Vector2Int newTopRightCorner = StructureHelper.GenerateTopRightCornerBetween(space.BottomLeftCorner, space.TopRightCorner, roomTopCornerModifier, roomOffset);
            space.BottomLeftCorner = newBottomLeftCorner;
            space.TopRightCorner = newTopRightCorner;
            space.BottomRightCorner = new Vector2Int(newTopRightCorner.x,newBottomLeftCorner.y);
            space.TopLeftCorner = new Vector2Int(newBottomLeftCorner.x, newTopRightCorner.y);
            roomsToReturn.Add((RoomNode)space);
        }
        return roomsToReturn;
    }
}