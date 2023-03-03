namespace ET {
     // 这里是登录完成之后的逻辑，我需要写的是进入热更新程序域里去
    public class LoginFinish_CreateLobbyUI: AEventAsync<EventType.LoginFinish> {

        protected override async ETTask Run(EventType.LoginFinish args) {
            args.ZoneScene.GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Login);
            await args.ZoneScene.GetComponent<UIComponent>().ShowWindowAsync(WindowID.WindowID_Lobby);// 可以保留游戏大厅，现进入游戏改为 tetris3D
        }
    }
}