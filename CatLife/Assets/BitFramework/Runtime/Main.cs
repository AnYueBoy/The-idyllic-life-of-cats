using System;
using BitFramework.Core;
using BitFramework.Runtime;
using BitFramework.Util;
using UnityEngine;

public class Main : Framework
{
    protected override void OnStartCompleted(IApplication application, StartCompletedEventArgs args)
    {
        // 框架完成
    }

    protected override IBootstrap[] GetBootstraps()
    {
        return Arr.Merge(base.GetBootstraps(), Bootstraps.GetBootstraps(this));
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        App.Make<RoleManager>().LocalUpdate(dt);
    }
}