namespace ET {
    // 这里是客户端的ServerInfo System: 所以需要处理的
    // 1.将服务器端发来的消息中的结构体ServerInfoProto进行解析和保存
    // 2.将客户端本地的ServerInfo转换为ServerInfoProto
	[FriendClassAttribute(typeof(ET.ServerInfo))]
	public static class ServerInfosSystem {
        public static void FromMessage(this ServerInfo self, ServerInfoProto serverInfoProto) {
            self.Id = serverInfoProto.Id;
            self.Status = serverInfoProto.Status;
            self.ServerName = serverInfoProto.ServerName;
        }
        public static ServerInfoProto ToMessage(this ServerInfo self) {
            return new ServerInfoProto() { Id = (int)self.Id, ServerName = self.ServerName, Status = self.Status};
        }
    }
}