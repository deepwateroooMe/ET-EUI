using System.Collections.Generic;
namespace ET {
    [ComponentOf(typeof(Scene))]
    [ChildType(typeof(Player))]
    public class PlayerComponent : Entity, IAwake, IDestroy { // 它没有说是单例，但感觉应该是单例才对
        public readonly Dictionary<long, Player> idPlayers = new Dictionary<long, Player>();
    }
}