namespace ET {
    public static class TokenComponentSystem {
        // public readonly Dictionary<long, string> TokenDistionary = new Dictionary<long, string>();

        public static void Add(this TokenDistionary self, long key, string token) {
            self.TokenDistionary.Add(key, token);
            self.TimeOutRemoveKey(key, token).Coroutine();
        }
        public static void Get(this TokenDistionary self, long key) {
            string value = null;
            self.TokenDistionary.TryGetValue(key, out value);
            return value;
        }
        public static void Remove(this TokenDistionary self, long key) {
            if (self.TokenDistionary.ContainsKey(key))
                self.TokenDistionary.Remove(key);
        }
        public static async ETTask TimeOutRemoveKey(this TokenComponent self, long key, string token) {
            await TimerComponent.Instance.WaitAsync(600000); // 十分钟，就翻译 token
            string onlineToken = self.Get(key);
            if (!string.IsNullOrEmpty(onlineToken) && onlineToken == token)
                self.Remove(key);
        }        
    }
}