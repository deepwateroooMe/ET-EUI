using System;
namespace ET {

    [ActorMessageHandler] // 感觉，这个回调方法，应该是由 Gate 的某个组件来执行：下面，定义它的执行逻辑
    public class R2G_GetLoginKeyHandler : AMActorRpcHandler<Scene, R2G_GetLoginKey, G2R_GetLoginKey> {

        protected override async ETTask Run(Scene scene, R2G_GetLoginKey request, G2R_GetLoginKey response, Action reply) {
            long key = RandomHelper.RandInt64(); // 随机生成了一串长整型数字
            scene.GetComponent<GateSessionKeyComponent>().Add(key, request.Account);
            // 把与为这个帐户所分配的认证Key, 与网关服的实例Id 返回去【有对方的实例 id, 持有有效认证，就可以与网关服通信呀】
            response.Key = key;
            response.GateId = scene.Id; // 所有实例，以实例 id 相区分；场景类携带有场景类型信息，为标明是网关服，当前网关所在的区等相关信息
            reply();
            await ETTask.CompletedTask;
        }
    }
}