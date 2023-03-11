namespace ET {
    public enum MailboxType {
        MessageDispatcher,
        UnOrderMessageDispatcher,
        GateSession, // 这里主要是搞清楚：网关服的为什么要独立出来？
    }
}