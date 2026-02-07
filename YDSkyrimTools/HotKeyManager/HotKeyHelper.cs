using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;

namespace YDSkyrimTools.HotKeyManager
{
    public class HotKeyHelper
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd,                //要定义热键的窗口的句柄
      int id,                     //定义热键ID（不能与其它ID重复）
       KeyModifiers fsModifiers,   //标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效
       Keys vk                     //定义热键的内容
       );
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(
            IntPtr hWnd,                //要取消热键的窗口的句柄
            int id                      //要取消热键的ID
            );
        //定义了辅助键的名称（将数字转变为字符以便于记忆，也可去除此枚举而直接使用数值）
        [Flags()]
        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Ctrl = 2,
            Shift = 4,
            WindowsKey = 8
        }
        public static void Init(IntPtr Any)
        {
            HotKeyHelper.RegisterHotKey(Any, 100, KeyModifiers.None, Keys.F8);
            HotKeyHelper.RegisterHotKey(Any, 101, KeyModifiers.None, Keys.F6);
            HotKeyHelper.RegisterHotKey(Any, 102, KeyModifiers.None, Keys.F7);
        }

        public static void UnregisterHotkeys(IntPtr Any)
        {
            UnregisterHotKey(Any, 100);
            UnregisterHotKey(Any, 101);
            UnregisterHotKey(Any, 102);
        }

    }
}
