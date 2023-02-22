namespace ET {

    public static class DisconnectHelper {

        public static async ETTask Disconnect(this Session self) {
            if (self == null || self.IsDisposed)
                return ;
            // 提前捕获 instanceId
            long instanceId = self.InstanceId;
            await TimerComponent.Instance.WaitAsync(1000); // 等待1 秒
            // 如果此时 session 的 instanceId 和1 秒之前不一样
            // 那么就代表已经被释放和重建了，如果继续 Dispose() 就会出现逻辑错误
            // 释放 Session 会将 instanceId 重置为 0
            if (self.InstanceId != instanceId)
                return ;
            self.Dispose(); // 安全释放 
        }
    }
}