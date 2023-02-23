namespace ET {
    [ObjectSystem]
    public class AccountSessionsComponentAwakeSystem : AwakeSystem<AccountSessionsComponent> {
        public override void Awake(AccountSessionsComponent self) {
            self.AccountSessionDictionary.Clear();
        }
    }
    // 这里有点儿奇怪：只有销毁，都没有创建，哪里来的销毁？
    public class AccountSessionsComponentDestroySystem : DestroySystem<AccountSessionsComponent> {
        public override void Destroy(AccountSessionsComponent self) {
            self.AccountSessionDictionary.Clear();
        }
    }
    [FriendClassAttribute(typeof(ET.AccountSessionsComponent))]
    public static class AccountSessionsComponentSystem {
        public static long Get(this AccountSessionsComponent self, long accountId) {
            if (!self.AccountSessionDictionary.TryGetValue(accountId, out long instanceId))
                return 0;
            return instanceId;
        }
        public static void Add(this AccountSessionsComponent self, long accountId, long instanceId) {
            if (self.AccountSessionDictionary.ContainsKey(accountId)) {
                self.AccountSessionDictionary[accountId] = instanceId;
                return;
            }
            self.AccountSessionDictionary.Add(accountId, instanceId);
        }
        public static void Remove(this AccountSessionsComponent self, long accountId) {
            if (self.AccountSessionDictionary.ContainsKey(accountId))
                self.AccountSessionDictionary.Remove(accountId);
        }
    }
}