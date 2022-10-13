using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap groundTileMap;
    [SerializeField] private Tilemap obstacleTileMap;

    public Tilemap GroundTileMap => groundTileMap;
    public Tilemap ObstacleTileMap => obstacleTileMap;
}