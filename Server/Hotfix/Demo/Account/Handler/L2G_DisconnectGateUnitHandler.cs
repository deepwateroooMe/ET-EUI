namespace ET {
    // 需要 protobuf 工具来转换生成可被系统读取的 .cs 文件索引，现在找不到类
    public class L2G_DisconnectGateUnitHandler : AMActorRpcHandler<Scene, L2G_DisconnectGateUnit, G2L_DisconnectGateUnit> {

        protected override async ETTask Run(Scene scene, L2G_DisconnectGateUnit request, G2L_DisconnectGateUnit response, Action reply) {
            long accountId = request.AccountId;
            // 为什么要把协程锁与 accountId 相关联？与协程锁的释放时间相关。只有在锁在这个帐户上的操作完成后，才可以释放锁。密切相关
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.GateLoginLock, accountId.GetHashCode())) {
                // 拿到网关服 gate-scene 上面的 PlayerComponent 组件
                PlayerComponent playerComponent = scene.GetComponent<PlayerComponent>();
                Player gateUnit = playerComponent.Get(accountId);
                // 如果没有找到当前玩家
                if (gateUnit == null) {
                    reply();
                    return ;
                }
                // 如果网关服上找到当前玩家，就执行下线操作
                playerComponent.Remove(accountId);
                // 现在，暂时，直接 dispose 掉连接，然后客户端断线，比较粗暴
                gateUnit.Dispose();
                // TODO 完善下线操作
            }
            reply();
            await ETTask.CompletedTask;
        }
    }
}