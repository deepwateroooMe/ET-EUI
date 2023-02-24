using System;

namespace ET {
    // LoginAccount处理函数有两个结果
    // 1.不存在该用户，那么我们应该将它记录到我们的LoginAccount的字典里面
    // 2.如果存在用户，那么就拿到它的Gate网关地址，向Gate网关发消息，让玩家下线
    [ActorMessageHandler]  // 下面的两个协议要自己补上 proto 里面的
    public class A2L_LoginAccountRequestHandler : AMActorRpcHandler<Scene, A2L_LoginAccountRequest, L2A_LoginAccountResponse> {
        protected override async ETTask Run(Scene scene, A2L_LoginAccountRequest request, L2A_LoginAccountResponse response, Action reply) {
            // 1. 拿到 accountId
            long accountId = request.AccountId;
            // 2. 开启协程锁；它把协程锁添加了一个类型，那么就需要去补上
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginCenterLock, accountId.GetHashCode())) {
                // 3. 如果在注册登录服不存在当前用户的信息，就表示可以登录（要是还没有注册呢？）
                if (!scene.GetComponent<LoginInfoRecordComponent>().IsExist(accountId)) { // 这个类没有，需要重新定义生成一下
                    // TODO: 添加用户到 LoginAccount 里面
                    response.Error = ErrorCode.ERR_Success;
                    reply();
                    return ;
                }
                // 4. 如果存在用户登录信息，就应该通知 gate 网关服，让当前先前登录的玩家下线（好像只是下线，顶号是它其它某个部分的逻辑，可以补在这里）
                int zone = scene.GetComponent<LoginInfoRecordComponent>().Get(accountId);
                StartSceneConfig gateConfig = RealmGateAddressHelper.GetGate(zone, accountId); // 相当于是，拿到这个区，这个帐户所胡的网关服地址的配置 
                // 5. 拿到 actor 的 instanceId 后，就可以发送消息
                var g2LDisconnnectGateUnit = (G2L_DisconnectGateUnit)await MessageHelper
                    .CallActor(gateConfig.InstanceId, new L2G_DisconnectGateUnit() { AccountId = accountId});
                // 6. 将是否下线成功的消息返回给客户端 
                response.Error = g2LDisconnnectGateUnit.Error; // 这里体现出：把正确结果 ErrorCode.ERR_Success 也纳入错误的书写上的方便 
                reply();
            }
            await ETTask.CompletedTask;
        }
    }   
}