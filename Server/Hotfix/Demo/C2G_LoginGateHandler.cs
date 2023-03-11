using System;
namespace ET {
    [MessageHandler]
    [FriendClass(typeof(SessionPlayerComponent))]
    public class C2G_LoginGateHandler : AMRpcHandler<C2G_LoginGate, G2C_LoginGate> {

        protected override async ETTask Run(Session session, C2G_LoginGate request, G2C_LoginGate response, Action reply) {
            Scene scene = session.DomainScene();
            // 为注册或是登录时要过认证的帐户，读取认证信息：
            string account = scene.GetComponent<GateSessionKeyComponent>().Get(request.Key);
            if (account == null) { // 认证有有效期，过了分配认证时算起的 20 秒，就过期无期，会被自动从字典里移除，后，再读，就是 null
                response.Error = ErrorCore.ERR_ConnectGateKeyError;
                response.Message = "Gate key验证失败!";
                reply(); // 为什么总带一个 reply 呢？
                return;
            }
            session.RemoveComponent<SessionAcceptTimeoutComponent>(); // 什么时候加上这个的？NetKcpComponentSystem.cs OnAccept(...)
            PlayerComponent playerComponent = scene.GetComponent<PlayerComponent>();
            Player player = playerComponent.AddChild<Player, string>(account);
            playerComponent.Add(player);
            session.AddComponent<SessionPlayerComponent>().PlayerId = player.Id; // 把会话框与玩家，绑定在一起
            session.AddComponent<MailBoxComponent, MailboxType>(MailboxType.GateSession);
            response.PlayerId = player.Id;
            reply();
            await ETTask.CompletedTask;
        }
    }
}