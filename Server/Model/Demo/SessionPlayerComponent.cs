namespace ET {
    [ComponentOf(typeof(Session))]
    public class SessionPlayerComponent : Entity, IAwake, IDestroy {
        public long PlayerId; // 会话框玩家组件，也就一个会话框，一个玩家。那么为什么需要这个一对一组件？
    }
}