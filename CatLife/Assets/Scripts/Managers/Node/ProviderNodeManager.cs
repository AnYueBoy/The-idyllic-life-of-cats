using BitFramework.Core;
using UnityEngine;

public class ProviderNodeManager : MonoBehaviour, IServiceProvider
{
    [SerializeField] private Transform worldUITrans;
    [SerializeField] private Transform roleLayerTrans;

    public void Init()
    {
        App.Make<NodeManager>().Inject(worldUITrans, roleLayerTrans);
    }

    public void Register()
    {
        App.Singleton<NodeManager>();
    }
}