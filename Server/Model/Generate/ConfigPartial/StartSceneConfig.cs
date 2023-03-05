using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
namespace ET {
    public partial class StartSceneConfigCategory {

        public MultiMap<int, StartSceneConfig> Gates = new MultiMap<int, StartSceneConfig>();
        public MultiMap<int, StartSceneConfig> ProcessScenes = new MultiMap<int, StartSceneConfig>();
        public Dictionary<long, Dictionary<string, StartSceneConfig>> ZoneScenesByName = new Dictionary<long, Dictionary<string, StartSceneConfig>>();
        public StartSceneConfig LocationConfig;
        public List<StartSceneConfig> Robots = new List<StartSceneConfig>();

        public List<StartSceneConfig> GetByProcess(int process) {
            return this.ProcessScenes[process];
        }
        public StartSceneConfig GetBySceneName(int zone, string name) {
            return this.ZoneScenesByName[zone][name];
        }
        public override void AfterEndInit() { // 初始化完成之后的回调：
            foreach (StartSceneConfig startSceneConfig in this.GetAll().Values) {
                this.ProcessScenes.Add(startSceneConfig.Process, startSceneConfig);
                if (!this.ZoneScenesByName.ContainsKey(startSceneConfig.Zone)) {
                    this.ZoneScenesByName.Add(startSceneConfig.Zone, new Dictionary<string, StartSceneConfig>());
                }
                this.ZoneScenesByName[startSceneConfig.Zone].Add(startSceneConfig.Name, startSceneConfig);
                switch (startSceneConfig.Type) {
                    case SceneType.Gate:
                        this.Gates.Add(startSceneConfig.Zone, startSceneConfig);
                        break;
                    case SceneType.Location:
                        this.LocationConfig = startSceneConfig;
                        break;
                    case SceneType.Robot:
                        this.Robots.Add(startSceneConfig);
                        break;
                }
            }
        }
    }
// 外网地址：就是我们常说的公网IP地址。一个局域网里所有电脑的内网IP是互不相同的,但共用一个外网IP
// 内网地址：就是路由器分配给设备的地址，也可称为局域网IP地址
// 内部端口：是指内部服务器的应用所使用的端口，也就是我们内网电脑提供服务所对应得端口号。比如常见得数据库服务对应的默认端口是1433、IIS服务对应的默认端口号是80、远程桌面服务对应的默认端口是3389等。
// 外部端口：是指外网访问该映射的服务器应用所使用的端口。也就是外网电脑访问时输入的端口号。也就是外网电脑访问时输入的端口号，这个端口在设置的时候可与内部端口号一致，也可不一致
// 在局域网中，每台电脑都可以自己分配自己的IP，这个IP只在局域网中有效。而如果你将电脑连接到互联网，你的网络提供商（ISP）的服务器会为你分配一个IP地址，这个IP地址才是你在外网的IP。两个IP同时存在，一个对内，一个对外。
    public partial class StartSceneConfig : ISupportInitialize { 
        public long InstanceId;
        public SceneType Type;
        public StartProcessConfig StartProcessConfig {
            get {
                return StartProcessConfigCategory.Instance.Get(this.Process);
            }
        }
        public StartZoneConfig StartZoneConfig {
            get {
                return StartZoneConfigCategory.Instance.Get(this.Zone);
            }
        }
// 感觉这里：外网端口，内网端口，概念上还有点儿区分不清楚
        // 内网地址外网端口，通过防火墙映射端口过来
        private IPEndPoint innerIPOutPort;
        public IPEndPoint InnerIPOutPort {
            get {
                if (innerIPOutPort == null) {
                    this.innerIPOutPort = NetworkHelper.ToIPEndPoint($"{this.StartProcessConfig.InnerIP}:{this.OuterPort}");
                }
                return this.innerIPOutPort;
            }
        }
        private IPEndPoint outerIPPort;
        // 外网地址外网端口
        public IPEndPoint OuterIPPort {
            get {
                if (this.outerIPPort == null) {
                    this.outerIPPort = NetworkHelper.ToIPEndPoint($"{this.StartProcessConfig.OuterIP}:{this.OuterPort}");
                }
                return this.outerIPPort;
            }
        }
        public override void BeginInit() {
        }
        public override void EndInit() {
            this.Type = EnumHelper.FromString<SceneType>(this.SceneType);
            InstanceIdStruct instanceIdStruct = new InstanceIdStruct(this.Process, (uint) this.Id);
            this.InstanceId = instanceIdStruct.ToLong();
        }
    }
}