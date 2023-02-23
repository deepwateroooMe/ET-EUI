using System;

namespace ET {
    
    [Timer(TimerType.AccountSessionCheckOutTime)] // 这里与，下面Awake() 里设置的闹钟类型是一致的，必须一致
    public class AccountSessionCheckOutTimer : ATimer<AccountCheckOutTimeComponent> {
        public override void Run(AccountCheckOutTimeComponent self) {
            try {
                self.DeleteSession();
            } catch (Exception e) {
                Log.Error(e.ToString());
            }
        }
    }
    public class AccountCheckOutTimeComponentAwakeSystem : AwakeSystem<AccountCheckOutTimeComponent, long> {
        public override void Awake(AccountCheckOutTimeComponent self, long accountId) {
            self.AccountId = accountId;
            // ref 关键字，传入前必须初始化
            TimerComponent.Instance.Remove(ref self.Timer);
            self.Timer = TimerComponent.Instance.NewOnceTimer(TimeHelper.ServerNow() + 60000, TimerType.AccountSessionCheckOutTime, self);
        }
    }
    public class AccountCheckOutTimeComponentDestroySystem : DestroySystem<AccountCheckOutTimeComponent> {
        public override void Destroy(AccountCheckOutTimeComponent self) {
            self.AccountId = 0;
            // 清空计时器： 它是用单例静态【？】管理类来清空当前这个实例的。因为单例对对它的管理【添加在管理字典里等】，需要处理 
            TimerComponent.Instance.Remove(ref self.Timer); 
        }
    }
    [FriendClassAttribute(typeof(ET.AccountCheckOutTimeComponent))]// 十分钟等待，如果玩家还没有进行下一步操作，我们就断开Session，避免客户端一直占用Account服务器
    public static class AccountCheckOutTimeComponentSystem {
        // public long Timer = 0;
        // public long AccountId;
        public static void DeleteSession(this AccountCheckOutTimeComponent self) {
            // 从父物体身上获取 session
            Session session = self.GetParent<Session>();
            // 拿到注册登录服已经登录的 session 的 instanceId
            long sessionInstanceId = session.DomainScene().GetComponent<AccountSessionsComponent>().Get(self.AccountId);
            // 比较父物体身上的 instanceId 和 AccountId 所对应的 instanceId 是不是同一个
            if (session.InstanceId == sessionInstanceId) {
                // 是同一个，那么就可以移除注册登录服 Session 字典缓存的 session
                session.DomainScene().GetComponent<AccountSessionsComponent>().Remove(self.AccountId);
            }
            // 然后通知客户端连接已断开
            session?.Send(new A2C_Disconnect() { Error = 1 });
            session?.Disconnect().Coroutine();
        }
    }
}
   