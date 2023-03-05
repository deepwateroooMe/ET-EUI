using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
namespace ET {
    public static class NetworkHelper {
        public static string[] GetAddressIPs() {
            List<string> list = new List<string>();
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces()) {
                if (networkInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet) {
                    continue;
                }
                foreach (UnicastIPAddressInformation add in networkInterface.GetIPProperties().UnicastAddresses) {
                    list.Add(add.Address.ToString());
                }
            }
            return list.ToArray();
        }
        public static IPEndPoint ToIPEndPoint(string host, int port) {
            return new IPEndPoint(IPAddress.Parse(host), port); // 把一个 18 位长的字符串解析为：可连接的，IP 地址与端口， IPEndPoint 封装
        }
        public static IPEndPoint ToIPEndPoint(string address) { // 把一个 18 位长的字符串解析为：可连接的，IP 地址与端口， IPEndPoint 封装
            int index = address.LastIndexOf(':');
            string host = address.Substring(0, index);
            string p = address.Substring(index + 1);
            int port = int.Parse(p);
            return ToIPEndPoint(host, port);
        }
    }
}
