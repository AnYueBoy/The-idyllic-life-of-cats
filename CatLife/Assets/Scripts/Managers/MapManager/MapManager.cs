using System;
using System.Collections.Generic;
using System.IO;
using BitFramework.Component.AssetsModule;
using BitFramework.Component.ObjectPoolModule;
using BitFramework.Core;
using Sirenix.Serialization;
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
        BuildMapBaseInfo();

        if (useJPSPlus)
        {
            LoadJPSPlusMapInfo();
            if (isOpenDebug)
            {
                BuildPrimaryDebugInfo();
                BuildStraightDebugInfo();
                BuildDiagonalDebugInfo();
            }
        }
        else
        {
            BuildNonJPSPlusNode();
        }

        pathFinding.Init(column, row, mapNodeCellArray, jpsMapNodeArray);
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

    private NodeCell[,] mapNodeCellArray;

    private void BuildNonJPSPlusNode()
    {
        // 平面直角坐标系 x轴向右 y轴向上
        mapNodeCellArray = new NodeCell[column, row];
        for (int x = leftBottomIndex.x; x < leftBottomIndex.x + column; x++)
        {
            for (int y = leftBottomIndex.y; y < leftBottomIndex.y + row; y++)
            {
                bool isObstacle = MapUtil.IsObstacle(curMap.GroundTileMap, curMap.ObstacleTileMap, x, y);
                Vector3 pos = MapUtil.GetPosByTileIndex(curMap.GroundTileMap, x, y);

                Vector2Int nodeCellIndex = MapUtil.ConvertTileIndexToCellIndex(curMap.GroundTileMap, x, y);

                // 构建非JPS+的节点信息
                mapNodeCellArray[nodeCellIndex.x, nodeCellIndex.y] =
                    new NodeCell(isObstacle, pos, nodeCellIndex.x, nodeCellIndex.y, new Vector3Int(x, y, 0));
            }
        }
    }

    private void BuildMapBaseInfo()
    {
        leftBottomIndex = curMap.GroundTileMap.origin;
        var size = curMap.GroundTileMap.size;
        column = size.x;
        row = size.y;
    }

    private void LoadJPSPlusMapInfo()
    {
        string mapInfoPath = Application.dataPath + AssetsPath.JPSPlusMapDirPath + curMap.gameObject.name + ".json";
        var mapInfoJson = File.ReadAllBytes(mapInfoPath);
        jpsMapNodeArray = SerializationUtility.DeserializeValue<JPSPlusNode[,]>(mapInfoJson, DataFormat.JSON);
    }

    #region JPS+ Preprocess Map

    private JPSPlusNode[,] jpsMapNodeArray;

    private void BuildPrimaryDebugInfo()
    {
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

    private void BuildStraightDebugInfo()
    {
        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                JPSPlusNode node = jpsMapNodeArray[x, y];
                for (Directions i = Directions.UP; i <= Directions.LEFT_UP; i++)
                {
                    int distance = node.distances[(int)i];
                    if (distance == int.MinValue || node.debugInfo[(int)i] != null)
                    {
                        continue;
                    }

                    Vector2Int offset = GetDirOffset(i);
                    GameObject infoNode = new GameObject();
                    var infoComp = infoNode.AddComponent<TextMesh>();
                    infoComp.fontSize = 170;
                    infoComp.transform.localScale = Vector3.one * 0.01f;
                    infoNode.transform.position = node.pos + new Vector3(offset.x * 0.3f, offset.y * 0.3f, -1);
                    infoComp.text = distance.ToString();
                    if (distance > 0)
                    {
                        infoComp.color = Color.blue;
                    }
                    else
                    {
                        infoComp.color = Color.red;
                    }

                    node.debugInfo[(int)i] = infoComp;
                }
            }
        }
    }

    private void BuildDiagonalDebugInfo()
    {
        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                JPSPlusNode node = jpsMapNodeArray[x, y];
                for (Directions i = Directions.UP; i <= Directions.LEFT_UP; i++)
                {
                    int distance = node.distances[(int)i];
                    if (distance == int.MinValue || node.debugInfo[(int)i] != null)
                    {
                        continue;
                    }

                    Vector2Int offset = GetDirOffset(i);
                    GameObject infoNode = new GameObject();
                    var infoComp = infoNode.AddComponent<TextMesh>();
                    infoComp.fontSize = 170;
                    infoComp.transform.localScale = Vector3.one * 0.01f;
                    infoNode.transform.position = node.pos + new Vector3(offset.x * 0.3f, offset.y * 0.3f, -1);
                    infoComp.text = distance.ToString();
                    if (distance > 0)
                    {
                        infoComp.color = Color.green;
                    }
                    else
                    {
                        infoComp.color = Color.red;
                    }

                    node.debugInfo[(int)i] = infoComp;
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

    public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
    {
        var endCellIndex = curMap.GroundTileMap.WorldToCell(endPos);
        var startCellIndex = curMap.GroundTileMap.WorldToCell(startPos);

        var startNodeArrayIndex =
            MapUtil.ConvertTileIndexToCellIndex(curMap.GroundTileMap, startCellIndex.x, startCellIndex.y);
        var endNodeArrayIndex =
            MapUtil.ConvertTileIndexToCellIndex(curMap.GroundTileMap, endCellIndex.x, endCellIndex.y);
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