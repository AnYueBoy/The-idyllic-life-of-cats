using UnityEngine;

public class NodeManager : IManager
{
    private Transform worldUITrans;


    public void Inject(Transform worldUITrans)
    {
        this.worldUITrans = worldUITrans;
    }

    public void Init()
    {
    }

    public void LocalUpdate(float dt)
    {
    }


    public Transform WorldUITrans => worldUITrans;
}