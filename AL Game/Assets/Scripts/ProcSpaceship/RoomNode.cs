using UnityEngine;

public class RoomNode : Node
{
    public RoomNode(Vector2 bottomLeftCorner, Vector2 topRightCorner, Node parentNode, int index, bool  isIndivisible) : base(parentNode, isIndivisible)
    {
        this.BottomLeftCorner = bottomLeftCorner;
        this.TopRightCorner = topRightCorner;
        this.BottomRightCorner = new Vector2(topRightCorner.x, bottomLeftCorner.y);
        this.TopLeftCorner = new Vector2(bottomLeftCorner.x, topRightCorner.y);
    }

    public float Width { get { return (TopRightCorner.x - BottomLeftCorner.x); } }
    public float Length { get { return (TopRightCorner.y - BottomLeftCorner.y); } }

}