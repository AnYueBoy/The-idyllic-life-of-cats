using System;
using BitFramework.Core;
using UnityEngine;
using IServiceProvider = BitFramework.Core.IServiceProvider;

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

    private void OnApplicationPause(bool pauseStatus)
    {
        App.Make<DataManager>().OnApplicationPause(pauseStatus);
    }
}