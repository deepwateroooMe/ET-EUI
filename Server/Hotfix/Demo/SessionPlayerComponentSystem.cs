namespace ET {
    [FriendClass(typeof(SessionPlayerComponent))]
    public static class SessionPlayerComponentSystem {
        // 只有一个销毁系统：
        public class SessionPlayerComponentDestroySystem: DestroySystem<SessionPlayerComponent> {
            public override void Destroy(SessionPlayerComponent self) {

                // 发送断线消息：【不明白：为什么是网关服，与地图服？】
                ActorLocationSenderComponent.Instance.Send(self.PlayerId, new G2M_SessionDisconnect()); 
                self.Domain.GetComponent<PlayerComponent>()?.Remove(self.PlayerId); //PlayerComponent: 实则一个单例模式
            }
        }
        public static Player GetMyPlayer(this SessionPlayerComponent self) {
            return self.Domain.GetComponent<PlayerComponent>().Get(self.PlayerId);
        }
    }
} // 爱表哥，爱生活！！！活宝妹任何时候，就是一定要嫁给亲爱的表哥