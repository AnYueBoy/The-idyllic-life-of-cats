using BitFramework.Core;

public class ProviderRoleManager : IServiceProvider
{
    public void Init()
    {
      
    }

    public void Register()
    {
        App.Singleton<RoleManager>();
    }
}