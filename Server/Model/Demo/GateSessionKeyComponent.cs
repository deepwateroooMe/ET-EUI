using System.Collections.Generic;
namespace ET {

    [ComponentOf(typeof(Scene))]
    public class GateSessionKeyComponent : Entity, IAwake {

        // 字典：纪录当前网关下，注册生成过的有效 Key, 以及所对应的客户端登录帐户用户的 account
        public readonly Dictionary<long, string> sessionKey = new Dictionary<long, string>();
    }
}