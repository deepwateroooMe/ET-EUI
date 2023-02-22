namespace ET {
    public static class SceneChangeHelper {
        // 场景切换协程
        // 3.我们创建客户端的 Unit: 客户端的Unit在哪里创建的呢？在切换场景的时候，SceneChangeHelper创建新客户端场景的同时，进行Unit创建
        public static async ETTask SceneChangeTo(Scene zoneScene, string sceneName, long sceneInstanceId) {
            zoneScene.RemoveComponent<AIComponent>();
            
            CurrentScenesComponent currentScenesComponent = zoneScene.GetComponent<CurrentScenesComponent>();
            // 这里做的事是：更新这个 CurrentScenesComponent 组件的 Scene 成员，将以前的Scene 回收，创建新的 Scene 赋值给这个组件
            currentScenesComponent.Scene?.Dispose(); // 删除之前的CurrentScene，创建新的
            Scene currentScene = SceneFactory.CreateCurrentScene(sceneInstanceId, zoneScene.Zone, sceneName, currentScenesComponent); // 加在这个组件下
            UnitComponent unitComponent = currentScene.AddComponent<UnitComponent>(); // 为新场景添加了 Unit 组件
// 可以订阅这个事件中创建Loading界面
            Game.EventSystem.Publish(new EventType.SceneChangeStart() {ZoneScene = zoneScene});

            // 等待CreateMyUnit的消息： ObjectWait 是在大区【游戏客户端？】场景 SceneFactory 里面添加的
            WaitType.Wait_CreateMyUnit waitCreateMyUnit = await zoneScene.GetComponent<ObjectWait>().Wait<WaitType.Wait_CreateMyUnit>();
            M2C_CreateMyUnit m2CCreateMyUnit = waitCreateMyUnit.Message; // 2
            Unit unit = UnitFactory.Create(currentScene, m2CCreateMyUnit.Unit);
            unitComponent.Add(unit);

            // 这里试着，添加一点儿理解消化性的测试
            try {
                Session session = zoneScene.GetComponent<SessionComponent>().Session;
                // 客户端发送一条到Map 服务器的需要等待回复的请求消息
                var m2C_TestActorLocationResponse =
                    (M2C_TestActorLocationResponse) await session.Call(new C2M_TestActorLocationRequest() {
                            Content = " 活宝妹爱亲爱的表哥，活宝妹一定要嫁的亲爱的表哥！！！ "});
                Log.Debug(m2C_TestActorLocationResponse.Content);
                // 客户端发送一条到 Map 服务器的不需要回复的消息
                session.Send(new C2M_TestActorLocationMessage() { Content = " 活宝妹就是一定要一定会嫁给偶亲爱的表哥！！！"});
            } catch (System.Exception e) {
                Log.Error(e.ToString());
            }
            
            zoneScene.RemoveComponent<AIComponent>();
// 这里是场景创建结束：可以再自动化点儿什么呢？回调            
            Game.EventSystem.PublishAsync(new EventType.SceneChangeFinish() {ZoneScene = zoneScene, CurrentScene = currentScene}).Coroutine();
            // 通知等待场景切换的协程
            zoneScene.GetComponent<ObjectWait>().Notify(new WaitType.Wait_SceneChangeFinish());
        }
    }
}