using BitFramework.Core;

public class ProviderInputManager : IServiceProvider
{
    public void Init()
    {
        App.Make<InputManager>().Init();
    }

    public void Register()
    {
        App.Singleton<InputManager>();
    }
}