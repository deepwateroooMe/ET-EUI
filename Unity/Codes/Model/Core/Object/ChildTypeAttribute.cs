using System;
namespace ET {
    // 实体类AddChild方法参数约束
    // 不填type参数 则不限制Child类型
    [AttributeUsage(AttributeTargets.Class)]
    public class ChildTypeAttribute : Attribute {
        public Type type;
        public ChildTypeAttribute(Type type = null) { // 这里不申明类型：默认会是空，所以应该申明 
            this.type = type;
        }
    }
}