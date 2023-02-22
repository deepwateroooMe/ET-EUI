using UnityEngine;
namespace ET {
    public static class UnitFactory { // 这个Unit: 是个玩家的什么属性性质？
        public static Unit Create(Scene currentScene, UnitInfo unitInfo) {
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            Unit unit = unitComponent.AddChildWithId<Unit, int>(unitInfo.UnitId, unitInfo.ConfigId);
            unitComponent.Add(unit);
            
            unit.Position = new Vector3(unitInfo.X, unitInfo.Y, unitInfo.Z); // 带玩家，当前位置
            unit.Forward = new Vector3(unitInfo.ForwardX, unitInfo.ForwardY, unitInfo.ForwardZ); // 带玩家，需要变化的位置。大概它的旋转另有逻辑吧
            
            NumericComponent numericComponent = unit.AddComponent<NumericComponent>(); // 这个对数字进行管理的，不用看
            for (int i = 0; i < unitInfo.Ks.Count; ++i) {
                numericComponent.Set(unitInfo.Ks[i], unitInfo.Vs[i]);
            }
            unit.AddComponent<MoveComponent>();
            if (unitInfo.MoveInfo != null) {
                if (unitInfo.MoveInfo.X.Count > 0) {
                    using (ListComponent<Vector3> list = ListComponent<Vector3>.Create()) {
                        list.Add(unit.Position);
                        for (int i = 0; i < unitInfo.MoveInfo.X.Count; ++i) {
                            list.Add(new Vector3(unitInfo.MoveInfo.X[i], unitInfo.MoveInfo.Y[i], unitInfo.MoveInfo.Z[i]));
                        }
                        unit.MoveToAsync(list).Coroutine();
                    }
                }
            }
            unit.AddComponent<ObjectWait>();
            unit.AddComponent<XunLuoPathComponent>();
            
            Game.EventSystem.Publish(new EventType.AfterUnitCreate() {Unit = unit});
            return unit;
        }
    }
}
