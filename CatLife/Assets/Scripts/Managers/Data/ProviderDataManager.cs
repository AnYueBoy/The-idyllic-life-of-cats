using BitFramework.Core;

public class ProviderDataManager : IServiceProvider
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