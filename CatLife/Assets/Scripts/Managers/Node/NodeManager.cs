using UnityEngine;

public class NodeManager : IManager
{
    private Transform worldUITrans;
    private Transform roleLayerTrans;
    private Transform mapLayerTrans;


    public void Inject(Transform worldUITrans, Transform roleLayerTrans, Transform mapLayerTrans)
    {
        this.worldUITrans = worldUITrans;
        this.roleLayerTrans = roleLayerTrans;
        this.mapLayerTrans = mapLayerTrans;
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
}