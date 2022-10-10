using System;
using BitFramework.Component.AssetsModule;
using BitFramework.Component.ObjectPoolModule;
using BitFramework.Core;
using BitFramework.Runtime;
using BitFramework.Util;
using UnityEngine;

public class Main : Framework
{
    protected override void OnStartCompleted(IApplication application, StartCompletedEventArgs args)
    {
        // 框架初始化完成
        // 初始化GameManager
        App.Make<GameManager>().Init();

        GameObject catPrefab = App.Make<IAssetsManager>().GetAssetByUrlSync<GameObject>("Cat");
        GameObject catNode = App.Make<IObjectPool>().RequestInstance(catPrefab);
        Cat cat = catNode.GetComponent<Cat>();
        cat.Init();
        App.Make<RoleManager>().AddRole(cat);
    }

    protected override IBootstrap[] GetBootstraps()
    {
        return Arr.Merge(base.GetBootstraps(), Bootstraps.GetBootstraps(this));
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        App.Make<GameManager>().LocalUpdate(dt);
    }
}