using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
namespace ET {
    public struct ProcessActorId {
        public int Process;
        public long ActorId;
        public ProcessActorId(long actorId) {
            InstanceIdStruct instanceIdStruct = new InstanceIdStruct(actorId);
            this.Process = instanceIdStruct.Process; // 中间 18 位的信息
            instanceIdStruct.Process = Game.Options.Process; // 更新了
            this.ActorId = instanceIdStruct.ToLong(); // 再更新
        }
    }
    [ComponentOf(typeof(Scene))]
    [ChildType(typeof(Session))]
    public class NetInnerComponent: Entity, IAwake<IPEndPoint, int>, IAwake<int>, IDestroy {
        public AService Service;
        public static NetInnerComponent Instance;
        public int SessionStreamDispatcherType { get; set; }
    }
}