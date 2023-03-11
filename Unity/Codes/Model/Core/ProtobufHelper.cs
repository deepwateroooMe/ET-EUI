using System;
using System.Collections.Generic;
#if NOT_UNITY
using System.ComponentModel;
#endif
using System.IO;
using ProtoBuf;
using ProtoBuf.Meta;
namespace ET {
    public static class ProtobufHelper {
        private const string TAG = "ProtobufHelper";
        public static void Init() {
        }
        public static object FromBytes(Type type, byte[] bytes, int index, int count) {
            using (MemoryStream stream = new MemoryStream(bytes, index, count)) {
                object o = RuntimeTypeModel.Default.Deserialize(stream, null, type);
                if (o is ISupportInitialize supportInitialize) {
                    supportInitialize.EndInit();
                }
                return o;
            }
        }
        public static byte[] ToBytes(object message) {
            using (MemoryStream stream = new MemoryStream()) {
                ProtoBuf.Serializer.Serialize(stream, message);
                return stream.ToArray();
            }
        }
        public static void ToStream(object message, MemoryStream stream) {
            ProtoBuf.Serializer.Serialize(stream, message);
        }
        public static object FromStream(Type type, MemoryStream stream) {
            Log.Error(TAG, "stream.Length.ToString(): " + stream.Length.ToString());
            Log.Error(TAG, "type.Name: " + type.Name);
            object o = RuntimeTypeModel.Default.Deserialize(stream, null, type);
            if (o is ISupportInitialize supportInitialize) { // 这里是，有没有注册什么回调接口之类的
                supportInitialize.EndInit();
            }
            return o;
        }
    }
}

