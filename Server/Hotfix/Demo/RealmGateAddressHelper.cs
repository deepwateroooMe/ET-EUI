using System.Collections.Generic;
namespace ET {
    public static class RealmGateAddressHelper {

        public static StartSceneConfig GetGate(int zone) {
            List<StartSceneConfig> zoneGates = StartSceneConfigCategory.Instance.Gates[zone];
            
            // int n = RandomHelper.RandomNumber(0, zoneGates.Count); // 这里每次都是随机数生成的，不确定不固定的网关地址
// 重载一个方法，让每次拿到的是固定的Gate网关地址
            int n = accountId.GetHashCode() % zoneGates.Count; // 对于每个登录过的帐户来说，每次拿到的都是它当前所属风关的地址
            
            return zoneGates[n];
        }
    }
}
