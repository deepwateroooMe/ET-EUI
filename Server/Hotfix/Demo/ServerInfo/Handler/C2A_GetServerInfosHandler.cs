using System;
namespace ET {
    [FriendClassAttribute(typeof(ET.ServerInfoManagerComponent))]
    internal class C2A_GetServerInfosHandler : AMRpcHandler<C2A_GetServerInfos, A2C_GetServerInfos> {
        protected override async ETTask Run(Session session, C2A_GetServerInfos request, A2C_GetServerInfos response, Action reply) {
            // 1. 判断当前请求的 Session 的类型
            if (session.DomainScene().SceneType != SceneType.Account) {
                Log.Error($" 请求的服务器 Scene 错误。当前的 Scene 为： {session.DomainScene().SceneType}");
                session?.Dispose();
                await ETTask.CompletedTask ;
                return;
            }
            // 2. 拿到当前 Session 用户的 Token
            string token = session.DomainScene().GetComponent<TokenComponent>().Get(request.AccountId);
            // 3. 请求的 Session 必须存在于当前服务器所保存的 Session 里，代表其已经登录。如果找不到，就是该 Session 用户没有登录
            if (token == null || token != request.Token) {
                response.Error = ErrorCode.ERR_TokenError;
                reply();
                session?.Disconnect().Coroutine();
				await ETTask.CompletedTask;
				return;
            }
            // 4. 遍历服务器的区服信息，返回给客户端【客户端需要权限拿这些东西吗？】
            foreach (var serverInfo in session.DomainScene().GetComponent<ServerInfoManagerComponent>().serverInfos) {
                response.ServerInfosList.Add(serverInfo.ToMessage());
            }
            reply();
            await ETTask.CompletedTask;
        }
    }
}