using BitFramework.Core;

public class ProviderSpawnManager : IServiceProvider
{
    public void Init()
    {
    }

    public void Register()
    {
        App.Singleton<SpawnManager>();
    }
}