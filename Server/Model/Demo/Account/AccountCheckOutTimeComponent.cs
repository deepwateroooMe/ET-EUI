namespace ET {
    
// 十分钟等待，如果玩家还没有进行下一步操作，我们就断开Session，避免客户端一直占用Account服务器
    public class AccountCheckOutTimeComponent : Entity IAwake<long>, IDestroy {
        public long Timer = 0;
        public long AccountId;
    }