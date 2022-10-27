using System;
using System.Collections.Generic;
using System.Linq;
using BitFramework.Component.AssetsModule;
using BitFramework.Component.ObjectPoolModule;
using BitFramework.Core;
using UnityEngine;

public class MapManager : MonoBehaviour, IManager
{
    [SerializeField] private bool isOpenDebug;
    [SerializeField] private bool useJPS;
    [SerializeField] private bool useJPSPlus;

    public void Init()
    {
        pathFinding = new PathFinding();
        InitMap();
    }

    public void LocalUpdate(float dt)
    {
    }

    private void InitMap()
    {
        curMap = App.Make<SpawnManager>().SpawnMap();
        ScanMapInfo();
    }

    private Map curMap;

    /// <summary>
    /// 决定了垂直格子的个数即y的最大值
    /// </summary>
    private int row;

    /// <summary>
    /// 决定了水平格子的个数即 x的最大值
    /// </summary>
    private int column;

    private Vector3Int leftBottomIndex;

    private PathFinding pathFinding;

    private void ScanMapInfo()
    {
        leftBottomIndex = curMap.GroundTileMap.origin;
        var size = curMap.GroundTileMap.size;
        column = size.x;
        row = size.y;

        // 平面直角坐标系 x轴向右 y轴向上
        var mapNodeCellArray = new NodeCell[column, row];
        if (useJPSPlus)
        {
            jpsMapNodeArray = new JPSPlusNode[column, row];
        }

        for (int x = leftBottomIndex.x; x < leftBottomIndex.x + column; x++)
        {
            for (int y = leftBottomIndex.y; y < leftBottomIndex.y + row; y++)
            {
                bool isObstacle = IsObstacle(x, y);
                Vector3 pos = GetPosByTileIndex(x, y);

                Vector2Int nodeCellIndex = ConvertTileIndexToCellIndex(x, y);

                // 构建非JPS+的节点信息
                mapNodeCellArray[nodeCellIndex.x, nodeCellIndex.y] =
                    new NodeCell(isObstacle, pos, nodeCellIndex.x, nodeCellIndex.y, new Vector3Int(x, y, 0));

                // 构建JPS+的节点信息
                if (useJPSPlus)
                {
                    jpsMapNodeArray[nodeCellIndex.x, nodeCellIndex.y] = new JPSPlusNode(isObstacle, pos,
                        nodeCellIndex.x,
                        nodeCellIndex.y, new Vector3Int(x, y, 0));
                }
            }
        }

        if (useJPSPlus)
        {
            BuildPrimaryJumpPoints();
            BuildStraightJumpPoint();
            BuildDiagonalJumpPoint();
        }

        pathFinding.Init(column, row, mapNodeCellArray, jpsMapNodeArray);
    }

    #region JPS+ Preprocess Map

    private JPSPlusNode[,] jpsMapNodeArray;

    private void BuildPrimaryJumpPoints()
    {
        // 构建主要跳点
        for (int y = 0; y < row; y++)
        {
            for (int x = 0; x < column; x++)
            {
                JPSPlusNode currNode = jpsMapNodeArray[x, y];
                if (!currNode.isObstacle)
                {
                    continue;
                }

                // 主要跳点为论文中的八种情况，但可两两合并。

                // 相对障碍的右上角情况
                if (isEmpty(x + 1, y + 1) && isEmpty(x, y + 1) && isEmpty(x + 1, y))
                {
                    JPSPlusNode node = jpsMapNodeArray[x + 1, y + 1];
                    node.isJumpPoint = true;
                    node.jumpPointDirection[(int)Directions.DOWN] = true;
                    node.jumpPointDirection[(int)Directions.LEFT] = true;
                }

                // 相对障碍的右下角情况
                if (isEmpty(x + 1, y - 1) && isEmpty(x + 1, y) && isEmpty(x, y - 1))
                {
                    JPSPlusNode node = jpsMapNodeArray[x + 1, y - 1];
                    node.isJumpPoint = true;
                    node.jumpPointDirection[(int)Directions.LEFT] = true;
                    node.jumpPointDirection[(int)Directions.UP] = true;
                }

                // 相对障碍的左下角情况 
                if (isEmpty(x - 1, y - 1) && isEmpty(x - 1, y) && isEmpty(x, y - 1))
                {
                    JPSPlusNode node = jpsMapNodeArray[x - 1, y - 1];
                    node.isJumpPoint = true;
                    node.jumpPointDirection[(int)Directions.RIGHT] = true;
                    node.jumpPointDirection[(int)Directions.UP] = true;
                }

                // 相对障碍的左上角情况 
                if (isEmpty(x - 1, y + 1) && isEmpty(x - 1, y) && isEmpty(x, y + 1))
                {
                    JPSPlusNode node = jpsMapNodeArray[x - 1, y + 1];
                    node.isJumpPoint = true;
                    node.jumpPointDirection[(int)Directions.RIGHT] = true;
                    node.jumpPointDirection[(int)Directions.DOWN] = true;
                }
            }
        }
    }

    private void BuildStraightJumpPoint()
    {
        // 构建直线跳点
        for (int y = 0; y < row; y++)
        {
            int jumpDistanceSoFar = -1;
            bool jumpPointSeen = false;
            // 从左到右扫描,填充节点Left距离值
            for (int x = 0; x < column; x++)
            {
                JPSPlusNode node = jpsMapNodeArray[x, y];
                if (node.isObstacle)
                {
                    jumpDistanceSoFar = -1;
                    jumpPointSeen = false;
                    node.distances[(int)Directions.LEFT] = 0;
                    continue;
                }

                jumpDistanceSoFar++;

                if (jumpPointSeen)
                {
                    node.distances[(int)Directions.LEFT] = jumpDistanceSoFar;
                }
                else
                {
                    node.distances[(int)Directions.LEFT] = -jumpDistanceSoFar;
                }

                if (node.isJumpPointComingFrom(Directions.RIGHT))
                {
                    jumpDistanceSoFar = 0;
                    jumpPointSeen = true;
                }
            }

            jumpDistanceSoFar = -1;
            jumpPointSeen = false;
            // 从右向左扫描，填充Right距离值
            for (int x = column - 1; x >= 0; x--)
            {
                JPSPlusNode node = jpsMapNodeArray[x, y];
                if (node.isObstacle)
                {
                    jumpDistanceSoFar = -1;
                    jumpPointSeen = false;
                    node.distances[(int)Directions.RIGHT] = 0;
                    continue;
                }

                jumpDistanceSoFar++;
                if (jumpPointSeen)
                {
                    node.distances[(int)Directions.RIGHT] = jumpDistanceSoFar;
                }
                else
                {
                    node.distances[(int)Directions.RIGHT] = -jumpDistanceSoFar;
                }

                if (node.isJumpPointComingFrom(Directions.LEFT))
                {
                    jumpDistanceSoFar = 0;
                    jumpPointSeen = true;
                }
            }
        }

        for (int x = 0; x < column; x++)
        {
            int jumpDistanceSoFar = -1;
            bool jumpPointSeen = false;
            // 从下向上扫描，填充Down距离值
            for (int y = 0; y < row; y++)
            {
                JPSPlusNode node = jpsMapNodeArray[x, y];
                if (node.isObstacle)
                {
                    jumpDistanceSoFar = -1;
                    jumpPointSeen = false;
                    node.distances[(int)Directions.DOWN] = 0;
                    continue;
                }

                jumpDistanceSoFar++;
                if (jumpPointSeen)
                {
                    node.distances[(int)Directions.DOWN] = jumpDistanceSoFar;
                }
                else
                {
                    node.distances[(int)Directions.DOWN] = -jumpDistanceSoFar;
                }

                if (node.isJumpPointComingFrom(Directions.UP))
                {
                    jumpDistanceSoFar = 0;
                    jumpPointSeen = true;
                }
            }

            jumpDistanceSoFar = -1;
            jumpPointSeen = false;
            // 从下向上扫描，填充Up距离值
            for (int y = row - 1; y >= 0; y--)
            {
                JPSPlusNode node = jpsMapNodeArray[x, y];
                if (node.isObstacle)
                {
                    jumpDistanceSoFar = -1;
                    jumpPointSeen = false;
                    node.distances[(int)Directions.UP] = 0;
                    continue;
                }

                jumpDistanceSoFar++;
                if (jumpPointSeen)
                {
                    node.distances[(int)Directions.UP] = jumpDistanceSoFar;
                }
                else
                {
                    node.distances[(int)Directions.UP] = -jumpDistanceSoFar;
                }

                if (node.isJumpPointComingFrom(Directions.DOWN))
                {
                    jumpDistanceSoFar = 0;
                    jumpPointSeen = true;
                }
            }
        }

        BuildStraightDebugInfo();
    }

    private void BuildDiagonalJumpPoint()
    {
        // 构建对角线跳点
        for (int y = 0; y < row; y++)
        {
            // 从左向右自下而上
            for (int x = 0; x < column; x++)
            {
                if (!isEmpty(x, y)) continue;
                JPSPlusNode node = jpsMapNodeArray[x, y];
                if (x == 0 || y == 0 || !isEmpty(x - 1, y) || !isEmpty(x, y - 1) || !isEmpty(x - 1, y - 1))
                {
                    node.distances[(int)Directions.LEFT_DOWN] = 0;
                }
                else if (isEmpty(x - 1, y) && isEmpty(x, y - 1) &&
                         (jpsMapNodeArray[x - 1, y - 1].distances[(int)Directions.LEFT] > 0 ||
                          jpsMapNodeArray[x - 1, y - 1].distances[(int)Directions.DOWN] > 0))
                {
                    node.distances[(int)Directions.LEFT_DOWN] = 1;
                }
                else
                {
                    int jumpDistance = jpsMapNodeArray[x - 1, y - 1].distances[(int)Directions.LEFT_DOWN];
                    if (jumpDistance > 0)
                    {
                        node.distances[(int)Directions.LEFT_DOWN] = 1 + jumpDistance;
                    }
                    else
                    {
                        node.distances[(int)Directions.LEFT_DOWN] = -1 + jumpDistance;
                    }
                }

                if (x == column - 1 || y == 0 || !isEmpty(x + 1, y) || !isEmpty(x, y - 1) || !isEmpty(x + 1, y - 1))
                {
                    node.distances[(int)Directions.RIGHT_DOWN] = 0;
                }
                else if (isEmpty(x + 1, y) && isEmpty(x, y - 1) &&
                         (jpsMapNodeArray[x + 1, y - 1].distances[(int)Directions.DOWN] > 0 ||
                          jpsMapNodeArray[x + 1, y - 1].distances[(int)Directions.RIGHT] > 0))
                {
                    node.distances[(int)Directions.RIGHT_DOWN] = 1;
                }
                else
                {
                    int jumpDistance = jpsMapNodeArray[x + 1, y - 1].distances[(int)Directions.RIGHT_DOWN];
                    if (jumpDistance > 0)
                    {
                        node.distances[(int)Directions.RIGHT_DOWN] = 1 + jumpDistance;
                    }
                    else
                    {
                        node.distances[(int)Directions.RIGHT_DOWN] = -1 + jumpDistance;
                    }
                }
            }
        }

        for (int y = row - 1; y >= 0; y--)
        {
            // 从左向右自上而下
            for (int x = 0; x < column; x++)
            {
                // 障碍不处理
                if (!isEmpty(x, y)) continue;

                JPSPlusNode node = jpsMapNodeArray[x, y];
                if (x == 0 || y == row - 1 || !isEmpty(x - 1, y) || !isEmpty(x, y + 1) || !isEmpty(x - 1, y + 1))
                {
                    node.distances[(int)Directions.LEFT_UP] = 0;
                }
                else if (isEmpty(x - 1, y) && isEmpty(x, y + 1) &&
                         (jpsMapNodeArray[x - 1, y + 1].distances[(int)Directions.LEFT] > 0 ||
                          jpsMapNodeArray[x - 1, y + 1].distances[(int)Directions.UP] > 0))

                {
                    node.distances[(int)Directions.LEFT_UP] = 1;
                }
                else
                {
                    int jumpDistance = jpsMapNodeArray[x - 1, y + 1].distances[(int)Directions.LEFT_UP];
                    if (jumpDistance > 0)
                    {
                        node.distances[(int)Directions.LEFT_UP] = 1 + jumpDistance;
                    }
                    else
                    {
                        node.distances[(int)Directions.LEFT_UP] = -1 + jumpDistance;
                    }
                }

                if (x == column - 1 || y == row - 1 || !isEmpty(x + 1, y) || !isEmpty(x, y + 1) ||
                    !isEmpty(x + 1, y + 1))
                {
                    node.distances[(int)Directions.RIGHT_UP] = 0;
                }
                else if (isEmpty(x + 1, y) && isEmpty(x, y + 1) &&
                         (jpsMapNodeArray[x + 1, y + 1].distances[(int)Directions.RIGHT] > 0 ||
                          jpsMapNodeArray[x + 1, y + 1].distances[(int)Directions.UP] > 0))
                {
                    node.distances[(int)Directions.RIGHT_UP] = 1;
                }
                else
                {
                    int jumpDistance = jpsMapNodeArray[x + 1, y + 1].distances[(int)Directions.RIGHT_UP];
                    if (jumpDistance > 0)
                    {
                        node.distances[(int)Directions.RIGHT_UP] = 1 + jumpDistance;
                    }
                    else
                    {
                        node.distances[(int)Directions.RIGHT_UP] = -1 + jumpDistance;
                    }
                }
            }
        }
    }

    private bool isInBound(int x, int y)
    {
        if (x < 0 || y < 0 || x >= column || y >= row)
        {
            return false;
        }

        return true;
    }

    private bool isEmpty(int x, int y)
    {
        return isInBound(x, y) && !jpsMapNodeArray[x, y].isObstacle;
    }

    private void BuildStraightDebugInfo()
    {
        return;
        GameObject prefab = App.Make<IAssetsManager>().GetAssetByUrlSync<GameObject>(AssetsPath.MapLocationPath);
        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                JPSPlusNode node = jpsMapNodeArray[x, y];
                for (Directions i = Directions.UP; i <= Directions.LEFT_UP; i++)
                {
                    if (node.isJumpPoint)
                    {
                        GameObject jumpPointNode = App.Make<IObjectPool>().RequestInstance(prefab);
                        jumpPointNode.transform.position = node.pos;
                    }
                }
            }
        }
    }

    private Vector2Int GetDirOffset(Directions dir)
    {
        switch (dir)
        {
            case Directions.UP:
                return new Vector2Int(0, 1);
            case Directions.DOWN:
                return new Vector2Int(0, -1);
            case Directions.LEFT:
                return new Vector2Int(-1, 0);
            case Directions.RIGHT:
                return new Vector2Int(1, 0);
            case Directions.LEFT_UP:
                return new Vector2Int(-1, 1);
            case Directions.RIGHT_UP:
                return new Vector2Int(1, 1);
            case Directions.LEFT_DOWN:
                return new Vector2Int(-1, -1);
            case Directions.RIGHT_DOWN:
                return new Vector2Int(1, -1);
        }

        return Vector2Int.zero;
    }

    #endregion

    private bool IsObstacle(int x, int y)
    {
        Vector3Int tileIndex = new Vector3Int(x, y, 0);
        if (!curMap.GroundTileMap.HasTile(tileIndex))
        {
            return true;
        }

        if (curMap.ObstacleTileMap.HasTile(tileIndex))
        {
            return true;
        }

        return false;
    }

    private Vector3 GetPosByTileIndex(int x, int y)
    {
        Vector3Int tileIndex = new Vector3Int(x, y, 0);
        // 返回的是格子左下角坐标
        var pos = curMap.GroundTileMap.CellToWorld(tileIndex);

        // 将坐标转换为格子中心点坐标
        var cellSize = curMap.GroundTileMap.layoutGrid.cellSize;
        pos.x += cellSize.x / 2;
        pos.y += cellSize.y / 2;
        return pos;
    }

    private Vector2Int ConvertTileIndexToCellIndex(int tileX, int tileY)
    {
        int x = tileX + Mathf.Abs(leftBottomIndex.x);
        int y = tileY + Mathf.Abs(leftBottomIndex.y);
        return new Vector2Int(x, y);
    }

    public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
    {
        var endCellIndex = curMap.GroundTileMap.WorldToCell(endPos);
        var startCellIndex = curMap.GroundTileMap.WorldToCell(startPos);

        var startNodeArrayIndex = ConvertTileIndexToCellIndex(startCellIndex.x, startCellIndex.y);
        var endNodeArrayIndex = ConvertTileIndexToCellIndex(endCellIndex.x, endCellIndex.y);
        List<Vector3> pathPosList;
        if (useJPSPlus)
        {
            pathPosList = pathFinding.FindPathByJpsPlus(startNodeArrayIndex, endNodeArrayIndex);
        }
        else if (useJPS)
        {
            pathPosList = pathFinding.FindPathByJps(startNodeArrayIndex, endNodeArrayIndex);
        }
        else
        {
            pathPosList = pathFinding.FindPath(startNodeArrayIndex, endNodeArrayIndex);
        }

        DrawPathPoint(pathPosList);
        return pathPosList;
    }

    public bool IsOpenDebug => isOpenDebug;

    private List<GameObject> pathPointList = new List<GameObject>();

    private void DrawPathPoint(List<Vector3> pathPosList)
    {
        if (!isOpenDebug || pathPosList == null)
        {
            return;
        }

        foreach (var point in pathPointList)
        {
            App.Make<IObjectPool>().ReturnInstance(point);
        }

        GameObject locationPrefab =
            App.Make<IAssetsManager>().GetAssetByUrlSync<GameObject>(AssetsPath.MapLocationPath);
        foreach (var pos in pathPosList)
        {
            GameObject locationNode = App.Make<IObjectPool>().RequestInstance(locationPrefab);
            locationNode.transform.SetParent(App.Make<NodeManager>().MapLayerTrans);
            locationNode.transform.position = pos;
            pathPointList.Add(locationNode);
        }
    }
}