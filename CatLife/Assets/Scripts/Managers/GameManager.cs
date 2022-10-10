using BitFramework.Core;

public class GameManager : IManager
{
    public void Init()
    {
        App.Make<DataManager>().Init();
        App.Make<InputManager>().Init();
        App.Make<RoleManager>().Init();
        App.Make<NodeManager>().Init();
    }

    public void LocalUpdate(float dt)
    {
        App.Make<DataManager>().LocalUpdate(dt);
        App.Make<InputManager>().LocalUpdate(dt);
        App.Make<RoleManager>().LocalUpdate(dt);
        App.Make<NodeManager>().LocalUpdate(dt);
    }
}