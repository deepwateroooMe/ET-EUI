﻿using System;
using System.IO;
namespace ET {
    [Timer(TimerType.ActorMessageSenderChecker)]
    public class ActorMessageSenderChecker: ATimer<ActorMessageSenderComponent> {
        public override void Run(ActorMessageSenderComponent self) {
            try {
                self.Check();
            }
            catch (Exception e) {
                Log.Error($"move timer error: {self.Id}\n{e}");
            }
        }
    }
    [ObjectSystem]
    public class ActorMessageSenderComponentAwakeSystem: AwakeSystem<ActorMessageSenderComponent> {
        public override void Awake(ActorMessageSenderComponent self) {
            ActorMessageSenderComponent.Instance = self;
            self.TimeoutCheckTimer = TimerComponent.Instance.NewRepeatedTimer(1000, TimerType.ActorMessageSenderChecker, self);
        }
    }
    [ObjectSystem]
    public class ActorMessageSenderComponentDestroySystem: DestroySystem<ActorMessageSenderComponent> {
        public override void Destroy(ActorMessageSenderComponent self) {
            ActorMessageSenderComponent.Instance = null;
            TimerComponent.Instance.Remove(ref self.TimeoutCheckTimer);
            self.TimeoutCheckTimer = 0;
            self.TimeoutActorMessageSenders.Clear();
        }
    }
    [FriendClass(typeof(ActorMessageSenderComponent))]
    public static class ActorMessageSenderComponentSystem {
        public static void Run(ActorMessageSender self, IActorResponse response) {
            if (response.Error == ErrorCore.ERR_ActorTimeout) {
                self.Tcs.SetException(new Exception($"Rpc error: request, 注意Actor消息超时，请注意查看是否死锁或者没有reply: actorId: {self.ActorId} {self.MemoryStream.ToActorMessage()}, response: {response}"));
                return;
            }
            if (self.NeedException && ErrorCore.IsRpcNeedThrowException(response.Error)) {
                self.Tcs.SetException(new Exception($"Rpc error: actorId: {self.ActorId} request: {self.MemoryStream.ToActorMessage()}, response: {response}"));
                return;
            }
            self.Tcs.SetResult(response);
        }

        public static void Check(this ActorMessageSenderComponent self) {
            long timeNow = TimeHelper.ServerNow();
            foreach ((int key, ActorMessageSender value) in self.requestCallback) {
                if (timeNow < value.CreateTime + ActorMessageSenderComponent.TIMEOUT_TIME) { // 因为是顺序发送的，所以，检测到第一个不超时的就退出
                    break;
                }
                self.TimeoutActorMessageSenders.Add(key);
            } 
            foreach (int rpcId in self.TimeoutActorMessageSenders) {
                ActorMessageSender actorMessageSender = self.requestCallback[rpcId];
                self.requestCallback.Remove(rpcId);
                try {
                    IActorResponse response = ActorHelper.CreateResponse((IActorRequest)actorMessageSender.MemoryStream.ToActorMessage(), ErrorCore.ERR_ActorTimeout);
                    Run(actorMessageSender, response);
                }
                catch (Exception e) {
                    Log.Error(e.ToString());
                }
            }
            self.TimeoutActorMessageSenders.Clear();
        }
        public static void Send(this ActorMessageSenderComponent self, long actorId, IMessage message) {
            if (actorId == 0) {
                throw new Exception($"actor id is 0: {message}");
            }
            ProcessActorId processActorId = new ProcessActorId(actorId); // 这里是一层封装
            Session session = NetInnerComponent.Instance.Get(processActorId.Process); // 拿到，内网服务器之间，与这个目标服务器进程会话的会话框
            session.Send(processActorId.ActorId, message); // 用这个会话框发消息
        }
        public static void Send(this ActorMessageSenderComponent self, long actorId, MemoryStream memoryStream) {
            if (actorId == 0) {
                throw new Exception($"actor id is 0: {memoryStream.ToActorMessage()}");
            }
            ProcessActorId processActorId = new ProcessActorId(actorId); // 清清楚楚：要给对方 actorId 发消息，那根据这个参数，转换拿到它发在的服务器地址
            Session session = NetInnerComponent.Instance.Get(processActorId.Process);
            session.Send(processActorId.ActorId, memoryStream);
        }
        public static int GetRpcId(this ActorMessageSenderComponent self) {
            return ++self.RpcId; // 这个也是，自增变量的吗？
        }
        public static async ETTask<IActorResponse> Call(this ActorMessageSenderComponent self, long actorId, IActorRequest request, bool needException = true) {
            request.RpcId = self.GetRpcId();
            if (actorId == 0) {
                throw new Exception($"actor id is 0: {request}");
            }
            (ushort _, MemoryStream stream) = MessageSerializeHelper.MessageToStream(request); // 又回到，它说，从内存流上发消息
            return await self.Call(actorId, request.RpcId, stream, needException);
        }
        public static async ETTask<IActorResponse> Call(
            this ActorMessageSenderComponent self,
            long actorId,
            int rpcId,
            MemoryStream memoryStream,
            bool needException = true
            ) {
            if (actorId == 0) {
                throw new Exception($"actor id is 0: {memoryStream.ToActorMessage()}");
            }
            var tcs = ETTask<IActorResponse>.Create(true);
            self.requestCallback.Add(rpcId, new ActorMessageSender(actorId, memoryStream, tcs, needException));
            self.Send(actorId, memoryStream);
            long beginTime = TimeHelper.ServerFrameTime();
            IActorResponse response = await tcs;
            long endTime = TimeHelper.ServerFrameTime();
            long costTime = endTime - beginTime;
            if (costTime > 200) {
                Log.Warning("actor rpc time > 200: {0} {1}", costTime, memoryStream.ToActorMessage());
            }
            return response;
        }
        public static void RunMessage(this ActorMessageSenderComponent self, long actorId, IActorResponse response) {
            ActorMessageSender actorMessageSender;
            if (!self.requestCallback.TryGetValue(response.RpcId, out actorMessageSender)) {
                return;
            }
            self.requestCallback.Remove(response.RpcId);
            Run(actorMessageSender, response);
        }
    }
}