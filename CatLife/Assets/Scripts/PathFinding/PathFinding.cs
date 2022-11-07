using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding
{
    private NodeCell[,] nodeCellArray;
    private JPSPlusNode[,] jpsPlusMapNodeArray;
    private int horizontalValue;
    private int verticalValue;
    private BinaryHeap<NodeCell> binaryHeap;
    private BinaryHeap<JPSPlusNode> jpsPlusHeap;

    private readonly List<Vector2Int> directionList = new List<Vector2Int>()
    {
        Vector2Int.left,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.up,

        // 左下
        new Vector2Int(-1, -1),
        // 左上
        new Vector2Int(-1, 1),
        // 右下
        new Vector2Int(1, -1),
        // 右上
        new Vector2Int(1, 1),
    };

    public void Init(int horizontalValue, int verticalValue, NodeCell[,] nodeCellArray,
        JPSPlusNode[,] jpsPlusMapNodeArray)
    {
        this.horizontalValue = horizontalValue;
        this.verticalValue = verticalValue;
        this.nodeCellArray = nodeCellArray;
        this.jpsPlusMapNodeArray = jpsPlusMapNodeArray;
        binaryHeap = new BinaryHeap<NodeCell>();
        jpsPlusHeap = new BinaryHeap<JPSPlusNode>();
    }

    #region A*寻路

    public List<Vector3> FindPath(Vector2Int startTileIndex, Vector2Int endTileIndex)
    {
        return FindPath(nodeCellArray[startTileIndex.x, startTileIndex.y],
            nodeCellArray[endTileIndex.x, endTileIndex.y]);
    }

    public List<Vector3> FindPath(NodeCell startCell, NodeCell endCell)
    {
        List<NodeCell> openList = new List<NodeCell>();
        List<NodeCell> closeList = new List<NodeCell>();

        openList.Add(startCell);

        while (openList.Count > 0)
        {
            NodeCell curNode = openList[0];
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].FCost < curNode.FCost ||
                    (openList[i].FCost == curNode.FCost && openList[i].hCost < curNode.hCost))
                {
                    curNode = openList[i];
                }
            }

            openList.Remove(curNode);
            closeList.Add(curNode);

            if (curNode == endCell)
            {
                // 生成路径
                return GeneratePath(startCell, endCell);
            }

            foreach (var nodeCell in GetNeighbours(curNode))
            {
                if (nodeCell.isObstacle || closeList.Contains(nodeCell))
                {
                    continue;
                }

                int gCost = curNode.gCost + GetManhattan(curNode, nodeCell);

                if (gCost < nodeCell.gCost || !openList.Contains(nodeCell))
                {
                    nodeCell.gCost = gCost;
                    nodeCell.hCost = GetManhattan(nodeCell, endCell);
                    nodeCell.parent = curNode;

                    if (!openList.Contains(nodeCell))
                    {
                        openList.Add(nodeCell);
                    }
                }
            }
        }

        return null;
    }

    private List<NodeCell> GetNeighbours(NodeCell curNode)
    {
        List<NodeCell> neighbours = new List<NodeCell>();
        for (int i = 0; i < directionList.Count; i++)
        {
            int x = curNode.x + directionList[i].x;
            int y = curNode.y + directionList[i].y;

            if (IsCanReachable(x, y))
            {
                neighbours.Add(nodeCellArray[x, y]);
            }
        }

        return neighbours;
    }

    #endregion

    #region JPS

    public List<Vector3> FindPathByJps(Vector2Int startTileIndex, Vector2Int endTileIndex)
    {
        return FindPathByJps(nodeCellArray[startTileIndex.x, startTileIndex.y],
            nodeCellArray[endTileIndex.x, endTileIndex.y]);
    }

    public List<Vector3> FindPathByJps(NodeCell startCell, NodeCell endCell)
    {
        List<NodeCell> closeList = new List<NodeCell>();
        binaryHeap.Clear();
        startCell.Reset();
        binaryHeap.Push(startCell);

        while (binaryHeap.Count > 0)
        {
            var curNode = binaryHeap.Pop();
            closeList.Add(curNode);

            if (curNode == endCell)
            {
                // 生成路径
                return GeneratePath(startCell, endCell);
            }

            IdentitySuccessors(curNode, endCell, binaryHeap, closeList);
        }

        return null;
    }

    private void IdentitySuccessors(NodeCell curNode, NodeCell endNode, BinaryHeap<NodeCell> openList,
        List<NodeCell> closeList)
    {
        foreach (var neighbour in GetJPSNeighbours(curNode))
        {
            var jumpNode = Jump(neighbour, curNode, endNode);
            if (jumpNode != null)
            {
                if (closeList.Contains(jumpNode))
                {
                    continue;
                }

                int gCost = curNode.gCost + GetManhattan(curNode, jumpNode);

                if (gCost < jumpNode.gCost || !openList.Contains(jumpNode))
                {
                    jumpNode.gCost = gCost;
                    jumpNode.hCost = GetManhattan(jumpNode, endNode);
                    jumpNode.parent = curNode;

                    if (!openList.Contains(jumpNode))
                    {
                        openList.Push(jumpNode);
                    }
                    else
                    {
                        openList.UpdateHead(jumpNode);
                    }
                }
            }
        }
    }

    private NodeCell Jump(NodeCell curNode, NodeCell parentNode, NodeCell endNode)
    {
        int x = curNode.x;
        int y = curNode.y;
        int dx = curNode.x - parentNode.x;
        int dy = curNode.y - parentNode.y;
        if (!IsCanReachable(x, y))
        {
            return null;
        }

        if (curNode == endNode)
        {
            return curNode;
        }

        // 斜向
        if (dx != 0 && dy != 0)
        {
            // 寻找强迫邻居，进而判断该节点是否为跳点
            if ((IsCanReachable(x - dx, y + dy) && !IsCanReachable(x - dx, y)) ||
                (IsCanReachable(x + dx, y - dy) && !IsCanReachable(x, y - dy)))
                return curNode;

            if (Jump(nodeCellArray[x + dx, y], curNode, endNode) != null ||
                Jump(nodeCellArray[x, y + dy], curNode, endNode) != null)
                return curNode;
        }
        // 直线
        else
        {
            if (dx != 0)
            {
                // 水平 
                if ((IsCanReachable(x + dx, y + 1) && !IsCanReachable(x, y + 1)) ||
                    (IsCanReachable(x + dx, y - 1) && !IsCanReachable(x, y - 1)))
                    return curNode;
            }
            else
            {
                // 垂直
                if ((IsCanReachable(x + 1, y + dy) && !IsCanReachable(x + 1, y)) ||
                    (IsCanReachable(x - 1, y + dy) && !IsCanReachable(x - 1, y)))
                    return curNode;
            }
        }

        if (IsCanReachable(x + dx, y) || IsCanReachable(x, y + dy))
            return Jump(nodeCellArray[x + dx, y + dy], curNode, endNode);
        return null;
    }

    private List<NodeCell> GetJPSNeighbours(NodeCell curNode)
    {
        List<NodeCell> neighbours = new List<NodeCell>();
        if (curNode.parent == null)
        {
            // 返回八个方向的邻居
            for (int i = 0; i < directionList.Count; i++)
            {
                int x = curNode.x + directionList[i].x;
                int y = curNode.y + directionList[i].y;

                if (IsCanReachable(x, y))
                {
                    neighbours.Add(nodeCellArray[x, y]);
                }
            }
        }
        else
        {
            var nodeParent = curNode.parent;

            var dx = (curNode.x - nodeParent.x) / Mathf.Max(Mathf.Abs(curNode.x - nodeParent.x), 1);
            var dy = (curNode.y - nodeParent.y) / Mathf.Max(Mathf.Abs(curNode.y - nodeParent.y), 1);

            // 斜向移动(剪枝)
            if (dx != 0 && dy != 0)
            {
                bool neighbourUp = IsCanReachable(curNode.x, curNode.y + dy);
                bool neighbourRight = IsCanReachable(curNode.x + dx, curNode.y);
                bool neighbourLeft = IsCanReachable(curNode.x - dx, curNode.y);
                bool neighbourDown = IsCanReachable(curNode.x, curNode.y - dy);

                if (neighbourUp)
                {
                    neighbours.Add(nodeCellArray[curNode.x, curNode.y + dy]);
                }

                if (neighbourRight)
                {
                    neighbours.Add(nodeCellArray[curNode.x + dx, curNode.y]);
                }

                if ((neighbourUp || neighbourRight) && IsCanReachable(curNode.x + dx, curNode.y + dy))
                {
                    neighbours.Add(nodeCellArray[curNode.x + dx, curNode.y + dy]);
                }

                if (!neighbourLeft && neighbourUp && IsCanReachable(curNode.x - dx, curNode.y + dy))
                {
                    neighbours.Add(nodeCellArray[curNode.x - dx, curNode.y + dy]);
                }

                if (!neighbourDown && neighbourRight && IsCanReachable(curNode.x + dx, curNode.y - dy))
                {
                    neighbours.Add(nodeCellArray[curNode.x + dx, curNode.y - dy]);
                }
            }
            // 直线移动
            else
            {
                // 垂直移动
                if (dx == 0)
                {
                    if (IsCanReachable(curNode.x, curNode.y + dy))
                    {
                        neighbours.Add(nodeCellArray[curNode.x, curNode.y + dy]);

                        if (!IsCanReachable(curNode.x + 1, curNode.y) && IsCanReachable(curNode.x + 1, curNode.y + dy))
                        {
                            neighbours.Add(nodeCellArray[curNode.x + 1, curNode.y + dy]);
                        }

                        if (!IsCanReachable(curNode.x - 1, curNode.y) && IsCanReachable(curNode.x - 1, curNode.y + dy))
                        {
                            neighbours.Add(nodeCellArray[curNode.x - 1, curNode.y + dy]);
                        }
                    }
                }
                else
                {
                    if (IsCanReachable(curNode.x + dx, curNode.y))
                    {
                        neighbours.Add(nodeCellArray[curNode.x + dx, curNode.y]);

                        if (!IsCanReachable(curNode.x, curNode.y + 1) && IsCanReachable(curNode.x + dx, curNode.y + 1))
                        {
                            neighbours.Add(nodeCellArray[curNode.x + dx, curNode.y + 1]);
                        }

                        if (!IsCanReachable(curNode.x, curNode.y - 1) && IsCanReachable(curNode.x + dx, curNode.y - 1))
                        {
                            neighbours.Add(nodeCellArray[curNode.x + dx, curNode.y - 1]);
                        }
                    }
                }
            }
        }

        return neighbours;
    }

    private bool IsCanReachable(int x, int y)
    {
        if (x < 0 || x >= horizontalValue)
        {
            return false;
        }

        if (y < 0 || y >= verticalValue)
        {
            return false;
        }

        return !nodeCellArray[x, y].isObstacle;
    }

    #endregion

    #region JPS+

    private Dictionary<Directions, Directions[]> validDirLookUpTable = new Dictionary<Directions, Directions[]>
    {
        {
            Directions.DOWN,
            new[] { Directions.LEFT, Directions.LEFT_DOWN, Directions.DOWN, Directions.RIGHT_DOWN, Directions.RIGHT }
        },
        { Directions.RIGHT_DOWN, new[] { Directions.DOWN, Directions.RIGHT_DOWN, Directions.RIGHT } },
        {
            Directions.RIGHT,
            new[] { Directions.DOWN, Directions.RIGHT_DOWN, Directions.RIGHT, Directions.RIGHT_UP, Directions.UP }
        },
        { Directions.RIGHT_UP, new[] { Directions.RIGHT, Directions.RIGHT_UP, Directions.UP } },
        {
            Directions.UP,
            new[] { Directions.RIGHT, Directions.RIGHT_UP, Directions.UP, Directions.LEFT_UP, Directions.LEFT }
        },
        { Directions.LEFT_UP, new[] { Directions.UP, Directions.LEFT_UP, Directions.LEFT } },
        {
            Directions.LEFT,
            new[] { Directions.UP, Directions.LEFT_UP, Directions.LEFT, Directions.LEFT_DOWN, Directions.DOWN }
        },
        { Directions.LEFT_DOWN, new[] { Directions.LEFT, Directions.LEFT_DOWN, Directions.DOWN } }
    };

    private Directions[] allDirections = Enum.GetValues(typeof(Directions)).Cast<Directions>().ToArray();

    public List<Vector3> FindPathByJpsPlus(Vector2Int startTileIndex, Vector2Int endTileIndex)
    {
        return FindPathByJpsPlus(jpsPlusMapNodeArray[startTileIndex.x, startTileIndex.y],
            jpsPlusMapNodeArray[endTileIndex.x, endTileIndex.y]);
    }

    public List<Vector3> FindPathByJpsPlus(JPSPlusNode startNode, JPSPlusNode endNode)
    {
        List<JPSPlusNode> closeList = new List<JPSPlusNode>();
        jpsPlusHeap.Clear();
        startNode.Reset();
        jpsPlusHeap.Push(startNode);

        while (jpsPlusHeap.Count > 0)
        {
            var curNode = jpsPlusHeap.Pop();
            closeList.Add(curNode);

            if (curNode == endNode)
            {
                // 生成路径
                return GeneratePath(startNode, endNode);
            }

            foreach (var dir in GetAllValidDirections(curNode))
            {
                JPSPlusNode newSuccessor = null;
                int gCost = 0;

                // 检查当前的方向dir,是否与curNode 到endNode方向一致，且到目标点的值小于计算值，则此节点此方向的后继节点可认为为目标点
                if (IsCardinal(dir) && GoalIsInExactDirection(curNode, dir, endNode) &&
                    JPSPlusNode.Diff(curNode, endNode) <= Mathf.Abs(curNode.distances[(int)dir]))
                {
                    newSuccessor = endNode;
                    gCost = curNode.gCost + JPSPlusNode.Diff(curNode, endNode);
                }

                // 如上一样的思想，只是此条件为对角线方向的检查逻辑
                else if (IsDiagonal(dir) && GoalIsInGeneralDirection(curNode, dir, endNode) &&
                         (Mathf.Abs(endNode.y - curNode.y) <= Mathf.Abs(curNode.distances[(int)dir]) ||
                          Mathf.Abs(endNode.x - curNode.x) <= Mathf.Abs(curNode.distances[(int)dir])))
                {
                    int minDiff = Mathf.Min(Mathf.Abs(endNode.y - curNode.y), Mathf.Abs(endNode.x - curNode.x));
                    newSuccessor = GetJPSNodeByDis(curNode, dir, minDiff);
                    gCost = curNode.gCost + GetManhattan(curNode, newSuccessor);
                }
                else if (curNode.distances[(int)dir] > 0)
                {
                    newSuccessor = GetJPSNodeByDis(curNode, dir, curNode.distances[(int)dir]);
                    gCost = curNode.gCost + GetManhattan(curNode, newSuccessor);
                }

                // A星寻路逻辑
                if (newSuccessor != null && !closeList.Contains(newSuccessor))
                {
                    if (!jpsPlusHeap.Contains(newSuccessor))
                    {
                        newSuccessor.parent = curNode;
                        newSuccessor.gCost = gCost;
                        newSuccessor.directionFromParent = dir;
                        newSuccessor.fCost = gCost + GetManhattan(newSuccessor, endNode);
                        jpsPlusHeap.Push(newSuccessor);
                    }
                    else if (gCost < newSuccessor.gCost)
                    {
                        newSuccessor.parent = curNode;
                        newSuccessor.gCost = gCost;
                        newSuccessor.directionFromParent = dir;
                        newSuccessor.fCost = gCost + GetManhattan(newSuccessor, endNode);
                        jpsPlusHeap.UpdateHead(newSuccessor);
                    }
                }
            }
        }

        return null;
    }

    private JPSPlusNode GetJPSNodeByDis(JPSPlusNode curNode, Directions dir, int distance)
    {
        int x = curNode.x;
        int y = curNode.y;
        switch (dir)
        {
            case Directions.UP:
                y += distance;
                break;

            case Directions.DOWN:
                y -= distance;
                break;

            case Directions.LEFT:
                x -= distance;
                break;

            case Directions.RIGHT:
                x += distance;
                break;

            case Directions.LEFT_UP:
                x -= distance;
                y += distance;
                break;

            case Directions.RIGHT_UP:
                x += distance;
                y += distance;
                break;

            case Directions.LEFT_DOWN:
                x -= distance;
                y -= distance;
                break;

            case Directions.RIGHT_DOWN:
                x += distance;
                y -= distance;
                break;
        }

        if (IsInBound(x, y))
        {
            return jpsPlusMapNodeArray[x, y];
        }

        return null;
    }

    private bool IsInBound(int x, int y)
    {
        if (x < 0 || y < 0 || x >= horizontalValue || y >= verticalValue)
        {
            return false;
        }

        return true;
    }

    private bool IsCardinal(Directions dir)
    {
        switch (dir)
        {
            case Directions.UP:
            case Directions.DOWN:
            case Directions.LEFT:
            case Directions.RIGHT:
                return true;
        }

        return false;
    }

    private bool IsDiagonal(Directions dir)
    {
        switch (dir)
        {
            case Directions.RIGHT_DOWN:
            case Directions.LEFT_DOWN:
            case Directions.LEFT_UP:
            case Directions.RIGHT_UP:
                return true;
        }

        return false;
    }

    /// <summary>
    /// 目标与当前节点处于方正方向
    /// </summary>
    private bool GoalIsInExactDirection(JPSPlusNode curNode, Directions dir, JPSPlusNode goalNode)
    {
        int diffX = goalNode.x - curNode.x;
        int diffY = goalNode.y - curNode.y;
        switch (dir)
        {
            case Directions.UP:
                return diffX == 0 && diffY > 0;

            case Directions.DOWN:
                return diffX == 0 && diffY < 0;

            case Directions.LEFT:
                return diffX < 0 && diffY == 0;

            case Directions.RIGHT:
                return diffX > 0 && diffY == 0;

            case Directions.LEFT_UP:
                return diffX < 0 && diffY > 0 && Mathf.Abs(diffX) == Mathf.Abs(diffY);

            case Directions.RIGHT_UP:
                return diffX > 0 && diffY > 0 && Mathf.Abs(diffX) == Mathf.Abs(diffY);

            case Directions.LEFT_DOWN:
                return diffX < 0 && diffY < 0 && Mathf.Abs(diffX) == Mathf.Abs(diffY);

            case Directions.RIGHT_DOWN:
                return diffX > 0 && diffY < 0 && Mathf.Abs(diffX) == Mathf.Abs(diffY);
        }

        return false;
    }

    /// <summary>
    /// 目标节点与当前节点通用方向
    /// </summary>
    private bool GoalIsInGeneralDirection(JPSPlusNode curNode, Directions dir, JPSPlusNode goalNode)
    {
        int diffX = goalNode.x - curNode.x;
        int diffY = goalNode.y - curNode.y;
        switch (dir)
        {
            case Directions.UP:
                return diffX == 0 && diffY > 0;

            case Directions.DOWN:
                return diffX == 0 && diffY < 0;

            case Directions.LEFT:
                return diffX < 0 && diffY == 0;

            case Directions.RIGHT:
                return diffX > 0 && diffY == 0;

            case Directions.LEFT_UP:
                return diffX < 0 && diffY > 0;

            case Directions.RIGHT_UP:
                return diffX > 0 && diffY > 0;

            case Directions.LEFT_DOWN:
                return diffX < 0 && diffY < 0;

            case Directions.RIGHT_DOWN:
                return diffX > 0 && diffY < 0;
        }

        return false;
    }

    private Directions[] GetAllValidDirections(JPSPlusNode curNode)
    {
        return curNode.parent == null ? allDirections : validDirLookUpTable[curNode.directionFromParent];
    }

    #endregion

    private int GetManhattan(NodeCell curNode, NodeCell endCell)
    {
        return Mathf.Abs(endCell.x - curNode.x) * 10 + Mathf.Abs(endCell.y - curNode.y) * 10;
    }

    private int GetManhattan(JPSPlusNode curNode, JPSPlusNode endNode)
    {
        return Mathf.Abs(endNode.x - curNode.x) * 10 + Mathf.Abs(endNode.y - curNode.y) * 10;
    }

    private List<Vector3> GeneratePath(NodeCell startNode, NodeCell endNode)
    {
        List<Vector3> path = new List<Vector3>();
        while (endNode != startNode)
        {
            path.Add(endNode.pos);
            var parent = endNode.parent;
            endNode.Reset();
            endNode = parent;
        }

        path.Reverse();
        return path;
    }

    private List<Vector3> GeneratePath(JPSPlusNode startNode, JPSPlusNode endNode)
    {
        List<Vector3> path = new List<Vector3>();
        while (endNode != startNode)
        {
            path.Add(endNode.pos);
            var parent = endNode.parent;
            endNode.Reset();
            endNode = parent;
        }

        path.Reverse();
        return path;
    }
}