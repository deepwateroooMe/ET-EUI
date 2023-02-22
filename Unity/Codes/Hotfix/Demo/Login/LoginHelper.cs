using NLog.Fluent;
using System;
namespace ET {
    
// 因为添加了网络出错的异常常数，可以加点儿逻辑
    // 我们在登录的逻辑层修改登录的逻辑
    // 我们通过ErrorCode来告诉显示层，调用是否成功
    // 当显示层调用Login这个逻辑层代码的时候，可以通过拿到的返回值来知道是否登录成功
    public static class LoginHelper {

        public static async ETTask<int> Login(Scene zoneScene, string address, string account, string password) {
            // try { // 【源码】：把捕获异常永远封装在最底层最外面
                A2C_LoginAccount a2C_LoginAccount = null;
                Session accountSession = null;
                // // 创建一个ETModel层的Session 【这是原码】
                // R2C_Login r2CLogin;
                // Session session = null;
                try { // 从 zoneScene 上面获取 NetKcpComponent 的 IP 地址，通过这个来创建一个新的会话框 
                    accountSession = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(address));
                    a2C_LoginAccount = (A2C_LoginAccount) await accountSession.Call(
                        new C2A_LoginAccount() {AccountName = account, Password = password});
                    // session = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(address));
                    // r2CLogin = (R2C_Login) await session.Call(new C2R_Login() { Account = account, Password = password });
                } catch (Exception e) { // 因为现在定义了网络异常，就可以捕获了
                    accountSession?.Dispose();  // 如果出错，就关掉或是回收
                    Log.Error(e.ToString());
                    return ErrorCode.ERR_NetWorkError; // 返回错误码，提示登录失败
                }
                // finally {
                //     session?.Dispose();
                // }
                if (a2C_LoginAccount.Error != ErrorCode.ERR_Success) {
                    accountSession?.Dispose();
                    return a2C_LoginAccount.Error; // 返回错误。与上面的不同，非网络异常
                }
                // // 创建一个gate Session,并且保存到SessionComponent中
                // Session gateSession = zoneScene.GetComponent<NetKcpComponent>().Create(NetworkHelper.ToIPEndPoint(r2CLogin.Address));
                // gateSession.AddComponent<PingComponent>();
                // zoneScene.AddComponent<SessionComponent>().Session = gateSession;
                // G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await gateSession.Call(
                //     new C2G_LoginGate() { Key = r2CLogin.Key, GateId = r2CLogin.GateId});
                // Log.Debug("登陆gate成功!");
                // Game.EventSystem.PublishAsync(new EventType.LoginFinish() {ZoneScene = zoneScene}).Coroutine();

                // 如果成功，把 session 保留给 zoneScene, 作为通讯接口。【这些，就明显看见，服务器的格局小了很多】
                zoneScene.AddComponent<SessionComponent>().Session = accountSession;
                // 记录拿到的帐房信息
                zoneScene.GetComponent<AccountInfoComponent>().Token = a2C_LoginAccount.Token;
                zoneScene.GetComponent<AccountInfoComponent>().AccountId = a2C_LoginAccount.AccountId;
                
                return ErrorCode.ERR_Success;
            // }
            // catch (Exception e) {
            //     Log.Error(e);
            // }
        } 
    }
}