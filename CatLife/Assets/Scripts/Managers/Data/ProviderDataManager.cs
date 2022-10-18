using BitFramework.Core;
using UnityEngine;
using IServiceProvider = BitFramework.Core.IServiceProvider;

public class ProviderDataManager : MonoBehaviour, IServiceProvider
{
    public void Init()
    {
    }

    public void Register()
    {
        App.Instance<DataManager>(GetComponent<DataManager>());
    }
}