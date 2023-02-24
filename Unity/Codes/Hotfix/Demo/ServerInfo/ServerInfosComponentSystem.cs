namespace ET {
    [ObjectSystem]
    public class ServerInfosComponentDestroySystem : DestroySystem<ServerInfosComponent> {
        public override void Destroy(ServerInfosComponent self) {
            foreach (var serverInfo in self.ServerInfoList)
                serverInfo?.Dispose();
            self.ServerInfoList.Clear();
        }
    }
    [FriendClassAttribute(typeof(ET.ServerInfosComponent))]
    public static class ServerInfosComponentSystem {
        public static void Add(this ServerInfosComponent self, ServerInfo serverInfo) {
            self.ServerInfoList.Add(serverInfo);
        }
    }
}