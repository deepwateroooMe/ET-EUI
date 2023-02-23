using System.Collections.Generic;

namespace ET {
    [ComponentOf]
    public class AccountSessionsComponent : Entity, IAwake, IDestroy {
        public Dictionary<long, long> AccountSessionDictionary = new Dictionary<long, long>();
    }
}