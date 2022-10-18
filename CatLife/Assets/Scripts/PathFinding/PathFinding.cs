using System.Collections.Generic;
using UnityEngine;

public class PathFinding
{
    private NodeCell[,] nodeCellArray;
    private int horizontalValue;
    private int verticalValue;
    private BinaryHeap<NodeCell> binaryHeap;

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

    public void Init(int horizontalValue, int verticalValue, NodeCell[,] nodeCellArray)
    {
        this.horizontalValue = horizontalValue;
        this.verticalValue = verticalValue;
        this.nodeCellArray = nodeCellArray;
        binaryHeap = new BinaryHeap<NodeCell>();
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
        if (curNode == null)
        {
            return null;
        }

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

                if (neighbourUp || neighbourRight)
                {
                    if (IsCanReachable(curNode.x + dx, curNode.y + dy))
                    {
                        neighbours.Add(nodeCellArray[curNode.x + dx, curNode.y + dy]);
                    }
                }

                if (!neighbourLeft && neighbourUp)
                {
                    if (IsCanReachable(curNode.x - dx, curNode.y + dy))
                    {
                        neighbours.Add(nodeCellArray[curNode.x - dx, curNode.y + dy]);
                    }
                }

                if (!neighbourDown && neighbourRight)
                {
                    if (IsCanReachable(curNode.x + dx, curNode.y - dy))
                    {
                        neighbours.Add(nodeCellArray[curNode.x + dx, curNode.y - dy]);
                    }
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


    private int GetManhattan(NodeCell curNode, NodeCell endCell)
    {
        return Mathf.Abs(endCell.x - curNode.x) * 10 + Mathf.Abs(endCell.y - curNode.y) * 10;
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
}