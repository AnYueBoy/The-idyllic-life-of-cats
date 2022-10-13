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

    private void ScanMapInfo()
    {
        var leftBottomIndex = curMap.GroundTileMap.origin;

        var horizontalValue = Mathf.Abs(leftBottomIndex.x);
        var verticalValue = Mathf.Abs(leftBottomIndex.y);
        var mapNodeCellArray = new NodeCell[horizontalValue, verticalValue];
        for (int x = leftBottomIndex.x; x <= Mathf.Abs(leftBottomIndex.x); x++)
        {
            for (int y = leftBottomIndex.y; y <= Mathf.Abs(leftBottomIndex.y); y++)
            {
                bool isObstacle = IsObstacle(x, y);
                Vector3 pos = GetPosByTileIndex(x, y);
                mapNodeCellArray[x, y] = new NodeCell(isObstacle, pos, x, y);
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
}