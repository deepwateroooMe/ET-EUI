using System;
using System.IO;
namespace ET {

    [FriendClass(typeof(SessionPlayerComponent))]
    [SessionStreamDispatcher(SessionStreamDispatcherType.SessionStreamDispatcherServerOuter)]
    public class SessionStreamDispatcherServerOuter: ISessionStreamDispatcher {
        private const string TAG = "SessionStreamDispatcherServerOuter";

        public void Dispatch(Session session, MemoryStream memoryStream) {
            ushort opcode = BitConverter.ToUInt16(memoryStream.GetBuffer(), Packet.KcpOpcodeIndex);  // 这里读的位置是对的: 10023 for C2R_Login
            Log.ILog.Debug(TAG + ": {0}", opcode); 

            Type type = OpcodeTypeComponent.Instance.GetType(opcode);
            Log.ILog.Debug(TAG + ": {0}", type.Name);

            object message = MessageSerializeHelper.DeserializeFrom(opcode, type, memoryStream); // 这里要反序列化，这里出错了, 它说这个反序列化的类型 C2R_Login出错了
            if (message is IResponse response) {
                session.OnRead(opcode, response);
                return;
            }
            OpcodeHelper.LogMsg(session.DomainZone(), opcode, message);
            DispatchAsync(session, opcode, message).Coroutine();
        }
        public async ETTask DispatchAsync(Session session, ushort opcode, object message) {
            // 根据消息接口判断是不是Actor消息，不同的接口做不同的处理
            switch (message) {
                case IActorLocationRequest actorLocationRequest: { // gate session收到actor rpc消息，先向actor 发送rpc请求，再将请求结果返回客户端
                    long unitId = session.GetComponent<SessionPlayerComponent>().PlayerId;
                    int rpcId = actorLocationRequest.RpcId; // 这里要保存客户端的rpcId
                    long instanceId = session.InstanceId;
                    IResponse response = await ActorLocationSenderComponent.Instance.Call(unitId, actorLocationRequest);
                    response.RpcId = rpcId;
                    // session可能已经断开了，所以这里需要判断
                    if (session.InstanceId == instanceId) {
                        session.Reply(response);
                    }
                    break;
                }
                case IActorLocationMessage actorLocationMessage: {
                    long unitId = session.GetComponent<SessionPlayerComponent>().PlayerId;
                    ActorLocationSenderComponent.Instance.Send(unitId, actorLocationMessage);
                    break;
                }
                case IActorRequest actorRequest: {  // 分发IActorRequest消息，目前没有用到，需要的自己添加
                    break;
                }
                case IActorMessage actorMessage: {  // 分发IActorMessage消息，目前没有用到，需要的自己添加
                    break;
                }
                default: {
                    // 非Actor消息
                    MessageDispatcherComponent.Instance.Handle(session, opcode, message);
                    break;
                }
            }
        }
    }
}