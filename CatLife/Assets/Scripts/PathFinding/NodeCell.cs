using UnityEngine;

public class NodeCell
{
    public NodeCell parent;

    public bool isObstacle;

    public Vector3 pos;

    public Vector2Int index;

    public int gCost;

    public int hCost;

    public int FCost => gCost + hCost;

    public NodeCell(bool isObstacle, Vector3 pos, Vector2Int index)
    {
        this.isObstacle = isObstacle;
        this.pos = pos;
        this.index = index;
    }
}