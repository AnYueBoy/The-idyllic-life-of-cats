using BitFramework.Core;
using UnityEngine;

public class ProviderGameManager : MonoBehaviour, IServiceProvider
{
    public void Init()
    {
    }

    public void Register()
    {
        App.Singleton<GameManager>();
    }
}