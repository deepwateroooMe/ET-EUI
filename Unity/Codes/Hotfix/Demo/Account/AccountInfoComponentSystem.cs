namespace ET {
    [ObjectSystem]
    public class AccountInfoComponentAwakeSystem : AwakeSystem<AccountInfoComponent> {
        public override void Awake(AccountInfoComponent self) {
            self.Token = null;
            self.AccountId = 0;
        }
    }
    public class AccountInfoComponentDestroySystem : DestroySystem<AccountInfoComponent> {
        public override void Destroy(AccountInfoComponent self) {
            self.Token = null;
            self.AccountId = 0;
        }
    }
    [FriendClassAttribute(typeof(AccountInfoComponent))]
    public static class AccountInfoComponentSystem {
        //... 爱表哥，爱生活！！！
    }
}