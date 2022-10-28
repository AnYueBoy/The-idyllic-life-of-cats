using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap groundTileMap;
    [SerializeField] private Tilemap obstacleTileMap;
    [SerializeField] private Tilemap farmTileMap;

    public Tilemap GroundTileMap => groundTileMap;
    public Tilemap ObstacleTileMap => obstacleTileMap;
    public Tilemap FarmTileMap => farmTileMap;

    #region 预处理地图

    [Button("预处理地图", ButtonSizes.Large)]
    private void PreprocessMap()
    {
        Vector3Int leftBottomIndex = groundTileMap.origin;
        var size = groundTileMap.size;
        column = size.x;
        row = size.y;

        // 平面直角坐标系 x轴向右 y轴向上
        jpsMapNodeArray = new JPSPlusNode[column, row];

        for (int x = leftBottomIndex.x; x < leftBottomIndex.x + column; x++)
        {
            for (int y = leftBottomIndex.y; y < leftBottomIndex.y + row; y++)
            {
                bool isObstacle = MapUtil.IsObstacle(groundTileMap, obstacleTileMap, x, y);
                Vector3 pos = MapUtil.GetPosByTileIndex(groundTileMap, x, y);

                Vector2Int nodeCellIndex = MapUtil.ConvertTileIndexToCellIndex(groundTileMap, x, y);

                // 构建JPS+的节点信息
                jpsMapNodeArray[nodeCellIndex.x, nodeCellIndex.y] = new JPSPlusNode(isObstacle, pos,
                    nodeCellIndex.x,
                    nodeCellIndex.y, new Vector3Int(x, y, 0));
            }
        }
        
        BuildPrimaryJumpPoints();
        BuildStraightJumpPoint();
        BuildDiagonalJumpPoint();
        
        // TODO: 写入数据
    }

    private JPSPlusNode[,] jpsMapNodeArray;
    private int row, column;

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

    #endregion
}