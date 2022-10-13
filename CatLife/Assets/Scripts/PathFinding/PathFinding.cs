using System.Collections.Generic;
using UnityEngine;

public class PathFinding
{
    private NodeCell[,] nodeCellArray;
    private int horizontalValue;
    private int verticalValue;

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

    private int GetManhattan(NodeCell curNode, NodeCell endCell)
    {
        return Mathf.Abs(endCell.x - curNode.x) * 10 + Mathf.Abs(endCell.y - curNode.y) * 10;
    }

    private List<Vector3> GeneratePath(NodeCell startNode, NodeCell endNode)
    {
        List<Vector3> path = new List<Vector3>();
        if (endNode == null)
        {
            return path;
        }

        while (endNode != startNode)
        {
            path.Add(endNode.pos);
            endNode = endNode.parent;
        }

        path.Reverse();
        return path;
    }
}