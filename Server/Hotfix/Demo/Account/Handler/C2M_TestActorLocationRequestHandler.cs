using System;

namespace ET {
    [ActorMessageHandler]
    public class C2M_TestActorLocationRequestHandler : AMActorLocationRpcHandler<Unit, C2M_TestActorLocationRequest, M2C_TestActorLocationResponse> {
        protected override async ETTask Run(Unit unit, C2M_TestActorLocationRequest request, M2C_TestActorLocationResponse response, Action reply) {
            // 先打印一下收到的消息内容
            Log.Warning(request.Content);
            // 设置回复消息的内容
            response.Content = " 我们结婚吧！！！";
            reply();
            await ETTask.CompletedTask;
        }
    }
}