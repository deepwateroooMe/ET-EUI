using System.Collections.Generic;

namespace ET {
    [ComponentOf]
    [FriendClass(typeof(ServerInfo))]
    public class ServerInfoManagerComponent : Entity, IAwake, IDestroy, ILoad {
        public List<ServerInfo> serverInfos = new List<ServerInfo>();
    }
}