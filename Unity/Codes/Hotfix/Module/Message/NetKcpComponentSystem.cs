using System;
using System.IO;
using System.Net;
namespace ET {
    [FriendClass(typeof(NetThreadComponent))]
    [ObjectSystem]
    public class NetKcpComponentAwakeSystem: AwakeSystem<NetKcpComponent, int> {
        public override void Awake(NetKcpComponent self, int sessionStreamDispatcherType) {
            self.SessionStreamDispatcherType = sessionStreamDispatcherType;
            
            self.Service = new TService(NetThreadComponent.Instance.ThreadSynchronizationContext, ServiceType.Outer);
            self.Service.ErrorCallback += (channelId, error) => self.OnError(channelId, error);
            self.Service.ReadCallback += (channelId, Memory) => self.OnRead(channelId, Memory);
            NetThreadComponent.Instance.Add(self.Service);
        }
    }
    [FriendClass(typeof(NetThreadComponent))]
    [ObjectSystem]
    public class NetKcpComponentAwake1System: AwakeSystem<NetKcpComponent, IPEndPoint, int> {
        public override void Awake(NetKcpComponent self, IPEndPoint address, int sessionStreamDispatcherType) {
            self.SessionStreamDispatcherType = sessionStreamDispatcherType;
            
            self.Service = new TService(NetThreadComponent.Instance.ThreadSynchronizationContext, address, ServiceType.Outer);
            self.Service.ErrorCallback += (channelId, error) => self.OnError(channelId, error);
            self.Service.ReadCallback += (channelId, Memory) => self.OnRead(channelId, Memory);
            self.Service.AcceptCallback += (channelId, IPAddress) => self.OnAccept(channelId, IPAddress);
            NetThreadComponent.Instance.Add(self.Service);
        }
    }
    [ObjectSystem]
    public class NetKcpComponentDestroySystem: DestroySystem<NetKcpComponent> {
        public override void Destroy(NetKcpComponent self) {
            NetThreadComponent.Instance.Remove(self.Service);
            self.Service.Destroy();
        }
    }
    [FriendClass(typeof(NetKcpComponent))]
    public static class NetKcpComponentSystem {
        private const string TAG = "NetKcpComponentSystem";

        public static void OnRead(this NetKcpComponent self, long channelId, MemoryStream memoryStream) {
            Session session = self.GetChild<Session>(channelId);
            if (session == null) {
                return;
            }
            session.LastRecvTime = TimeHelper.ClientNow();
// 这里仍然是没有问题的，还在后面出的错
            SessionStreamDispatcher.Instance.Dispatch(self.SessionStreamDispatcherType, session, memoryStream); // <<<<<<<<<<
            // 接下来是： SessionStreamDispatcherServerOuter.cs
        }
        public static void OnError(this NetKcpComponent self, long channelId, int error) {
            Session session = self.GetChild<Session>(channelId);
            if (session == null) {
                return;
            }
            session.Error = error;
            session.Dispose();
        }
        // 这个channelId是由CreateAcceptChannelId生成的
        public static void OnAccept(this NetKcpComponent self, long channelId, IPEndPoint ipEndPoint) {
            Session session = self.AddChildWithId<Session, AService>(channelId, self.Service);
            session.RemoteAddress = ipEndPoint;
            // 挂上这个组件，5秒就会删除session，所以客户端验证完成要删除这个组件。该组件的作用就是防止外挂一直连接不发消息也不进行权限验证
            session.AddComponent<SessionAcceptTimeoutComponent>(); // 接下来是在：注册登录之后，拿到与网关服的通信认证，客户端登录网关服的时候，会把这个组件去掉
            // 客户端连接，2秒检查一次recv消息，10秒没有消息则断开【这是，传说中的心跳包吗？】
            session.AddComponent<SessionIdleCheckerComponent, int>(NetThreadComponent.checkInteral);
        }
        public static Session Get(this NetKcpComponent self, long id) {
            Session session = self.GetChild<Session>(id);
            return session;
        }
        public static Session Create(this NetKcpComponent self, IPEndPoint realIPEndPoint) {
            long channelId = RandomHelper.RandInt64();
            Session session = self.AddChildWithId<Session, AService>(channelId, self.Service);
            session.RemoteAddress = realIPEndPoint;
            session.AddComponent<SessionIdleCheckerComponent, int>(NetThreadComponent.checkInteral);
            
            self.Service.GetOrCreate(session.Id, realIPEndPoint);
            return session;
        }
    }
}