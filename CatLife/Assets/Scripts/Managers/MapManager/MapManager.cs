using BitFramework.Core;

public class MapManager : IManager
{
    public void Init()
    {
        InitMap();
    }

    public void LocalUpdate(float dt)
    {
    }

    private void InitMap()
    {
        App.Make<SpawnManager>().SpawnMap();
    }
}