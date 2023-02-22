namespace ET {
    [MessageHelper]
    public class M2C_TestActorMessageHandler : AMHandler<M2C_TestActorMessage> {
        Log.Debug(message.Content);
        await ETTask.CompletedTask;
    }
}