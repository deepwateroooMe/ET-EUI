using System;
using System.Text.RegularExpressions;
namespace ET {
    [FriendClassAttribute(typeof(ET.Account))]
    public class C2A_LoginAccountHandler : AMRpcHandler<C2A_LoginAccount, A2C_LoginAccount> {
        // 也错的是，在这个方法里面，那么，把这里面的日志多打印一点儿出来
        
        protected override async ETTask Run(Session session, C2A_LoginAccount request, A2C_LoginAccount response, Action reply) {
            // TODO: 这里暂时不处理，等处理完客户端逻辑，再回来写
            // 有过服务器开发经验的程序员估计会发现我们的服务器处理逻辑有很多问题，对数据的验证还有session的使用过于草率。能跑，但是不安全            
            // 1. 判断请求的 SceneType( 进程 ) 是否为 Account. 若当前进程不是注册登录服，说明哪里出错了 
            if (session.DomainScene().SceneType != SceneType.Account) {
                Log.Error($" 请求到的 Scene 错误，当前场景 Scene 为： {session.DomainScene().SceneType}");
                session.Dispose();
                return;
            }
            // 2. 取消掉计时断开的组件（避免自动断开连接），代表连接通过了验证
            session.RemoveComponent<SessionAcceptTimeoutComponent>();
            // 通过判断是否加载了 SessionLockingComponent 来确定是否是多次消息
            Log.Error($"(session.GetComponent<SessionLockingComponent>() != null): {(session.GetComponent<SessionLockingComponent>() != null)}");
            if (session.GetComponent<SessionLockingComponent>() != null) {
                response.Error = ErrorCode.ERR_RequestRepeatedly; // 重复点击了，先前的请求正在处理中，后续点击不处理
                reply();
                session.Disconnect().Coroutine();
                return ;
            }
            // 3. 验证帐房密码的正确性。加上：不曾注册请先注册
            if (string.IsNullOrEmpty(request.AccountName) || string.IsNullOrEmpty(request.Password)) {
                // 帐户或是密码为空，断开连接
                response.Error = ErrorCode.ERR_LoginInfoError;
                reply();
                // session.Dispose(); // 改成是下面的： 
                session.Disconnect().Coroutine();
                return;
            }
            // // 这里的正则表达式：自己根本就没有看呀，那么极有可能就是说，我的用户名与密码是不符合它的要求的。也就是说，我现有的错误是从这下面的这个分支抛出来的
            // // 处理的办法很简单嘛：就是不管什么正则表达式，凡数据库里没有的，就自动注册，否则就登录呀
            // if (!Regex.IsMatch(request.AccountName.Trim(), @"^(?=.*[0-9].*)(?=.*[A-Z].*)(?=.*[a-z].*).{6,15}$")) {
            //     // 帐号不满足正则表达式
            //     response.Error = ErrorCode.ERR_LoginInfoError;
            //     Log.Error($" 用户名不符合正则表达式： response.Error: {response.Error}");
            //     reply();
            //     // session.Dispose();
            //     session.Disconnect().Coroutine();
            //     return;
            // }
            // if (!Regex.IsMatch(request.Password.Trim(), @"^(?=.*[0-9].*)(?=.*[A-Z].*)(?=.*[a-z].*).{6,15}$")) {
            //     // 密码不满足正则表达式
            //     response.Error = ErrorCode.ERR_LoginInfoError;
            //     Log.Error($" 密码不符合正则表达式： response.Error: {response.Error}");
            //     reply();
            //     // session.Dispose();
            //     session.Disconnect().Coroutine();
            //     return;
            // }
            // 4. 通过游戏服务器查询帐户是否存在：下面感觉奇怪，它说是个链表，就拿第一个？ 这里的逻辑是，即使是黑名单，也被保存在数据库，
            // 把后面的 异步逻辑块 用using关键字括起来。 using 关键定，使用完会自动释放组件
            using (session.AddComponent<SessionLockingComponent>()) {
                // 下面的 using: 解决两个客户端同时登录并且使用相同的账号和密码
                // 两条消息都会进入处理阶段，然后在数据库根据账号进行查询，都是为0，然后同时创建account账户，然后同时存入到数据库。
                //     那么，当他们下次登录的时候，同学A可能会去登录到同学B的账户里面，会造成数据错乱
                //     这就是数据库里面最重要的一个概念，就是唯一值，我们必须得保证账户是唯一的，才能避免这些情况的发生
                // 使用协程锁,锁的是异步逻辑,进入这个异步逻辑，就会锁上，直到执行完，才会解锁，让下个逻辑进来
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginAccount, request.AccountName.GetHashCode())) {
                    var accountInfoList = await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).
                        Query<Account>(d => d.AccountName.Equals(request.AccountName.Trim())); // List<T>: 同一个用户名可能注册了多个帐户？
                    Account account = null;
                    if (accountInfoList.Count > 0) { // 必须得有帐户存在吧
                        // 存在就对帐户信息进行校验
                        account = accountInfoList[0]; // 下面感觉奇怪，它说是个链表，就拿第一个？反正是个示例吧
//                         session.AddChild(account); // 感觉直接接在这个父物件的下面，也不是很好，而且后现还有两种返回的情况
                        if (account.AccountType == (int)AccountType.BlackList) { // 被列过黑名单
                            // 如果登录的黑名单类型的帐户，那么断开连接
                            response.Error = ErrorCode.ERR_LoginBlackListError;
                            reply();
                            // session.Dispose();
                            session.Disconnect().Coroutine(); 
                            // 对一些组件使用完及时释放： 比如我们查询数据库的临时实体变量，我们在不使用之后，就得进行判空然后dispose释放
                            account?.Dispose();
                            return;
                        }
                        // 然后校验登录的帐户的密码
                        if (!account.Password.Equals(request.Password)) {
                            response.Error = ErrorCode.ERR_LoginInfoError;
                            reply();
                            // session.Dispose();
                            session.Disconnect().Coroutine();
                            account?.Dispose();
                            return;
                        }
                        session.AddChild(account); // 感觉直接接在这个父物件的下面 // <<<<<<<<<<<<<<<<<<<<
                    } else { // 帐房在数据库中不存在，就自动注册
                        account = session.AddChild<Account>();
                        account.AccountName = request.AccountName.Trim();
                        account.Password = request.Password;
                        account.CreateTime = TimeHelper.ServerNow();
                        account.AccountType = (int)AccountType.General;
                        Log.Error($"account.AccountName: {account.AccountName}");
                        Log.Error($"account.Password 用户保存进数据库之前: {account.Password}");
                        // 从下面一句可以看到，接下来要处理的就是，MongoDB 数据库类的查询与保存等操作的封装与精简简化
                        await DBManagerComponent.Instance.GetZoneDB(session.DomainZone()).Save<Account>(account); // 异步保存进数据库
                    }
                    // 走到这一步就代表我们登录成功了
                    // 顶号操作：根据帐号 ID 拿到当前的 sessionInstanceID
                    long AccountSessionsInstanceId = session.DomainScene().GetComponent<AccountSessionsComponent>().Get(account.Id);
                    // 如果 session 存在，则表明已经上线了
                    Session otherSession = Game.EventSystem.Get(AccountSessionsInstanceId) as Session;
                    // 发消息给客户端，让它知道断开了
                    otherSession?.Send(new A2C_Disconnect() { Error = 0 });
                    // 断掉 Session
                    otherSession?.Disconnect().Coroutine();
                    // 之前登录的帐房已经断开了，被顶掉了，现在我们的 session 需要更新成新帐户的
                    session.DomainScene().GetComponent<AccountSessionsComponent>().Add(account.Id, session.InstanceId);

                    // 5. 创建通假令牌；这里无验证的令牌，生成的方法是生成随机数
                    string Token = TimeHelper.ServerNow().ToString() + RandomHelper.RandomNumber(int.MinValue, int.MaxValue).ToString();

                    // 6. 添加通讯的令牌
                    session.DomainScene().GetComponent<TokenComponent>().Remove(account.Id);
                    session.DomainScene().GetComponent<TokenComponent>().Add(account.Id, Token);
// 更新了：10 分钟计时算起
                    // 添加上计时器组件：十分钟自动销毁当前会话框
                    // 当客户端忽然掉线，服务器会一直保留先前的 session 会话框（这里的逻辑是，保留但是最多保留十分钟才对）；客户端再次登录后会创建新的会话框，并重新计时
                    // 如果会话框一直保留，会占用系统资源，服务器的内存会满，会崩溃
                    session.AddComponent<AccountCheckOutTimeComponent, long>(account.Id);

                    // 7. 返回消息；再跟进去看客户端的消息处理
                    response.AccountId = account.Id;
                    response.Token = Token;
                    reply();
                    await ETTask.CompletedTask;
                }
            }
        }
    }
}