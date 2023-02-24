namespace ET {
    public enum AccountType {
        General = 0,
        BlackList = 1
    }
    [ChildType(typeof(Session))]
    //[ComponentOf(typeof(Session))]
    public class Account : Entity, IAwake {
        public string AccountName;
        public string Password;
        public long CreateTime;
        public int AccountType;
    }
}