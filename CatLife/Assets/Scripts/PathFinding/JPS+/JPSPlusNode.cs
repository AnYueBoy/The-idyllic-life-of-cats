using UnityEngine;

public class JPSPlusNode
{
    public JPSPlusNode parent;

    public bool isObstacle;

    public Vector3 pos;

    public int x;
    public int y;
    public Vector3Int mappingTileIndex;

    public int gCost;
    public int hCost;

    /// <summary>
    /// 八方向的距离值
    /// </summary>
    public int[] distances = new int[8];

    /// <summary>
    /// 是否是跳点
    /// </summary>
    public bool isJumpPoint;

    /// <summary>
    /// 跳点方向
    /// </summary>
    public bool[] jumpPointDirection = new bool[8];

    /// <summary>
    /// 从父节点扩展到此的方向
    /// </summary>
    public Directions directionFromParent;

    public JPSPlusNode(bool isObstacle, Vector3 pos, int x, int y, Vector3Int mappingTileIndex)
    {
        this.isObstacle = isObstacle;
        this.pos = pos;
        this.x = x;
        this.y = y;
        this.mappingTileIndex = mappingTileIndex;
    }

    public bool isJumpPointComingFrom(Directions dir)
    {
        // 是否是此方向的跳点
        return isJumpPoint && jumpPointDirection[(int)dir];
    }
}

public enum Directions
{
    UP,
    RIGHT_UP,
    RIGHT,
    RIGHT_DOWN,
    DOWN,
    LEFT_DOWN,
    LEFT,
    LEFT_UP
}