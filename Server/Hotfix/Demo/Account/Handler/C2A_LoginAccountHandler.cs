using System;
using System.Text.RegularExpressions;
namespace ET {
    [FriendClassAttribute(typeof(ET.Account))]
    public class C2A_LoginAccountHandler : AMRpcHandler<C2A_LoginAccount, A2C_LoginAccount> {
        protected override async ETTask Run(Session session, C2A_LoginAccount request, A2C_LoginAccount response, Action reply) {
            // TODO: 这里暂时不处理，等处理完客户端逻辑，再回来写
            // 有过服务器开发经验的程序员估计会发现我们的服务器处理逻辑有很多问题，对数据的验证还有session的使用过于草率。能跑，但是不安全            
            // 1. 判断请求的 SceneType( 进程 ) 是否为 Account. 若当前进程不是注册登录服，说明哪里出错了 
            if (session.DomainScene().SceneType != SceneType.Account) {
                Log.Error($" 请求到的 Scene 错误，当前场景 Scene 为： {session.DomainScene().SceneType}");
                session.Dispose();
                return;
            }
            // 2. 制裁掉计时断开的组件（避免自动断开连接），代表连接通过了验证
            session.RemoveComponent<SessionAcceptTimeoutComponent>();
            if (session.GetComponent<SessionLockingComponent>() != null) {
                response.Error = ErrorCode.ERR_RequestRepeatedly;
                reply();
                session.Disconnect().Coroutine();
                return ;
            }
            // 3. 验证帐房密码的正确性。加上：不曾注册请先注册
            if (string.IsNullOrEmpty(request.AccountName) || string.IsNullOrEmpty(request.Password)) {
                // 帐户或是密码为空，断开连接
                response.Error = ErrorCode.ERR_LoginInfoError;
                reply();
                // session.Dispose();
                session.Disconnect().Coroutine();
                return;
            }
            if (!Regex.IsMatch(request.AccountName.Trim(), @"^(?=.*[0-9].*)(?=.*[A-Z].*)(?=.*[a-z].*).{6,15}$")) {
                // 帐号不满足正则表达式
                response.Error = ErrorCode.ERR_LoginInfoError;
                reply();
                session.Dispose();
                return;
            }
            if (!Regex.IsMatch(request.Password.Trim(), @"^(?=.*[0-9].*)(?=.*[A-Z].*)(?=.*[a-z].*).{6,15}$")) {
                // 密码不满足正则表达式
                response.Error = ErrorCode.ERR_LoginInfoError;
                reply();
                session.Dispose();
                return;
            }
            // 4. 通过游戏服务器查询帐户是否存在：下面感觉奇怪，它说是个链表，就拿第一个？
            using (session.AddComponent<SessionLockingComponent>()) {
                var accountInfoList = await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).
                    Query<Account>(d => d.AccountName.Equals(request.AccountName.Trim()));
                Account account = null;
                if (accountInfoList.Count > 0) { // 必须得有帐户存在吧
                    account = accountInfoList[0]; // 下面感觉奇怪，它说是个链表，就拿第一个？
                    session.AddChild(account);
                    if (account.AccountType == (int)AccountType.BlackList) { // 被列过黑名单 
                        response.Error = ErrorCode.ERR_LoginBlackListError;
                        reply();
                        session.Dispose();
                        return;
                    }
                    // 瑞检查密码
                    if (!account.Password.Equals(request.Password)) {
                        response.Error = ErrorCode.ERR_LoginInfoError;
                        reply();
                        session.Dispose();
                        return;
                    }
                }
                else { // 帐房在数据库中不存在，就自动注册
                    account = session.AddChild<Account>();
                    account.AccountName = request.AccountName.Trim();
                    account.Password = request.Password;
                    account.CreateTime = TimeHelper.ServerNow();
                    account.AccountType = (int)AccountType.General;
                    // 从下面一句可以看到，接下来要处理的就是，MongoDB 数据库类的查询与保存等操作的封装与精简简化
                    await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).Save<Account>(account);
                }
                // 走到这一步就代表我们登录成功了
                // 顶号操作
                // 根据帐号 ID 拿到当前的 sessionInstanceID
                long AccountSessionsInstanceId = session.DomainScene().GetComponent<AccountSessionsComponent>().Get(account.Id);
                // 如果 session 存在，则表明已经上线了
                Session otherSession = Game.EventSystem.Get(AccountSessionsInstanceId) as Session;
                // 发消息给客户端，让它知道断开了
                otherSession?.Send(new A2C_Disconnect() { Error = 0 });
                // 断掉 Session
                otherSession?.Disconnect().Coroutine();
                // 之前登录的帐房已经断开了，被顶掉了，现在我们的 session 需要更新成新帐户的
                session.DomainScene().GetComponent<AccountSessionsComponent>().Add(account.Id, session.instanceId);
            }
            // 5. 创建通假令牌；这里无验证的令牌，生成的方法是生成随机数
            string Token = TimeHelper.ServerNow().ToString() + RandomHelper.RandomNumber(int.MinValue, int.MaxValue).ToString();
            // 6. 添加通讯的令牌
            session.DomainScene().GetComponent<TokenComponent>().Remove(Account.Id);
            session.DomainScene().GetComponent<TokenComponent>().Add(Account.Id, Token); // 更新了：10 分钟计时算起
                                                                                         // 7. 返回消息
            response.AccountId = account.Id;
            response.Token = Token;
            reply();
            await ETTask.CompletedTask;
        }
    }
}