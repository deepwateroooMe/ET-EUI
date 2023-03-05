using System;
#if NOT_UNITY
using System.ComponentModel;
#endif
namespace ET {
    
#if !NOT_UNITY
    public interface ISupportInitialize {
        void BeginInit();
        void EndInit();
    }
    public interface IDisposable {
        void Dispose();
    }
#endif
    
    public abstract class ProtoObject: Object, ISupportInitialize {
        public object Clone() {
            byte[] bytes = ProtobufHelper.ToBytes(this); // 这里，把当前对象转化为字节数组：内部使用内存流，先前对象序列化到内存流，再从内存流转化为字节数组
            return ProtobufHelper.FromBytes(this.GetType(), bytes, 0, bytes.Length);
        }
        public virtual void BeginInit() {
        }
        public virtual void EndInit() {
        }
        public virtual void AfterEndInit() {
        }
    }
    
    public abstract class DisposeObject: Object, IDisposable, ISupportInitialize {
        public virtual void Dispose() {
        }
        public virtual void BeginInit() {
        }
        public virtual void EndInit() {
        }
#if !NOT_UNITY
        public override string ToString() {
            return this.GetType().Name;
        }
#endif
    }
}