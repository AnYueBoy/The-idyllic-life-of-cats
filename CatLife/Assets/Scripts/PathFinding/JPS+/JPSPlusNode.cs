using UnityEngine;

public class JPSPlusNode
{
    public JPSPlusNode parent;

    public bool isObstacle;

    public Vector3 pos;

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