using UnityEngine;
using UnityEngine.Tilemaps;

public class NodeManager : IManager
{
    private Transform worldUITrans;
    private Transform roleLayerTrans;
    private Transform mapLayerTrans;
    private Canvas canvasLayer;

    public Tile pathTile;


    public void Inject(Transform worldUITrans, Transform roleLayerTrans, Transform mapLayerTrans, Tile pathTile,
        Canvas canvasLayer)
    {
        this.worldUITrans = worldUITrans;
        this.roleLayerTrans = roleLayerTrans;
        this.mapLayerTrans = mapLayerTrans;
        this.pathTile = pathTile;
        this.canvasLayer = canvasLayer;
    }

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