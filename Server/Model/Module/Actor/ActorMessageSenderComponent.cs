using System.Collections.Generic;
namespace ET {
    [ComponentOf(typeof(Scene))]
    public class ActorMessageSenderComponent: Entity, IAwake, IDestroy { // 感觉这个部分，还需要再多花点儿时间再仔细理解一下，关于这里的时间机超时机制的
        public static long TIMEOUT_TIME = 40 * 1000; // 40 秒
        public static ActorMessageSenderComponent Instance { get; set; } // 单例
        public int RpcId;
// 这里为什么，要使用有序字典 ? 这个字典 ?
        public readonly SortedDictionary<int, ActorMessageSender> requestCallback = new SortedDictionary<int, ActorMessageSender>();
        public long TimeoutCheckTimer;
        public List<int> TimeoutActorMessageSenders = new List<int>(); // ？还没弄懂
    }
}