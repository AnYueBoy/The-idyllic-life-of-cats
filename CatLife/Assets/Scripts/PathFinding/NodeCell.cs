using UnityEngine;

public class NodeCell
{
    public NodeCell parent;

    public bool isObstacle;

    public Vector3 pos;

    public int gCost;

    public int hCost;

    public int x;
    public int y;

    public int FCost => gCost + hCost;

    public NodeCell(bool isObstacle, Vector3 pos, int x, int y)
    {
        this.isObstacle = isObstacle;
        this.pos = pos;
        this.x = x;
        this.y = y;
    }
}