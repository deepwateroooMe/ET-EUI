namespace ET {
    public enum SceneType { // 通过 SceneFactory 来生产实例的

        Process = 0,
        Manager = 1,
        Realm = 2,
        Gate = 3,
        Http = 4,
        Location = 5,
        Map = 6,
        Account = 7,  // 添加一个自定义的场景类型，注册登录界面
        
        // 客户端Model层
        Client = 30,
        Zone = 31,
        Login = 32,
        Robot = 33,
        Current = 34,
    }
}