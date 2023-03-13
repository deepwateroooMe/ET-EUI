namespace ET {
    [FriendClass(typeof(SessionPlayerComponent))]
    public static class SessionPlayerComponentSystem {

        // 只有一个销毁系统：[这里是自己感觉奇怪的：因为Awake() 似乎总与 Destroy() 系统成对出现，这里只有一个销毁系统？ ]
        public class SessionPlayerComponentDestroySystem: DestroySystem<SessionPlayerComponent> {
            public override void Destroy(SessionPlayerComponent self) {

                // 发送断线消息：【为什么是网关服，与地图服？】客户端只与其被分配的网关服通信，不与地图服通信。当被顶号或是用户下线，这里做的目的是通知地图服，这个用户下线了？
                ActorLocationSenderComponent.Instance.Send(self.PlayerId, new G2M_SessionDisconnect()); // 这里应该是一个不需要返回消息的通知消息 IMessage 
                self.Domain.GetComponent<PlayerComponent>()?.Remove(self.PlayerId); //PlayerComponent: 实则一个单例模式
            }
        }
        public static Player GetMyPlayer(this SessionPlayerComponent self) {
            return self.Domain.GetComponent<PlayerComponent>().Get(self.PlayerId);
        }
    }
} // 爱表哥，爱生活！！！活宝妹任何时候，就是一定要嫁给亲爱的表哥