using BitFramework.Core;

public class ProviderInputManager : IServiceProvider
{
    public void Init()
    {
    }

    public void Register()
    {
        App.Singleton<InputManager>();
    }
}