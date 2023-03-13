namespace ET {

    [ActorMessageHandler]
    public class G2M_SessionDisconnectHandler : AMActorLocationHandler<Unit, G2M_SessionDisconnect> {
        // 这里是什么也没有做
        protected override async ETTask Run(Unit unit, G2M_SessionDisconnect message) {
            await ETTask.CompletedTask;
        }
    }
}