 namespace ET {
    // 这个类的作用：是为了解决用户多次点击登录 多次消息处理的bug
     [ComponentOf(typeof(Session))]
    [ChildType(typeof(Account))]
     public class SessionLockingComponent : Entity, IAwake {
         
     }
 }