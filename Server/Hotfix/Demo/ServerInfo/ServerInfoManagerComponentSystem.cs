namespace ET {
    public static class ServerInfoManagerComponentAwakeSystem : AwakeSystem<ServerInfoManagerComponent> {
        public override void Awake(ServerInfoManagerComponent self) {
            self.Awake().Coroutine();
        }
    }
    public static class ServerInfoManagerComponentDestroySystem : DestroySystem<ServerInfoManagerComponent> {
        public override void Destroy(ServerInfoManagerComponent self) {
            foreach (var serverInfo in self.serverInfos)
                serverInfo?.Dispose(); // 每个回收
            self.serverInfos.Clear();  // 链表清空 
        }
    }
    public static class ServerInfoManagerComponentLoadSystem : LoadSystem<ServerInfoManagerComponent> {
        public override void Load(ServerInfoManagerComponent self) {
            self.Awake().Coroutine(); // 这里，这个东西不会调用两遍吗？不是一遍已经可以了吗？
        }
    }
    public static class ServerInfoManagerComponentSystem {
        public static async ETTask Awake(this ServerInfoManagerComponent self) {
            // 组件的 awake() 时间就是，从数据库获取房间信息，然后存储到List 里面
            var serverInfoList = await DBManagerComponent.Instance.GetZoneDB(self.DomainZone()).Query<ServerInfo>(d => true);
            if (serverInfoList == null || serverInfoList.Count <= 0) {
                Log.Error("serverInfo count is zero");
                return ;
            }
            self.serverInfos.Clear();
            foreach (var serverInfo in serverInfoList) {
                self.AddChild(serverInfo);
                self.serverInfos.Add(serverInfo);
            }
            await ETTask.CompletedTask;
        }
    }
}