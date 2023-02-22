using System;
using UnityEngine;
namespace ET {
    // 发送消息给服务端Map: 将Unit对象传送到Map游戏服务器上面的处理逻辑
    [ActorMessageHandler]
    public class M2M_UnitTransferRequestHandler : AMActorRpcHandler<Scene, M2M_UnitTransferRequest, M2M_UnitTransferResponse> {
        // 1.我们拿到Gate网关发来的Unit（在14节介绍过，新的Demo登录流程的Unit映射关系创建是在Gate网关创建的）
        // 2.我们通知客户端进行场景切换，并且创建客户端的新的客户端场景以及 Unit
        // 3.我们创建客户端的 Unit: 客户端的Unit在哪里创建的呢？在切换场景的时候，SceneChangeHelper创建新客户端场景的同时，进行Unit创建
        protected override async ETTask Run(Scene scene, M2M_UnitTransferRequest request, M2M_UnitTransferResponse response, Action reply) {
            await ETTask.CompletedTask;
            UnitComponent unitComponent = scene.GetComponent<UnitComponent>();
// 1.我们拿到Gate网关发来的Unit（在14节介绍过，新的Demo登录流程的Unit映射关系创建是在Gate网关创建的）
            Unit unit = request.Unit; 
            
            unitComponent.AddChild(unit); // 想不明白：怎么又加自己身上，又加到作子控件？这里不知道是在干什么
            unitComponent.Add(unit);
            foreach (Entity entity in request.Entitys) {
                unit.AddComponent(entity);
            }
            
            unit.AddComponent<MoveComponent>();
            unit.AddComponent<PathfindingComponent, string>(scene.Name);
            unit.Position = new Vector3(-10, 0, -10);
            
            unit.AddComponent<MailBoxComponent>();
            
            // 通知客户端创建My Unit: 2.我们通知客户端进行场景切换，并且创建客户端的新的客户端场景以及 Unit
            M2C_CreateMyUnit m2CCreateUnits = new M2C_CreateMyUnit();
            m2CCreateUnits.Unit = UnitHelper.CreateUnitInfo(unit);
            MessageHelper.SendToClient(unit, m2CCreateUnits);
            
            // 加入aoi
            unit.AddComponent<AOIEntity, int, Vector3>(9 * 1000, unit.Position);
            response.NewInstanceId = unit.InstanceId;
            
            reply();
        }
    }
}