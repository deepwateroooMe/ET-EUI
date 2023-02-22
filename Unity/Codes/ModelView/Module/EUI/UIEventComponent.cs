using System.Collections.Generic;
namespace ET {
// 数据字段和逻辑进行分离：
    // 进入EUIHelper，在这里我们为按钮添加自己定义的listener的时候，我们定义了一个isClicked的静态字段，但是这个类是在HotfixView层，不允许存在数据字段
    [ComponentOf(typeof(Scene))]
    public class UIEventComponent : Entity,IAwake,IDestroy {

        public static UIEventComponent Instance { get; set; }
        public readonly Dictionary<WindowID, IAUIEventHandler> UIEventHandlers = new Dictionary<WindowID, IAUIEventHandler>();
        public bool IsClicked { get; set; }
    }
}