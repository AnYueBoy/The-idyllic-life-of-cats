using UnityEngine;
using UnityEngine.Tilemaps;

public class NodeManager : MonoBehaviour, IManager
{
    [SerializeField] private Transform worldUITrans;
    [SerializeField] private Transform roleLayerTrans;
    [SerializeField] private Transform mapLayerTrans;
    [SerializeField] private Canvas canvasLayer;

    public Tile pathTile;

    public void Init()
    {
    }

    public void LocalUpdate(float dt)
    {
    }


    public Transform WorldUITrans => worldUITrans;
    public Transform RoleLayerTrans => roleLayerTrans;
    public Transform MapLayerTrans => mapLayerTrans;
    public Canvas CanvasLayer => canvasLayer;
}