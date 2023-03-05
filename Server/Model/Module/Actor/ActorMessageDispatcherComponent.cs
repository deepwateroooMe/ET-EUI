using System;
using System.Collections.Generic;
namespace ET {
    // Actor消息分发组件
    [ComponentOf(typeof(Scene))]
    public class ActorMessageDispatcherComponent: Entity, IAwake, IDestroy, ILoad {
        public static ActorMessageDispatcherComponent Instance;
        public readonly Dictionary<Type, IMActorHandler> ActorMessageHandlers = new Dictionary<Type, IMActorHandler>();
    }
}