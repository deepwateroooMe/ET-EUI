namespace ET
{
    public static partial class ErrorCode
    {
        public const int ERR_Success = 0;

        // 1-11004 是SocketError请看SocketError定义
        //-----------------------------------
        // 100000-109999是Core层的错误
        
        // 110000以下的错误请看ErrorCore.cs
        
        // 这里配置逻辑层的错误码
        // 110000 - 200000是抛异常的错误
        // 200001以上不抛异常

        // 自定义，加上一个网络出错
        public const int ERR_NetWorkError = 200002;
        public const int ERR_LoginInfoError = 200003;
        public const int ERR_AccountNameFormError = 200004;
        public const int ERR_PasswordFromError = 200005;
        public const int ERR_LoginBlackListError = 200006;
        public const int ERR_LoginPasswordError = 200007;
    }
}