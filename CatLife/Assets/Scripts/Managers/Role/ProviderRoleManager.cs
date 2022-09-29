using BitFramework.Core;

public class ProviderRoleManager : IServiceProvider
{
    public void Init()
    {
        App.Make<RoleManager>().Init();
    }

    public void Register()
    {
        App.Singleton<RoleManager>();
    }
}