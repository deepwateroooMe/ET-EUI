namespace ET {
    namespace WaitType {
// 定义成了接口；凡实现接口的，都自成等待类型
        public struct Wait_UnitStop: IWaitType {
            public int Error {
                get;
                set;
            }
        }
        public struct Wait_CreateMyUnit: IWaitType {
            public int Error {
                get;
                set;
            }
            public M2C_CreateMyUnit Message;
        }
        public struct Wait_SceneChangeFinish: IWaitType {
            public int Error {
                get;
                set;
            }
        }
    }
}