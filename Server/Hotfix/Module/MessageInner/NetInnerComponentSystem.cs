using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
namespace ET { // 内网消息组件 
    [FriendClass(typeof(NetThreadComponent))]
    [ObjectSystem]
    public class NetInnerComponentAwakeSystem: AwakeSystem<NetInnerComponent, int> {
        public override void Awake(NetInnerComponent self, int sessionStreamDispatcherType) {
            NetInnerComponent.Instance = self;
            self.SessionStreamDispatcherType = sessionStreamDispatcherType;
// 内网消息：默认使用TService            
            self.Service = new TService(NetThreadComponent.Instance.ThreadSynchronizationContext, ServiceType.Inner);
            self.Service.ErrorCallback += self.OnError;
            self.Service.ReadCallback += self.OnRead;
            NetThreadComponent.Instance.Add(self.Service);
        }
    }
    [FriendClass(typeof(NetThreadComponent))]
    [ObjectSystem]
    public class NetInnerComponentAwake1System: AwakeSystem<NetInnerComponent, IPEndPoint, int> {
        public override void Awake(NetInnerComponent self, IPEndPoint address, int sessionStreamDispatcherType) {
            NetInnerComponent.Instance = self;
            self.SessionStreamDispatcherType = sessionStreamDispatcherType;
// 内网消息：默认使用TService            
            self.Service = new TService(NetThreadComponent.Instance.ThreadSynchronizationContext, address, ServiceType.Inner);
            self.Service.ErrorCallback += self.OnError;
            self.Service.ReadCallback += self.OnRead;
            self.Service.AcceptCallback += self.OnAccept;
            NetThreadComponent.Instance.Add(self.Service);
        }
    }
    [ObjectSystem]
    public class NetInnerComponentDestroySystem: DestroySystem<NetInnerComponent> {
        public override void Destroy(NetInnerComponent self) {
            NetThreadComponent.Instance.Remove(self.Service);
            self.Service.Destroy();
        }
    }

    [FriendClass(typeof(NetInnerComponent))]
    public static class NetInnerComponentSystem {
        public static void OnRead(this NetInnerComponent self, long channelId, MemoryStream memoryStream) {
            Session session = self.GetChild<Session>(channelId);
            if (session == null) {
                return;
            }
            session.LastRecvTime = TimeHelper.ClientNow();
            SessionStreamDispatcher.Instance.Dispatch(self.SessionStreamDispatcherType, session, memoryStream);
        }
        public static void OnError(this NetInnerComponent self, long channelId, int error) {
            Session session = self.GetChild<Session>(channelId);
            if (session == null) {
                return;
            }
            session.Error = error;
            session.Dispose();
        }
        // 这个channelId是由CreateAcceptChannelId生成的
        public static void OnAccept(this NetInnerComponent self, long channelId, IPEndPoint ipEndPoint) {
            Session session = self.AddChildWithId<Session, AService>(channelId, self.Service);
            session.RemoteAddress = ipEndPoint;
            // session.AddComponent<SessionIdleCheckerComponent, int, int, int>(NetThreadComponent.checkInteral, NetThreadComponent.recvMaxIdleTime, NetThreadComponent.sendMaxIdleTime);
        }
        // 这个channelId是由CreateConnectChannelId生成的
        public static Session Create(this NetInnerComponent self, IPEndPoint ipEndPoint) {
            uint localConn = self.Service.CreateRandomLocalConn();
            long channelId = self.Service.CreateConnectChannelId(localConn);
            Session session = self.CreateInner(channelId, ipEndPoint);
            return session;
        }
        private static Session CreateInner(this NetInnerComponent self, long channelId, IPEndPoint ipEndPoint) { // 建立内网消息的会话框：
            Session session = self.AddChildWithId<Session, AService>(channelId, self.Service);
            session.RemoteAddress = ipEndPoint; // 内网消息另一端：远程服务器地址
            self.Service.GetOrCreate(channelId, ipEndPoint); // 创建，或拿现存的【与每个远程，应该是有且仅有一个信道】与这个远程端口想要通信的信道
            // session.AddComponent<InnerPingComponent>();
            // session.AddComponent<SessionIdleCheckerComponent, int, int, int>(NetThreadComponent.checkInteral, NetThreadComponent.recvMaxIdleTime, NetThreadComponent.sendMaxIdleTime);
            // 创建好了与这个内网远程服务器的IP 地址与端口的信道，概念上就认为，与这个内网服务器的会话框准备好了，可用了
            return session;
        }
// 内网actor session，channelId是进程号
        public static Session Get(this NetInnerComponent self, long channelId) { // 调用时传进来的是：中间 18 位进程号，这里当作是 channelId
            Session session = self.GetChild<Session>(channelId); // 它永远是会需要：传进一个 id 的。因为实例不同物件之间以 id 相区分 
            if (session == null) { // 不存在，创建一个新的
                IPEndPoint ipEndPoint = StartProcessConfigCategory.Instance.Get((int) channelId).InnerIPPort; 
                session = self.CreateInner(channelId, ipEndPoint);
            }
            return session;
        }
    }
}