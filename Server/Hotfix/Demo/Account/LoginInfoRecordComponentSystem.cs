using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET {
    // [ObjectSystem] // 下面的这个类，好像还有很多地方不对，只是 Awake(),也可以不用写
    // public class LoginInfoRecordComponentAwakeSystem : AwakeSystem<LoginInfoRecordComponent> {
    //     public override void Awake(this LoginInfoRecordComponent self) {
    //         self.accountsZones.Clear();
    //     }
    // }
    
    [FriendClass(typeof(LoginInfoRecordComponent))]
	public static class LoginInfoRecordComponentSystem {
        public static bool IsExist(this LoginInfoRecordComponent self, long accountId) {
            return self.accountsZones.ContainsKey(accountId);
        }
        public static int Get(this LoginInfoRecordComponent self, long accountId) {
            int zone = -1;
            if (self.accountsZones.TryGetValue(accountId, out zone))
                return zone;
            return -1;
        }
        // 既然是可以 Get(), 当然也是可以添加的, 添加就是加键值对呀
        public static void Add(this LoginInfoRecordComponent self, long accountId, int zone) { // 这里的返回类型可以再检查一下。暂时还没有使用的地方
            self.accountsZones.Add(accountId, zone);
        }
    }
}
