using BitFramework.Core;
using UnityEngine;

public class ProviderNodeManager : MonoBehaviour, IServiceProvider
{
    [SerializeField] private Transform worldUITrans;

    public void Init()
    {
        App.Make<NodeManager>().Inject(worldUITrans);
        App.Make<NodeManager>().Init();
    }

    public void Register()
    {
        App.Singleton<NodeManager>();
    }
}