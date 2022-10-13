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
                    new NodeCell(isObstacle, pos, nodeCellIndex.x, nodeCellIndex.y);
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
        var pos = curMap.GroundTileMap.CellToLocal(tileIndex);
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
        return pathFinding.FindPath(startNodeArrayIndex, endNodeArrayIndex);
    }
}