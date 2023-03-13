using System.Linq;
namespace ET {

    [FriendClass(typeof(PlayerComponent))]
    public static class PlayerComponentSystem { // 这里，它本质上就是一个单例，用来管理，与这个当前网关服相连接的，所有玩家 
        // 为什么：空方法，也必须得写这两个方法？
        public class AwakeSystem : AwakeSystem<PlayerComponent> {
            public override void Awake(PlayerComponent self) {  }
        }
        [ObjectSystem] // <<<<<<<<<< 这是自己整的吗？一个方法有，一个方法没有
        public class PlayerComponentDestroySystem: DestroySystem<PlayerComponent> {
            public override void Destroy(PlayerComponent self) {
            }
        }

        public static void Add(this PlayerComponent self, Player player) {
            self.idPlayers.Add(player.Id, player);
        }
        public static Player Get(this PlayerComponent self,long id) {
            self.idPlayers.TryGetValue(id, out Player gamer);
            return gamer;
        }
        public static void Remove(this PlayerComponent self,long id) {
            self.idPlayers.Remove(id);
        }
        public static Player[] GetAll(this PlayerComponent self) { // 字典里的值，作为数组返回回去
            return self.idPlayers.Values.ToArray();
        }
    }
}