using BitFramework.Core;
using UnityEngine;

public class ProviderDataManager : MonoBehaviour, IServiceProvider
{
    public void Init()
    {
        App.Make<DataManager>().Init();
    }

    public void Register()
    {
        App.Singleton<DataManager>();
    }
}