using System;

namespace ET
{
    public interface IMHandler
    {
        void Handle(Session session, object message);
        Type GetMessageType();

        Type GetResponseType();
		ETTask Run(Session session, M2C_TestActorMessage message);
	}
}