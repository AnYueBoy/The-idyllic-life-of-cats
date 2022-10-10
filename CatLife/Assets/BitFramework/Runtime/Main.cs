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