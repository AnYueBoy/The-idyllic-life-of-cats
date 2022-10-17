using System;
using System.Collections.Generic;
using BitFramework.Core;
using UnityEngine;

public class MapManager : IManager
{
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
    private PathFinding pathFinding;
    private Vector3Int tileMapSize;
    private Vector3Int leftBottomIndex;
    private Action<List<NodeCell>> pathGenerateCallback;

    private void ScanMapInfo()
    {
        leftBottomIndex = curMap.GroundTileMap.origin;
        tileMapSize = curMap.GroundTileMap.size;

        var mapNodeCellArray = new NodeCell[tileMapSize.x, tileMapSize.y];
        for (int x = leftBottomIndex.x; x < leftBottomIndex.x + tileMapSize.x; x++)
        {
            for (int y = leftBottomIndex.y; y < leftBottomIndex.y + tileMapSize.y; y++)
            {
                bool isObstacle = IsObstacle(x, y);
                Vector3 pos = GetPosByTileIndex(x, y);

                Vector2Int nodeCellIndex = ConvertTileIndexToCellIndex(x, y);
                mapNodeCellArray[nodeCellIndex.x, nodeCellIndex.y] =
                    new NodeCell(isObstacle, pos, nodeCellIndex.x, nodeCellIndex.y, new Vector3Int(x, y, 0));
            }
        }

        pathFinding.Init(tileMapSize.x, tileMapSize.y, mapNodeCellArray);
    }

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
        List<Vector3> pathPosList = pathFinding.FindPathByJps(startNodeArrayIndex, endNodeArrayIndex);
        List<NodeCell> pathNodeList = pathFinding.GeneratePathCallback();
        DrawPath(pathNodeList);
        return pathPosList;
    }

    private void DrawPath(List<NodeCell> pathNodeList)
    {
        if (pathNodeList == null || pathNodeList.Count <= 0)
        {
            return;
        }
        
        curMap.FarmTileMap.ClearAllTiles();

        foreach (NodeCell nodeCell in pathNodeList)
        {
            curMap.FarmTileMap.SetTile(nodeCell.mappingTileIndex, App.Make<NodeManager>().pathTile);
        }
    }
}