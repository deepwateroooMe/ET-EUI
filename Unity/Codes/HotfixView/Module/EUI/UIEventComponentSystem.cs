using System;
namespace ET {
    [ObjectSystem]
    public class UIEventComponentAwakeSystem : AwakeSystem<UIEventComponent> {
        public override void Awake(UIEventComponent self) {
            UIEventComponent.Instance = self;
            self.Awake();
        }
    }
    
    [ObjectSystem]
    public class UIEventComponentDestroySystem : DestroySystem<UIEventComponent> {
        public override void Destroy(UIEventComponent self) {
            self.UIEventHandlers.Clear();
            self.IsClicked = false; // 在这个UIEventComponent进行Destory的时候，置为false
            UIEventComponent.Instance = null;
        }
    }
    [FriendClass(typeof(UIEventComponent))] // 它们也是这么用的， FriendClass
    public static class UIEventComponentSystem {
        public static void Awake(this UIEventComponent self) {
            self.UIEventHandlers.Clear();
            foreach (Type v in Game.EventSystem.GetTypes(typeof (AUIEventAttribute))) {
                AUIEventAttribute attr = v.GetCustomAttributes(typeof (AUIEventAttribute), false)[0] as AUIEventAttribute;
                self.UIEventHandlers.Add(attr.WindowID, Activator.CreateInstance(v) as IAUIEventHandler);
            }
        }
        
        public static IAUIEventHandler GetUIEventHandler(this UIEventComponent self,WindowID windowID) {
            if (self.UIEventHandlers.TryGetValue(windowID, out IAUIEventHandler handler)) {
                return handler;
            }
            Log.Error($"windowId : {windowID} is not have any uiEvent");
            return null;
        }
// 定义给他赋值的拓展方法
        public static void SetUIClicked(this UIEventComponent self, bool isClicked) {
            self.IsClicked = isClicked;
        }
    }
}