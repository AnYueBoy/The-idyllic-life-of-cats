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
    private int horizontalValue;
    private int verticalValue;

    private void ScanMapInfo()
    {
        var leftBottomIndex = curMap.GroundTileMap.origin;

        horizontalValue = leftBottomIndex.x * leftBottomIndex.x;
        verticalValue = leftBottomIndex.y * leftBottomIndex.y;
        var mapNodeCellArray = new NodeCell[horizontalValue, verticalValue];
        for (int x = leftBottomIndex.x; x <= Mathf.Abs(leftBottomIndex.x); x++)
        {
            for (int y = leftBottomIndex.y; y <= Mathf.Abs(leftBottomIndex.y); y++)
            {
                bool isObstacle = IsObstacle(x, y);
                Vector3 pos = GetPosByTileIndex(x, y);

                Vector2Int nodeCellIndex = ConvertTileIndexToCellIndex(x, y);
                mapNodeCellArray[nodeCellIndex.x, nodeCellIndex.y] =
                    new NodeCell(isObstacle, pos, nodeCellIndex.x, nodeCellIndex.y);
            }
        }

        pathFinding.Init(horizontalValue, verticalValue, mapNodeCellArray);
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
        int x = tileX + (horizontalValue >> 1);
        int y = tileY + (verticalValue >> 1);
        return new Vector2Int(x, y);
    }

    public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
    {
        var endLocalPos = curMap.GroundTileMap.WorldToLocal(endPos);
        var endCellIndex = curMap.GroundTileMap.LocalToCell(endLocalPos);

        var startLocalPos = curMap.GroundTileMap.WorldToCell(startPos);
        var startCellIndex = curMap.GroundTileMap.LocalToCell(startLocalPos);

        var startNodeArrayIndex = ConvertTileIndexToCellIndex(startCellIndex.x, startCellIndex.y);
        var endNodeArrayIndex = ConvertTileIndexToCellIndex(endCellIndex.x, endCellIndex.y);
        return pathFinding.FindPath(startNodeArrayIndex, endNodeArrayIndex);
    }
}