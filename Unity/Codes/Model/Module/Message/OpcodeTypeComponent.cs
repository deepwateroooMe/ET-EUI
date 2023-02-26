using System;
using System.Collections.Generic;
namespace ET {
    [FriendClass(typeof(OpcodeTypeComponent))]
    public static class OpcodeTypeComponentSystem {
        [ObjectSystem] // 它说，这里起始加载的时候就出错了？
        public class OpcodeTypeComponentAwakeSystem: AwakeSystem<OpcodeTypeComponent> {
            public override void Awake(OpcodeTypeComponent self) {
                OpcodeTypeComponent.Instance = self;
                self.opcodeTypes.Clear();
                self.typeOpcodes.Clear();
                self.requestResponse.Clear();
                List<Type> types = Game.EventSystem.GetTypes(typeof (MessageAttribute));
                foreach (Type type in types) {
                    object[] attrs = type.GetCustomAttributes(typeof (MessageAttribute), false);
                    if (attrs.Length == 0) {
                        continue;
                    }
                    MessageAttribute messageAttribute = attrs[0] as MessageAttribute;
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
                        if (typeof (IActorLocationMessage).IsAssignableFrom(type)) {
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
