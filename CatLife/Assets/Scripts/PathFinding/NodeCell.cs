using System;
using UnityEngine;

public class NodeCell : IComparable<NodeCell>
{
    public NodeCell parent;

    public bool isObstacle;

    public Vector3 pos;

    public int gCost;

    public int hCost;

    public int x;
    public int y;

    public int FCost => gCost + hCost;

    public Vector3Int mappingTileIndex;

    public NodeCell(bool isObstacle, Vector3 pos, int x, int y, Vector3Int mappingTileIndex)
    {
        this.isObstacle = isObstacle;
        this.pos = pos;
        this.x = x;
        this.y = y;
        this.mappingTileIndex = mappingTileIndex;
    }

    public void Reset()
    {
        gCost = hCost = 0;
        parent = null;
    }

    public int CompareTo(NodeCell other)
    {
        return FCost - other.FCost;
    }
}