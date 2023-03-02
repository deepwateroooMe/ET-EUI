using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace ET {
    
    public static  class DlgLoginSystem {
        // 注册、调用客户端登录事件的方法
        public static void RegisterUIEvent(this DlgLogin self) {
            // 把同步的添加Listener的方法，改为我们刚才的拓展方法，防用户多次连击的
            // 需要的参数是一个Action委托函数，所以我们这里使用lambda表达式，这个()=>{}里面的，就是我们传入进去的函数
            // self.View.E_LoginButton.AddListener(() => { self.OnLoginClickHandler();}); // 【原码】
            self.View.E_LoginButton.AddListenerAsync(() => { return self.OnLoginClickHandler();}); 
        }
        public static void ShowWindow(this DlgLogin self, Entity contextData = null) {
        }
        // 需要的参数是一个需要返回值的异步函数，所以我们得修改我们的登录方法，将它从同步的调用Login，改为异步
        public static async ETTask OnLoginClickHandler(this DlgLogin self) {
            try {
                int errorCode = await LoginHelper.Login(self.DomainScene(), 
                                                        ConstValue.LoginAddress, 
                                                        self.View.E_AccountInputField.GetComponent<InputField>().text, 
                                                        self.View.E_PasswordInputField.GetComponent<InputField>().text);
                if (errorCode != ErrorCode.ERR_Success) {
                    // 服务器返回，不成功
                    Log.Error(errorCode.ToString());
                    return ;
                }
                // todo: 显示登录之后的页面
            } catch (Exception e) {
                Log.Error(e.ToString());
            }
// TODO: 登录成功之后的逻辑。这块儿逻辑在热更新的 HotfixView 层，就是定义不同的界面类型，再调用卸载与加载 
            self.DomainScene().GetComponent<UIComponent>().HideWindow(WindowID.WindowID_Login);
            self.DomainScene().GetComponent<UIComponent>().ShowWindow(WindowID.WindowID_Lobby);
        }
        // // 需要的参数是一个需要返回值的异步函数，所以我们得修改我们的登录方法，将它从同步的调用Login，改为异步
        // public static void OnLoginClickHandler(this DlgLogin self) {
        //     LoginHelper.Login(self.DomainScene(), 
        //                       ConstValue.LoginAddress, 
        //                       self.View.E_AccountInputField.GetComponent<InputField>().text, 
        //                       self.View.E_PasswordInputField.GetComponent<InputField>().text
        //         ).Coroutine();
        //     // 不知道这里为什么，没有这个方法，也就是它称为的默认的方法。因为说是是注释掉的，上面添加的是异步的方法
        //     // LoginHelper.LoginTest(self.DomainScene(), ConstValue.LoginAddress).Coroutine();  // 应该有的【源码 】
        // }
        public static void HideWindow(this DlgLogin self) {
        }
        
    }
}
