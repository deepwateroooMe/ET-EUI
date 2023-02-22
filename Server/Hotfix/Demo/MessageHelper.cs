using System.Collections.Generic;

namespace ET {
    [FriendClass(typeof(UnitGateComponent))]
    public static class MessageHelper {
        public static void Broadcast(Unit unit, IActorMessage message) {
            Dictionary<long, AOIEntity> dict = unit.GetBeSeePlayers();
            foreach (AOIEntity u in dict.Values) {
                SendToClient(u.Unit, message);
            }
        }
        public static void SendToClient(Unit unit, IActorMessage message) {
            SendActor(unit.GetComponent<UnitGateComponent>().GateSessionActorId, message);
        }
        // 发送协议给ActorLocation
        public static void SendToLocationActor(long id, IActorLocationMessage message) {
            ActorLocationSenderComponent.Instance.Send(id, message);
        }
        // 发送协议给Actor
        public static void SendActor(long actorId, IActorMessage message) {
            ActorMessageSenderComponent.Instance.Send(actorId, message);
        }
        // 发送RPC协议给Actor
        // <param name="actorId">注册Actor的InstanceId</param>
        public static async ETTask<IActorResponse> CallActor(long actorId, IActorRequest message) {
            return await ActorMessageSenderComponent.Instance.Call(actorId, message);
        }
        // 发送RPC协议给ActorLocation
        // <param name="id">注册Actor的Id</param>
        public static async ETTask<IActorResponse> CallLocationActor(long id, IActorLocationRequest message) {
            return await ActorLocationSenderComponent.Instance.Call(id, message);
        }
    }
}