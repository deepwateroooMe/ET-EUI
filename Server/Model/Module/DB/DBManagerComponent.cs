namespace ET {

    [ChildType(typeof(DBComponent))]
    [ComponentOfAttribute(typeof(Scene))]
    public class DBManagerComponent: Entity, IAwake, IDestroy {
        public static DBManagerComponent Instance; // 单例类型

        public DBComponent[] DBComponents = new DBComponent[IdGenerater.MaxZone];
    }
}