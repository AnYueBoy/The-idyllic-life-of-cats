using BitFramework.Core;

public class ProviderMapManager : IServiceProvider
{
    public void Init()
    {
    }

    public void Register()
    {
        App.Singleton<MapManager>();
    }
}