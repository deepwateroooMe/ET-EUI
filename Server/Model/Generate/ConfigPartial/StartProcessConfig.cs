using System.Net;
namespace ET {
    public partial class StartProcessConfig {

        private IPEndPoint innerIPPort;
        public long SceneId;
        public IPEndPoint InnerIPPort { 
            get { // 就是说，如果这个IP 端口还没有连接起来，就建立一个
                if (this.innerIPPort == null) {
                    this.innerIPPort = NetworkHelper.ToIPEndPoint($"{this.InnerIP}:{this.InnerPort}");
                }
                return this.innerIPPort;
            }
        }
        public string InnerIP => this.StartMachineConfig.InnerIP;
        public string OuterIP => this.StartMachineConfig.OuterIP;
        public StartMachineConfig StartMachineConfig => StartMachineConfigCategory.Instance.Get(this.MachineId);

        public override void AfterEndInit() {
            InstanceIdStruct instanceIdStruct = new InstanceIdStruct((int)this.Id, 0);
            this.SceneId = instanceIdStruct.ToLong();
            Log.Info($"StartProcess info: {this.MachineId} {this.Id} {this.SceneId}");
        }
    }
}