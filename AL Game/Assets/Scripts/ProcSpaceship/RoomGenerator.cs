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

    public List<RoomNode> GenerateRoomsInGivenSpace(List<Node> roomSpaces, float roomBottomCornerModifier, float roomTopCornerModifier, int roomOffset, int maxIterations)
    {
        List<RoomNode> roomsToReturn = new List<RoomNode>();
        foreach (Node space in roomSpaces)
        {
            /*bool roomFound = false;
            int iterations = 0;
            while (!roomFound && iterations < maxIterations)
            {
                iterations++;*/
                Vector2 newBottomLeftCorner = StructureHelper.GenerateBottomLeftCornerBetween(space.BottomLeftCorner, space.TopRightCorner, roomBottomCornerModifier, roomOffset);
                Vector2 newTopRightCorner = StructureHelper.GenerateTopRightCornerBetween(space.BottomLeftCorner, space.TopRightCorner, roomTopCornerModifier, roomOffset);
               // if (newTopRightCorner.x - newBottomLeftCorner.x >= roomWidthMin - 2 * roomOffset && newTopRightCorner.y - newBottomLeftCorner.y >= roomLengthMin - 2* roomOffset)
               // { 
                    space.BottomLeftCorner = newBottomLeftCorner;
                    space.TopRightCorner = newTopRightCorner;
                    space.BottomRightCorner = new Vector2(newTopRightCorner.x, newBottomLeftCorner.y);
                    space.TopLeftCorner = new Vector2(newBottomLeftCorner.x, newTopRightCorner.y);
                    roomsToReturn.Add((RoomNode)space);
                  /*  roomFound = true;
                }
            }*/
        }
        return roomsToReturn;
    }

    public List<RoomNode> GeneratePremadeRoomsInGivenSpace(List<RoomNode> premadeRoomSpaces, int premadeRoomOffset)
    {
        List<RoomNode> roomsToReturn = new List<RoomNode>();
        foreach (RoomNode space in premadeRoomSpaces)
        {
            Vector2 newBottomLeftCorner = new Vector2(space.BottomLeftCorner.x + ((float)premadeRoomOffset / 2.0f), space.BottomLeftCorner.y + ((float)premadeRoomOffset / 2.0f));
            Vector2 newTopRightCorner = new Vector2(space.TopRightCorner.x - ((float)premadeRoomOffset / 2.0f), space.TopRightCorner.y - ((float)premadeRoomOffset / 2.0f));
            space.BottomLeftCorner = newBottomLeftCorner;
            space.TopRightCorner = newTopRightCorner;
            space.BottomRightCorner = new Vector2(newTopRightCorner.x, newBottomLeftCorner.y);
            space.TopLeftCorner = new Vector2(newBottomLeftCorner.x, newTopRightCorner.y);
            roomsToReturn.Add((RoomNode)space);
        }
        return roomsToReturn;
    }
}