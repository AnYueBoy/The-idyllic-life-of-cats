using BitFramework.Core;

public class ProviderGameManager : IServiceProvider
{
    public void Init()
    {
    }

    public void Register()
    {
        App.Singleton<GameManager>();
    }
}