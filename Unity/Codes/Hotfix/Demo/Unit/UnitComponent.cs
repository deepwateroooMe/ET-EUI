namespace ET {
    [ObjectSystem]
    public class UnitComponentAwakeSystem : AwakeSystem<UnitComponent> {
        public override void Awake(UnitComponent self) {
        }
    }
    [ObjectSystem]
    public class UnitComponentDestroySystem : DestroySystem<UnitComponent> {
        public override void Destroy(UnitComponent self) {
        }
    }
    
    public static class UnitComponentSystem {
        // 这里的逻辑：什么也没有添加，为什么使用的时候，还要调用一下呢？不是。。。？！！
        public static void Add(this UnitComponent self, Unit unit) {
        }
        public static Unit Get(this UnitComponent self, long id) {
            Unit unit = self.GetChild<Unit>(id);
            return unit;
        }
        public static void Remove(this UnitComponent self, long id) {
            Unit unit = self.GetChild<Unit>(id);
            unit?.Dispose();
        }
    }
}