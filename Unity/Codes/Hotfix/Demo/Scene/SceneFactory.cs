namespace ET {
    public static class SceneFactory {

        public static Scene CreateZoneScene(int zone, string name, Entity parent) {
            Scene zoneScene = EntitySceneFactory.CreateScene(Game.IdGenerater.GenerateInstanceId(), zone, SceneType.Zone, name, parent);
            zoneScene.AddComponent<ZoneSceneFlagComponent>();
            zoneScene.AddComponent<NetKcpComponent, int>(SessionStreamDispatcherType.SessionStreamDispatcherClientOuter);
            zoneScene.AddComponent<CurrentScenesComponent>();
            zoneScene.AddComponent<ObjectWait>();
            zoneScene.AddComponent<PlayerComponent>();

// 添加自定义的组件，用来保存登录帐房的信息: 可是这里，仍然是还没有能够缓存保存数据库呀？
            zoneScene.AddComponent<AccountInfoComponent>();
            // 添加自定义的组件：存储房间列表
            zoneScene.AddComponent<ServerInfosComponent>();
            Game.EventSystem.Publish(new EventType.AfterCreateZoneScene() {ZoneScene = zoneScene});
            return zoneScene;
        }
        public static Scene CreateCurrentScene(long id, int zone, string name, CurrentScenesComponent currentScenesComponent) {
            Scene currentScene = EntitySceneFactory.CreateScene(id, IdGenerater.Instance.GenerateInstanceId(), zone, SceneType.Current, name, currentScenesComponent);
            currentScenesComponent.Scene = currentScene; // 将新创建的场景，加载在这个组件下
            Game.EventSystem.Publish(new EventType.AfterCreateCurrentScene() {CurrentScene = currentScene}); // 触发场景创建之后的相关回调
            return currentScene;
        }
    }
}