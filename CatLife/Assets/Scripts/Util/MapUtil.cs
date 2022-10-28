using UnityEngine;
using UnityEngine.Tilemaps;

public class MapUtil
{
    public static bool IsObstacle(Tilemap groundTileMap, Tilemap obstacleTileMap, int x, int y)
    {
        Vector3Int tileIndex = new Vector3Int(x, y, 0);
        if (!groundTileMap.HasTile(tileIndex))
        {
            return true;
        }

        if (obstacleTileMap.HasTile(tileIndex))
        {
            return true;
        }

        return false;
    }

    public static Vector3 GetPosByTileIndex(Tilemap groundTileMap, int x, int y)
    {
        Vector3Int tileIndex = new Vector3Int(x, y, 0);
        // 返回的是格子左下角坐标
        var pos = groundTileMap.CellToWorld(tileIndex);

        // 将坐标转换为格子中心点坐标
        var cellSize = groundTileMap.layoutGrid.cellSize;
        pos.x += cellSize.x / 2;
        pos.y += cellSize.y / 2;
        return pos;
    }
    
    public static Vector2Int ConvertTileIndexToCellIndex(Tilemap groundTileMap,int tileX, int tileY)
    {
        var leftBottomIndex = groundTileMap.origin;
        int column = groundTileMap.size.x;
        int row = groundTileMap.size.y;
        int x = tileX + Mathf.Abs(leftBottomIndex.x);
        x = Mathf.Clamp(x, 0, column - 1);
        int y = tileY + Mathf.Abs(leftBottomIndex.y);
        y = Mathf.Clamp(y, 0, row - 1);
        return new Vector2Int(x, y);
    }
}