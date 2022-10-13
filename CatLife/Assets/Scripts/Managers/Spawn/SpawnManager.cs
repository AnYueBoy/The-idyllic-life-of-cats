using BitFramework.Component.AssetsModule;
using BitFramework.Component.ObjectPoolModule;
using BitFramework.Core;
using UnityEngine;

public class SpawnManager : IManager
{
    public void Init()
    {
    }

    public void LocalUpdate(float dt)
    {
    }

    public BaseRole SpawnCat()
    {
        GameObject catPrefab = App.Make<IAssetsManager>().GetAssetByUrlSync<GameObject>(AssetsPath.CatPath);
        GameObject catNode = App.Make<IObjectPool>().RequestInstance(catPrefab);
        catNode.transform.SetParent(App.Make<NodeManager>().RoleLayerTrans);

        Cat cat = catNode.GetComponent<Cat>();
        cat.Init();
        App.Make<RoleManager>().AddRole(cat);

        return cat;
    }

    public Map SpawnMap()
    {
        GameObject mapPrefab = App.Make<IAssetsManager>().GetAssetByUrlSync<GameObject>(AssetsPath.Map1Path);
        GameObject mapNode = App.Make<IObjectPool>().RequestInstance(mapPrefab);
        mapNode.transform.SetParent(App.Make<NodeManager>().MapLayerTrans);

        Map map = mapNode.GetComponent<Map>();
        return map;
    }
}