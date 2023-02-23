namespace ET {
    [ComponentOf]
    public class AccountInfoComponent : Entity, IAwake, IDestroy {
        public string Token;   // 令牌
        public long AccountId; // 帐户ID 
    }
}