using BitFramework.Core;
using UnityEngine;

public class ProviderNodeManager : MonoBehaviour, IServiceProvider
{
    [SerializeField] private Transform worldUITrans;

    public void Init()
    {
        App.Make<NodeManager>().Inject(worldUITrans);
    }

    public void Register()
    {
        App.Singleton<NodeManager>();
    }
}