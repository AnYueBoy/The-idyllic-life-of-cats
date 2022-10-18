using BitFramework.Core;
using UnityEngine;

public class ProviderMapManager : MonoBehaviour, IServiceProvider
{
    public void Init()
    {
    }

    public void Register()
    {
        App.Instance<MapManager>(GetComponent<MapManager>());
    }
}