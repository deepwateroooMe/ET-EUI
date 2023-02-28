using System;
using System.Collections.Generic;
namespace ET {
    [FriendClass(typeof(OpcodeTypeComponent))]
    public static class OpcodeTypeComponentSystem {
        [ObjectSystem] // 它说，这里起始加载的时候就出错了？客户端，感觉这个类，有些部分还是没有看透彻
        public class OpcodeTypeComponentAwakeSystem: AwakeSystem<OpcodeTypeComponent> {
            public override void Awake(OpcodeTypeComponent self) {
                OpcodeTypeComponent.Instance = self;
                self.opcodeTypes.Clear(); // 就是在这个 ET-EUI 系统里，它每次都是被清空了，然后再一个一个又添加的
                self.typeOpcodes.Clear(); // 是每次客户端启动：都会被再清空一次，只填客户端程序集中的？【不一定是程序集，去查事件系统！】
                self.requestResponse.Clear();
                List<Type> types = Game.EventSystem.GetTypes(typeof (MessageAttribute)); // 这些类型的来源：是这个标签系统，是被 EventSystem 扫描出来的
                foreach (Type type in types) {
                    object[] attrs = type.GetCustomAttributes(typeof (MessageAttribute), false); // 不知道：这个数组是什么意思？
                    if (attrs.Length == 0) {
                        continue;
                    }
                    MessageAttribute messageAttribute = attrs[0] as MessageAttribute; // 【0】？ 
                    if (messageAttribute == null) {
                        continue;
                    }
                    self.opcodeTypes.Add(messageAttribute.Opcode, type); // 这两个是相反的，A ＝＝》 B
                    self.typeOpcodes.Add(type, messageAttribute.Opcode); // B ＝＝》 A
                    if (OpcodeHelper.IsOuterMessage(messageAttribute.Opcode) && typeof (IActorMessage).IsAssignableFrom(type)) {
                        self.outrActorMessage.Add(messageAttribute.Opcode);
                    }
                    // 检查request response
                    if (typeof (IRequest).IsAssignableFrom(type)) { // 它会进入到这个分支里面来
                        if (typeof (IActorLocationMessage).IsAssignableFrom(type)) { // 这里为什么一定要是这个类型，才能加进来，不是统计少了太多了吗？
                            self.requestResponse.Add(type, typeof(ActorResponse));
                            continue;
                        }
                        attrs = type.GetCustomAttributes(typeof (ResponseTypeAttribute), false); // 好像是跟 Inner.proto 定义中的 //ResponseType 标签相关，可是我也定义了的呀？只是它没有动态生成？
                        if (attrs.Length == 0) { // <<<<<<<<<<<<<<<<<<<< 这里就出错了：确定没能找到返回类型。可是 proto 里面我是申明清楚了的？
                            Log.Error($"not found responseType: {type}"); // 报错的日志是这一行：
                            continue;
                        }
                        ResponseTypeAttribute responseTypeAttribute = attrs[0] as ResponseTypeAttribute;
                        self.requestResponse.Add(type, Game.EventSystem.GetType($"ET.{responseTypeAttribute.Type}"));
                    }
                }
            }
        }
        [ObjectSystem]
        public class OpcodeTypeComponentDestroySystem: DestroySystem<OpcodeTypeComponent> {
            public override void Destroy(OpcodeTypeComponent self) {
                OpcodeTypeComponent.Instance = null;
            }
        }
        public static bool IsOutrActorMessage(this OpcodeTypeComponent self, ushort opcode) {
            return self.outrActorMessage.Contains(opcode);
        }
        public static ushort GetOpcode(this OpcodeTypeComponent self, Type type) {
            return self.typeOpcodes[type];
        }
        public static Type GetType(this OpcodeTypeComponent self, ushort opcode) {
            return self.opcodeTypes[opcode];
        }
        public static Type GetResponseType(this OpcodeTypeComponent self, Type request) {
            if (!self.requestResponse.TryGetValue(request, out Type response)) {
                throw new Exception($"not found response type, request type: {request.GetType().Name}");
            }
            return response;
        }
    }
    
    [ComponentOf(typeof(Scene))]
    public class OpcodeTypeComponent: Entity, IAwake, IDestroy {
        public static OpcodeTypeComponent Instance;
        
        public HashSet<ushort> outrActorMessage = new HashSet<ushort>();
        
        public readonly Dictionary<ushort, Type> opcodeTypes = new Dictionary<ushort, Type>();
        public readonly Dictionary<Type, ushort> typeOpcodes = new Dictionary<Type, ushort>();
        
        public readonly Dictionary<Type, Type> requestResponse = new Dictionary<Type, Type>();
    }
}