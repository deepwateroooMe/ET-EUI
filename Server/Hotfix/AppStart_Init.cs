using System;
using System.Net;
namespace ET { // 这是服务器热更新 Hotfix 里的启动
    public class AppStart_Init: AEvent<EventType.AppStart> {
        protected override void Run(EventType.AppStart args) {
            RunAsync(args).Coroutine();
        }
        
        private async ETTask RunAsync(EventType.AppStart args) {
            Game.Scene.AddComponent<ConfigComponent>();
            await ConfigComponent.Instance.LoadAsync();
            StartProcessConfig processConfig = StartProcessConfigCategory.Instance.Get(Game.Options.Process);
            Game.Scene.AddComponent<TimerComponent>();
            Game.Scene.AddComponent<OpcodeTypeComponent>(); // 它说，服务器运行时，这里加载这个组件出错了：原因是某个内网消息找不到返回类型？
            Game.Scene.AddComponent<MessageDispatcherComponent>();
            Game.Scene.AddComponent<SessionStreamDispatcher>();
            Game.Scene.AddComponent<CoroutineLockComponent>();
            // 发送普通actor消息
            Game.Scene.AddComponent<ActorMessageSenderComponent>();
            // 发送location actor消息
            Game.Scene.AddComponent<ActorLocationSenderComponent>();
            // 访问location server的组件
            Game.Scene.AddComponent<LocationProxyComponent>();
            Game.Scene.AddComponent<ActorMessageDispatcherComponent>();

// 添加数据库管理组件DBManagerComponent, Account就有了对数据库进行操作的能力
            Game.Scene.AddComponent<DBManagerComponent>(); // 可以直接在根节点Scene上面挂载一个，然后整个进程都可以用了
            // 如果一个进程有多个DBManagerComponent，则会发生错乱
            //     因为它是一个单例
            //     所以一个服务器进程只需要一个，也就是一个区服只需要一个

            // 数值订阅组件
            Game.Scene.AddComponent<NumericWatcherComponent>();
            Game.Scene.AddComponent<NetThreadComponent>();
            
            Game.Scene.AddComponent<NavmeshComponent, Func<string, byte[]>>(RecastFileReader.Read);
            switch (Game.Options.AppType) {
            case AppType.Server: {
                Game.Scene.AddComponent<NetInnerComponent, IPEndPoint, int>(processConfig.InnerIPPort, SessionStreamDispatcherType.SessionStreamDispatcherServerInner);
                var processScenes = StartSceneConfigCategory.Instance.GetByProcess(Game.Options.Process);
                foreach (StartSceneConfig startConfig in processScenes) {
                    await SceneFactory.Create(Game.Scene, startConfig.Id, startConfig.InstanceId, startConfig.Zone, startConfig.Name,
                                              startConfig.Type, startConfig);
                }
                break;
            }
            case AppType.Watcher: {
                StartMachineConfig startMachineConfig = WatcherHelper.GetThisMachineConfig();
                WatcherComponent watcherComponent = Game.Scene.AddComponent<WatcherComponent>();
                watcherComponent.Start(Game.Options.CreateScenes);
                Game.Scene.AddComponent<NetInnerComponent, IPEndPoint, int>(NetworkHelper.ToIPEndPoint($"{startMachineConfig.InnerIP}:{startMachineConfig.WatcherPort}"), SessionStreamDispatcherType.SessionStreamDispatcherServerInner);
                break;
            }
            case AppType.GameTool:
                break;
            }
            if (Game.Options.Console == 1) {
                Game.Scene.AddComponent<ConsoleComponent>();
            }
        }
    }
}