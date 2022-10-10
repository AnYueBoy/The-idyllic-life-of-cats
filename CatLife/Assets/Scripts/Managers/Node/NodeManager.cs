using UnityEngine;

public class NodeManager : IManager
{
    private Transform worldUITrans;
    private Transform roleLayerTrans;


    public void Inject(Transform worldUITrans, Transform roleLayerTrans)
    {
        this.worldUITrans = worldUITrans;
        this.roleLayerTrans = roleLayerTrans;
    }

    public void Init()
    {
    }

    public void LocalUpdate(float dt)
    {
    }


    public Transform WorldUITrans => worldUITrans;
    public Transform RoleLayerTrans => roleLayerTrans;
}