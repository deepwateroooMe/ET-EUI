using System;
using System.Net;
namespace ET {
// 这是服务器端：处理客户端登录请求的逻辑
    [MessageHandler]
    public class C2R_LoginHandler : AMRpcHandler<C2R_Login, R2C_Login> {
        protected override async ETTask Run(Session session, C2R_Login request, R2C_Login response, Action reply) {
            // 随机分配一个Gate: 拿到这个远程网关服的，服务器配置
            StartSceneConfig config = RealmGateAddressHelper.GetGateRandomly(session.DomainZone()); // 那个想要返回固定网关服的，方法以及用的地方，找出来再看下
            Log.Debug($"gate address: {MongoHelper.ToJson(config)}");
            // 向gate请求一个key,客户端可以拿着这个key连接gate 【这里也是前面提到过的，知道了对方的 InstanceId, 就可以给对方发消息，这里是Realm 与网关服的内网消息】
            G2R_GetLoginKey g2RGetLoginKey = (G2R_GetLoginKey) await ActorMessageSenderComponent.Instance.Call(
                config.InstanceId, new R2G_GetLoginKey() {Account = request.Account}); // 这里要去，找哪里的处理逻辑？想要知道这里，Gate 是如何处理相关逻辑的？

// 这个，下午看的，感觉脑袋没有早上那么清醒了，内网外网IP 地址，内网外网端口，没有看得狠懂，改天早上再跟进去看一下
            // 地址：应该是，服务器处理后，分配给这个客户端的网关服的，对外连接端口，供客户端接下来，可以与网关服直接发送交换消息 
            response.Address = config.OuterIPPort.ToString();  // 内网外网IP 地址，内网外网端口，没有看得狠懂，改天早上再跟进去看一下
            response.Key = g2RGetLoginKey.Key; // 返回去给客户端用的：可以被其网关服认证的Key 
            response.GateId = g2RGetLoginKey.GateId; // 不明白这个GateId, 是什么意思 
            reply();
        }
    }
} 