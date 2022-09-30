using System;
using BitFramework.Core;
using BitFramework.Scripts;
using UnityEngine;

namespace BitFramework.Runtime
{
    public abstract class Framework : MonoBehaviour
    {
        public DebugLevel DebugLevel = DebugLevel.Production;
        private BitApplication bitApplication;

        public IApplication Application
        {
            get => bitApplication;
        }

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            App.That = bitApplication = CreateApplication(DebugLevel);
            BeforeBootstrap(bitApplication);
            bitApplication.Bootstrap(GetBootstraps());
        }

        protected virtual void Start()
        {
            bitApplication.Init();
        }

        protected virtual void OnDestroy()
        {
            // FIXME: 因不同节点的生命周期不一致，因此可能造成相关实例已回收，但其他节点仍在访问的情况
            // bitApplication?.Terminate();
        }

        protected virtual BitApplication CreateApplication(DebugLevel debugLevel)
        {
            return new UnityApplication(this)
            {
                DebugLevel = debugLevel
            };
        }

        /// <summary>
        /// 返回引导程序数组
        /// </summary>
        protected virtual IBootstrap[] GetBootstraps()
        {
            return GetComponents<IBootstrap>();
        }

        /// <summary>
        /// 引导开始前触发
        /// </summary>
        protected virtual void BeforeBootstrap(IApplication application)
        {
            application.GetDispatcher()?.AddListener(BitApplicationEvents.OnStartCompleted,
                (sender, args) => { OnStartCompleted((IApplication) sender, (StartCompletedEventArgs) args); });
        }

        /// <summary>
        /// 触发框架开始事件
        /// </summary>
        protected abstract void OnStartCompleted(IApplication application, StartCompletedEventArgs args);
    }
}