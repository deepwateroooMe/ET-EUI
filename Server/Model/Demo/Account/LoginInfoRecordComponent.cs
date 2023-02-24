using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET {
    [ComponentOf(typeof(Scene))] // 这里把它定义成为，身背一个本地缓存，HashSet的形式？   
    public class LoginInfoRecordComponent : Entity, IAwake {
        public readonly Dictionary<long, int> accountsZones = new Dictionary<long, int>();
	}
}
