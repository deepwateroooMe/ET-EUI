namespace ET {
    [FriendClass(typeof(GateSessionKeyComponent))]
    public static class GateSessionKeyComponentSystem {

        public static void Add(this GateSessionKeyComponent self, long key, string account) {
            self.sessionKey.Add(key, account);
            self.TimeoutRemoveKey(key).Coroutine();
        }
        public static string Get(this GateSessionKeyComponent self, long key) {
            string account = null;
            self.sessionKey.TryGetValue(key, out account);
            return account;
        }
        public static void Remove(this GateSessionKeyComponent self, long key) {
            self.sessionKey.Remove(key);
        }
        private static async ETTask TimeoutRemoveKey(this GateSessionKeyComponent self, long key) {
            await TimerComponent.Instance.WaitAsync(20000); // 每个与客户端的有效会话框，有效时间为 20 秒，之后Key 这个有效认证会自动作废。【逻辑在其它地方】会话框自动移除回收
            self.sessionKey.Remove(key);
        }
    }
}