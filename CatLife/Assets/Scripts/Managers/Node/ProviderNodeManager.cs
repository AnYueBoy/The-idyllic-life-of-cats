using BitFramework.Core;
using UnityEngine;

public class ProviderNodeManager : MonoBehaviour, IServiceProvider
{
    [SerializeField] private Transform worldUITrans;
    [SerializeField] private Transform roleLayerTrans;
    [SerializeField] private Transform mapLayerTrans;

    public void Init()
    {
        App.Make<NodeManager>().Inject(worldUITrans, roleLayerTrans, mapLayerTrans);
    }

    public void Register()
    {
        App.Singleton<NodeManager>();
    }
}