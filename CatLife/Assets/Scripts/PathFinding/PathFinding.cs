using System.Collections.Generic;
using BitFramework.Core;
using UnityEngine;

public class PathFinding
{
    private NodeCell[,] nodeCellArray;
    private int horizontalValue;
    private int verticalValue;

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
    }

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

    private List<NodeCell> GetNeighbours(NodeCell nodeCell)
    {
        List<NodeCell> neighbours = new List<NodeCell>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }

                int x = nodeCell.x + i;
                if (x < 0 || x >= horizontalValue)
                {
                    continue;
                }

                int y = nodeCell.y + j;
                if (y < 0 || y >= verticalValue)
                {
                    continue;
                }

                neighbours.Add(nodeCellArray[x, y]);
            }
        }

        return neighbours;
    }

    #region JPS

    public List<Vector3> FindPathByJps(Vector2Int startTileIndex, Vector2Int endTileIndex)
    {
        return FindPathByJps(nodeCellArray[startTileIndex.x, startTileIndex.y],
            nodeCellArray[endTileIndex.x, endTileIndex.y]);
    }

    public List<Vector3> FindPathByJps(NodeCell startCell, NodeCell endCell)
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

            IdentitySuccessors(curNode, startCell, endCell, openList, closeList);
        }

        return null;
    }

    private void IdentitySuccessors(NodeCell curNode, NodeCell startNode, NodeCell endNode, List<NodeCell> openList,
        List<NodeCell> closeList)
    {
        foreach (var neighbour in GetJPSNeighbours(curNode))
        {
            // x->n 的方向
            Vector2Int dir = new Vector2Int(neighbour.x - curNode.x / Mathf.Max(Mathf.Abs(neighbour.x - curNode.x), 1),
                neighbour.y - curNode.y / Mathf.Max(Mathf.Abs(neighbour.y - curNode.y), 1));
            var jumpNode = Jump(curNode, dir, startNode, endNode);
            if (jumpNode == null || jumpNode.isObstacle || closeList.Contains(jumpNode))
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
                    openList.Add(jumpNode);
                }
            }
        }
    }

    private NodeCell Jump(NodeCell current, Vector2Int dir, NodeCell startNode, NodeCell endNode)
    {
        if (!IsCanReachable(current.x + dir.x, current.y + dir.y))
        {
            return null;
        }

        var stepNode = nodeCellArray[current.x + dir.x, current.y + dir.y];

        if (stepNode == endNode)
        {
            return stepNode;
        }

        if (isExistForceNeighbours(stepNode))
        {
            return stepNode;
        }

        if (dir.x != 0 && dir.y != 0)
        {
            if (Jump(stepNode, new Vector2Int(dir.x, 0), startNode, endNode) != null)
            {
                return stepNode;
            }

            if (Jump(stepNode, new Vector2Int(0, dir.y), startNode, endNode) != null)
            {
                return stepNode;
            }
        }

        return Jump(stepNode, dir, startNode, endNode);
    }

    private bool isExistForceNeighbours(NodeCell curNode)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }

                int x = curNode.x + i;
                if (x < 0 || x >= horizontalValue)
                {
                    continue;
                }

                int y = curNode.y + j;
                if (y < 0 || y >= verticalValue)
                {
                    continue;
                }

                if (nodeCellArray[x, y].isForced)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private IEnumerable<NodeCell> GetJPSNeighbours(NodeCell nodeCell)
    {
        if (nodeCell.parent == null)
        {
            // 返回八个方向的邻居迭代器
            for (int i = 0; i < directionList.Count; i++)
            {
                int x = nodeCell.x + directionList[i].x;
                int y = nodeCell.y + directionList[i].y;

                if (IsCanReachable(x, y))
                {
                    yield return nodeCellArray[x, y];
                }
            }
        }
        else
        {
            var nodeParent = nodeCell.parent;

            var dNormX = (nodeCell.x - nodeParent.x) / Mathf.Max(Mathf.Abs(nodeCell.x - nodeParent.x), 1);
            var dNormY = (nodeCell.y - nodeParent.y) / Mathf.Max(Mathf.Abs(nodeCell.y - nodeParent.y), 1);

            // 斜向移动(剪枝)
            if (dNormX != 0 && dNormY != 0)
            {
                if (IsCanReachable(nodeCell.x, nodeCell.y + dNormY))
                {
                    yield return nodeCellArray[nodeCell.x, nodeCell.y + dNormY];
                }

                if (IsCanReachable(nodeCell.x + dNormX, nodeCell.y))
                {
                    yield return nodeCellArray[nodeCell.x + dNormX, nodeCell.y];
                }

                if (IsCanReachable(nodeCell.x, nodeCell.y + dNormY) ||
                    IsCanReachable(nodeCell.x + dNormX, nodeCell.y) &&
                    IsCanReachable(nodeCell.x + dNormX, nodeCell.y + dNormY))
                {
                    yield return nodeCellArray[nodeCell.x + dNormX, nodeCell.y + dNormY];
                }

                if (!IsCanReachable(nodeCell.x - dNormX, nodeCell.y) &&
                    IsCanReachable(nodeCell.x, nodeCell.y + dNormY) &&
                    IsCanReachable(nodeCell.x - dNormX, nodeCell.y + dNormY))
                {
                    nodeCellArray[nodeCell.x - dNormX, nodeCell.y + dNormY].isForced = true;
                    yield return nodeCellArray[nodeCell.x - dNormX, nodeCell.y + dNormY];
                }

                if (!IsCanReachable(nodeCell.x, nodeCell.y - dNormY) &&
                    IsCanReachable(nodeCell.x + dNormX, nodeCell.y) &&
                    IsCanReachable(nodeCell.x + dNormX, nodeCell.y - dNormY))
                {
                    nodeCellArray[nodeCell.x + dNormX, nodeCell.y - dNormY].isForced = true;
                    yield return nodeCellArray[nodeCell.x + dNormX, nodeCell.y - dNormY];
                }
            }
            // 直线移动
            else
            {
                // 垂直移动
                if (dNormX == 0)
                {
                    if (IsCanReachable(nodeCell.x, nodeCell.y + dNormY))
                    {
                        yield return nodeCellArray[nodeCell.x, nodeCell.y + dNormY];
                        if (!IsCanReachable(nodeCell.x + 1, nodeCell.y) &&
                            IsCanReachable(nodeCell.x + 1, nodeCell.y + dNormY))
                        {
                            nodeCellArray[nodeCell.x + 1, nodeCell.y + dNormY].isForced = true;
                            yield return nodeCellArray[nodeCell.x + 1, nodeCell.y + dNormY];
                        }

                        if (!IsCanReachable(nodeCell.x - 1, nodeCell.y) &&
                            IsCanReachable(nodeCell.x - 1, nodeCell.y + dNormY))
                        {
                            nodeCellArray[nodeCell.x - 1, nodeCell.y + dNormY].isForced = true;
                            yield return nodeCellArray[nodeCell.x - 1, nodeCell.y + dNormY];
                        }
                    }
                }
                else
                {
                    if (IsCanReachable(nodeCell.x + dNormX, nodeCell.y))
                    {
                        yield return nodeCellArray[nodeCell.x + dNormX, nodeCell.y];

                        if (!IsCanReachable(nodeCell.x, nodeCell.y + 1) &&
                            IsCanReachable(nodeCell.x + dNormX, nodeCell.y + 1))
                        {
                            nodeCellArray[nodeCell.x + dNormX, nodeCell.y + 1].isForced = true;
                            yield return nodeCellArray[nodeCell.x + dNormX, nodeCell.y + 1];
                        }

                        if (!IsCanReachable(nodeCell.x, nodeCell.y - 1) &&
                            IsCanReachable(nodeCell.x + dNormX, nodeCell.y - 1))
                        {
                            nodeCellArray[nodeCell.x + dNormX, nodeCell.y - 1].isForced = true;
                            yield return nodeCellArray[nodeCell.x + dNormX, nodeCell.y - 1];
                        }
                    }
                }
            }
        }
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

        return nodeCellArray[x, y].isObstacle;
    }

    #endregion


    private int GetManhattan(NodeCell curNode, NodeCell endCell)
    {
        return Mathf.Abs(endCell.x - curNode.x) * 10 + Mathf.Abs(endCell.y - curNode.y) * 10;
    }

    private List<Vector3> GeneratePath(NodeCell startNode, NodeCell endNode)
    {
        List<Vector3> path = new List<Vector3>();
        pathNodeList = new List<NodeCell>();
        if (endNode == null)
        {
            return path;
        }

        while (endNode != startNode)
        {
            path.Add(endNode.pos);
            pathNodeList.Add(endNode);
            endNode = endNode.parent;
        }

        path.Reverse();
        pathNodeList.Reverse();
        return path;
    }

    private List<NodeCell> pathNodeList;

    public List<NodeCell> GeneratePathCallback()
    {
        return pathNodeList;
    }
}