using BitFramework.Core;
using UnityEngine;

public class ProviderNodeManager : MonoBehaviour, IServiceProvider
{
    public void Init()
    {
    }

    public void Register()
    {
        App.Instance<NodeManager>(GetComponent<NodeManager>());
    }
}