namespace ET {
    [ActorMessageHandler]
    internal class C2M_TestActorMessageHandler : AMActorLocationHandler<Unit, C2M_TestActorLocationMessage> {
        protected override async ETTask Run(Unit unit, C2M_TestActorLocationMessage message) {
            // 先打印一下收到的消息内容
            Log.Warning(message.Content);
            // 通过 Unit, 就可以定位到需要将消息发向的客户端
            // 这也解释了为什么客户端往服务器发消息需要 Unit, 而服务器往客户端发消息不需要 Unit
            // 服务器接收到客户端的消息的时候，已经拿到了客户端的Unit
            // 发送一条到客户端的不需要返回消息的消息 IActorMessage
            MessageHelper.SendToClient(unit, new M2C_TestActorMessage() { Content = " 越快越好！！！"});
            await ETTask.CompletedTask;
        }
    }
}