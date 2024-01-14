using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class CorridorNode : Node
{
    private Node structure1;
    private Node structure2;
    private int corridorWidth;
    private int modifierDistanceFromWall = 1;

    public float Width { get { return (TopRightCorner.x - BottomLeftCorner.x); } }
    public float Length { get { return (TopRightCorner.y - BottomLeftCorner.y); } }

    public CorridorNode(Node node1, Node node2, int corridorWidth) : base(null, false)
    {
        this.structure1 = node1;
        this.structure2 = node2;
        this.corridorWidth = corridorWidth;
        GenerateCorridor();
    }

    private void GenerateCorridor()
    {
        RelativePosition relativePositionOfStructure2 = CheckStructure2Position();
        switch (relativePositionOfStructure2)
        {
            case RelativePosition.Up:
                ProcessRoomUpOrDown(this.structure1, this.structure2);
                break;
            case RelativePosition.Down:
                ProcessRoomUpOrDown(this.structure2, this.structure1);
                break;
            case RelativePosition.Left:
                ProcessRoomRightOrLeft(this.structure2, this.structure1);
                break;
            case RelativePosition.Right:
                ProcessRoomRightOrLeft(this.structure1, this.structure2);
                break;
            default:
                break;
        }
    }

    private void ProcessRoomRightOrLeft(Node structure1, Node structure2)
    {
        Node leftStructure = null;
        List<Node> leftStructureChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(structure1);
        Node rightStructure = null;
        List<Node> rightStructureChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(structure2);

        var sortedLeftStructures = leftStructureChildren.OrderByDescending(child => child.TopRightCorner.x).ToList();
        if (sortedLeftStructures.Count == 1)
        {
            leftStructure = sortedLeftStructures[0];
        }
        else
        {
            float maxX = sortedLeftStructures[0].TopRightCorner.x;
            sortedLeftStructures = sortedLeftStructures.Where(children => Math.Abs(maxX - children.TopRightCorner.x) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedLeftStructures.Count);
            leftStructure = sortedLeftStructures[index];
        }

        var possibleRightStructureNeighbours = rightStructureChildren.Where(
            child => GetValidYForNeighbourLeftRight(
                leftStructure.TopRightCorner,
                leftStructure.BottomRightCorner,
                child.TopLeftCorner,
                child.BottomLeftCorner
                ) != -1
            ).OrderBy(child => child.BottomRightCorner.x).ToList();
        if (possibleRightStructureNeighbours.Count <= 0)
        {
            rightStructure = structure2;
        }
        else
        {
            rightStructure = possibleRightStructureNeighbours[0];
        }
        float y = GetValidYForNeighbourLeftRight(leftStructure.TopRightCorner, leftStructure.BottomRightCorner, rightStructure.TopLeftCorner, rightStructure.BottomLeftCorner);
        while (y == -1 && sortedLeftStructures.Count > 1)
        {
            sortedLeftStructures = sortedLeftStructures.Where(child => child.TopLeftCorner.y != leftStructure.TopLeftCorner.y).ToList();
            leftStructure = sortedLeftStructures[0];
            y = GetValidYForNeighbourLeftRight(leftStructure.TopRightCorner, leftStructure.BottomRightCorner, rightStructure.TopLeftCorner, rightStructure.BottomLeftCorner);
        }
        BottomLeftCorner = new Vector2(leftStructure.BottomRightCorner.x, y);
        TopRightCorner = new Vector2(rightStructure.TopLeftCorner.x, y + this.corridorWidth);
    }

    private float GetValidYForNeighbourLeftRight(Vector2 leftNodeUp, Vector2 leftNodeDown, Vector2 rightNodeUp, Vector2 rightNodeDown)
    {
        if (rightNodeUp.y >= leftNodeUp.y && rightNodeDown.y <= leftNodeDown.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                leftNodeDown + new Vector2(0, modifierDistanceFromWall),
                leftNodeUp - new Vector2(0, modifierDistanceFromWall + this.corridorWidth)
                ).y;
        }
        if (rightNodeUp.y <= leftNodeUp.y && leftNodeDown.y <= rightNodeDown.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                rightNodeDown + new Vector2(0, modifierDistanceFromWall),
                rightNodeUp - new Vector2(0, modifierDistanceFromWall + this.corridorWidth)
                ).y;
        }
        if (leftNodeUp.y >= rightNodeDown.y && leftNodeUp.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                rightNodeDown + new Vector2(0, modifierDistanceFromWall),
                leftNodeUp - new Vector2(0, modifierDistanceFromWall + this.corridorWidth)
                ).y;
        }
        if (leftNodeDown.y >= rightNodeDown.y && leftNodeDown.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateMiddlePoint(
                leftNodeDown + new Vector2(0, modifierDistanceFromWall),
                rightNodeUp - new Vector2(0, modifierDistanceFromWall + this.corridorWidth)
                ).y;
        }
        return -1;
    }

    private void ProcessRoomUpOrDown(Node structure1, Node structure2)
    {
        Node bottomStructure = null;
        List<Node> structureBottmChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(structure1);
        Node topStructure = null;
        List<Node> structureAboveChildren = StructureHelper.TraverseGraphToExtractLowestLeaves(structure2);

        var sortedBottomStructure = structureBottmChildren.OrderByDescending(child => child.TopRightCorner.y).ToList();

        if (sortedBottomStructure.Count == 1)
        {
            bottomStructure = structureBottmChildren[0];
        }
        else
        {
            float maxY = sortedBottomStructure[0].TopLeftCorner.y;
            sortedBottomStructure = sortedBottomStructure.Where(child => Mathf.Abs(maxY - child.TopLeftCorner.y) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedBottomStructure.Count);
            bottomStructure = sortedBottomStructure[index];
        }

        var possibleNeighboursInTopStructure = structureAboveChildren.Where(
            child => GetValidXForNeighbourUpDown(
                bottomStructure.TopLeftCorner,
                bottomStructure.TopRightCorner,
                child.BottomLeftCorner,
                child.BottomRightCorner)
            != -1).OrderBy(child => child.BottomRightCorner.y).ToList();
        if (possibleNeighboursInTopStructure.Count == 0)
        {
            topStructure = structure2;
        }
        else
        {
            topStructure = possibleNeighboursInTopStructure[0];
        }
        float x = GetValidXForNeighbourUpDown(
                bottomStructure.TopLeftCorner,
                bottomStructure.TopRightCorner,
                topStructure.BottomLeftCorner,
                topStructure.BottomRightCorner);
        while (x == -1 && sortedBottomStructure.Count > 1)
        {
            sortedBottomStructure = sortedBottomStructure.Where(child => child.TopLeftCorner.x != topStructure.TopLeftCorner.x).ToList();
            bottomStructure = sortedBottomStructure[0];
            x = GetValidXForNeighbourUpDown(
                bottomStructure.TopLeftCorner,
                bottomStructure.TopRightCorner,
                topStructure.BottomLeftCorner,
                topStructure.BottomRightCorner);
        }
        BottomLeftCorner = new Vector2(x, bottomStructure.TopLeftCorner.y);
        TopRightCorner = new Vector2(x + this.corridorWidth, topStructure.BottomLeftCorner.y);
    }

    private float GetValidXForNeighbourUpDown(Vector2 bottomNodeLeft,
        Vector2 bottomNodeRight, Vector2 topNodeLeft, Vector2 topNodeRight)
    {
        if (topNodeLeft.x < bottomNodeLeft.x && bottomNodeRight.x < topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                bottomNodeLeft + new Vector2(modifierDistanceFromWall, 0),
                bottomNodeRight - new Vector2(this.corridorWidth + modifierDistanceFromWall, 0)
                ).x;
        }
        if (topNodeLeft.x >= bottomNodeLeft.x && bottomNodeRight.x >= topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                topNodeLeft + new Vector2(modifierDistanceFromWall, 0),
                topNodeRight - new Vector2(this.corridorWidth + modifierDistanceFromWall, 0)
                ).x;
        }
        if (bottomNodeLeft.x >= (topNodeLeft.x) && bottomNodeLeft.x <= topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                bottomNodeLeft + new Vector2(modifierDistanceFromWall, 0),
                topNodeRight - new Vector2(this.corridorWidth + modifierDistanceFromWall, 0)

                ).x;
        }
        if (bottomNodeRight.x <= topNodeRight.x && bottomNodeRight.x >= topNodeLeft.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                topNodeLeft + new Vector2(modifierDistanceFromWall, 0),
                bottomNodeRight - new Vector2(this.corridorWidth + modifierDistanceFromWall, 0)

                ).x;
        }
        return -1;
    }

    private RelativePosition CheckStructure2Position()
    {
        Vector2 structure1MiddlePoint = ((Vector2)structure1.TopRightCorner + structure1.BottomLeftCorner) / 2;
        Vector2 structure2MiddlePoint = ((Vector2)structure2.TopRightCorner + structure2.BottomLeftCorner) / 2;
        float angle = CalculateAngle(structure1MiddlePoint, structure2MiddlePoint);
        if ((angle < 45 && angle >= 0) || (angle >= -45 && angle < 0))
        {
            return RelativePosition.Right;
        }
        else if (angle >= 45 && angle < 135)
        {
            return RelativePosition.Up;
        }
        else if (angle >= -135 && angle < -45)
        {
            return RelativePosition.Down;
        }
        else
        {
            return RelativePosition.Left;
        }
    }

    private float CalculateAngle(Vector2 vector1, Vector2 vector2)
    {
        return Mathf.Atan2(vector2.y - vector1.y, vector2.x - vector1.x) * Mathf.Rad2Deg;
    }
}
