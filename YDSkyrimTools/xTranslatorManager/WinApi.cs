
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace YDSkyrimTools.xTranslatorManager
{
    public class WinApi
    {

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint Flags);


        [System.Runtime.InteropServices.DllImport("user32")]
        public static extern int mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "SendMessageW")]
        public static extern int SendMessageW2([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, uint Msg, uint wParam, IntPtr lParam);
        public const int WM_GETTEXT = 13;



        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "SendMessageW")]
        public static extern int SendMessageA([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, uint Msg, uint wParam, IntPtr lParam);


        public const int WM_LBUTTONDOWN = 513; // 鼠标左键按下
        public const int WM_LBUTTONUP = 514; // 鼠标左键抬起
        public const int WM_RBUTTONDOWN = 516; // 鼠标右键按下
        public const int WM_RBUTTONUP = 517; // 鼠标右键抬起
        public const int WM_MBUTTONDOWN = 519; // 鼠标中键按下
        public const int WM_MBUTTONUP = 520; // 鼠标中键抬起

        public const int MOUSEEVENTF_MOVE = 0x0001; // 移动鼠标       
        public const int MOUSEEVENTF_LEFTDOWN = 0x0002; // 鼠标左键按下      
        public const int MOUSEEVENTF_LEFTUP = 0x0004; // 鼠标左键抬起      
        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008; // 鼠标右键按下     
        public const int MOUSEEVENTF_RIGHTUP = 0x0010; // 鼠标右键抬起        
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; // 鼠标中键按下  
        public const int MOUSEEVENTF_MIDDLEUP = 0x0040; // 鼠标中键抬起         
        public const int MOUSEEVENTF_ABSOLUTE = 0x8000; // 绝对坐标 

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr WindowFromPoint(int xPoint, int yPoint);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ScreenToClient(IntPtr hWnd, ref System.Drawing.Point lppt);


        [DllImport("user32.dll")]
        public static extern int EnumWindows(EnumWindowsProc ewp, int lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(int hWnd, StringBuilder title, int size);

        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(int hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "MoveWindow")]
        public static extern bool MoveWindow(System.IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        public static extern void BlockInput(bool Block);

        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        public extern static IntPtr FindWindow(string classname, string captionName);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);

        [DllImport("user32.dll", EntryPoint = "PostMessage")]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);




        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AnySendMessage(IntPtr hwnd, StringBuilder Message, bool ShowThis);

        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AnySendMessageR(IntPtr hwnd, StringBuilder Message);

        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AnySendCrlf(IntPtr hwnd, bool ShowThis);

        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AnySendTab(IntPtr hwnd, bool ShowThis);

        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AnySendTabR(IntPtr hwnd);

        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AnySendTabRR(IntPtr hwnd);

        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AnySendDown(IntPtr hwnd);

        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AnySendUP(IntPtr hwnd);

        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AnySendESC(IntPtr hwnd);

        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SendCtrlAndEnter(IntPtr hwnd);

        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AnySendEnter(IntPtr hwnd);

        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AnySendF6(IntPtr hwnd, bool ShowThis);

        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AnyAutoDelBlockText(IntPtr hwnd, int Count, bool ShowThis);

        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AnyAutoDelBlockTextR(IntPtr hwnd, int Count, int SSleep);

        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SelectAll(IntPtr hwnd, bool ShowThis, int X, int Y);

        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ClickTargetMy(IntPtr hwnd, int X, int Y);

        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DoubleClickTargetMy(IntPtr hwnd, int X, int Y);

        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ClickTarget(IntPtr hwnd, bool ShowThis, int X, int Y);

        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ClickTargetR(IntPtr hwnd, int X, int Y);

        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ClickTargetRR(IntPtr hwnd, int X, int Y);

        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ClickTargetByRight(IntPtr hwnd, bool ShowThis, int X, int Y);

        [DllImport("MessageControl.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DoubleClickTarget(IntPtr hwnd, bool ShowThis, int X, int Y);

        [DllImport("MessageControl.dll", CallingConvention = CallingConvention.Cdecl, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public static extern bool KrnGetProcessPath(uint PID, StringBuilder PPath);



        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref WinRECT lpRect);


        #region Dll引用
        [DllImport("User32.dll", EntryPoint = "GetDC")]
        private extern static IntPtr GetDC(IntPtr hWnd);

        [DllImport("User32.dll", EntryPoint = "ReleaseDC")]
        private extern static int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("User32.dll")]
        public static extern int GetSystemMetrics(int hWnd);

        const int DESKTOPVERTRES = 117;
        const int DESKTOPHORZRES = 118;

        const int SM_CXSCREEN = 0;
        const int SM_CYSCREEN = 1;

        #endregion


        /// <summary>
        /// 获取DPI缩放比例
        /// </summary>
        /// <param name="dpiscalex"></param>
        /// <param name="dpiscaley"></param>
        public static void GetDPIScale(ref float dpiscalex, ref float dpiscaley)
        {
            int x = GetSystemMetrics(SM_CXSCREEN);
            int y = GetSystemMetrics(SM_CYSCREEN);
            IntPtr hdc = GetDC(IntPtr.Zero);
            int w = GetDeviceCaps(hdc, DESKTOPHORZRES);
            int h = GetDeviceCaps(hdc, DESKTOPVERTRES);
            ReleaseDC(IntPtr.Zero, hdc);
            dpiscalex = (float)w / x;
            dpiscaley = (float)h / y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WinRECT
        {
            public int Left; //最左坐标
            public int Top; //最上坐标
            public int Right; //最右坐标
            public int Bottom; //最下坐标
        }

        public delegate bool EnumWindowsProc(int hWnd, int lParam);
        // 键盘虚拟键值
        public const int VK_CONTROL = 0X11;
        public const int VK_MENU = 0X12;
        public const int VK_S = 0X53;          // 'S'
        public const int VK_F4 = 0X73;

        public const int KEYEVENTF_EXTENDEDKEY = 0x1;
        public const int KEYEVENTF_KEYUP = 0x2;

        // WindowMessage 参数
        public const int WM_KEYDOWN = 0X100;
        public const int WM_KEYUP = 0X101;
        public const int WM_SYSCHAR = 0X106;
        public const int WM_SYSKEYUP = 0X105;
        public const int WM_SYSKEYDOWN = 0X104;
        public const int WM_CHAR = 0X102;
        public const int WM_DOWN = 0X0028;
        public const int WM_ENTER = 0X000D;
        public const int WM_CLOSE = 0X0010;
        public const int WM_QUIT = 0X0012;


        // keybd_event 状态参数
        public const int WM_SYSCOMMAND = 0x111;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int WM_SETFOCUS = 0x0007;


        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Int32 bInheritHandle, UInt32 dwProcessId);

        [DllImport("psapi.dll")]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In][MarshalAs(UnmanagedType.U4)] int nSize);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        public static string GetWindowModuleFileName(int PID)
        {
            uint processId = (uint)PID;
            const int nChars = 1024;
            StringBuilder filename = new StringBuilder(nChars);
            IntPtr hProcess = OpenProcess(1040, 0, processId);
            GetModuleFileNameEx(hProcess, IntPtr.Zero, filename, nChars);
            CloseHandle(hProcess);
            return (filename.ToString());
        }

        public static int MakeDWord(int low, int high)
        {
            return low + (high * Abs(~ushort.MaxValue));
        }

        public static int Abs(int value)
        {
            return ((value >> 31) ^ value) - (value >> 31);
        }

        public static bool screen_to_client(IntPtr hwnd, ref int x, ref int y)
        {
            bool bRetVal = false;
            System.Drawing.Point lpptPos = new System.Drawing.Point(x, y);
            if ((bRetVal = ScreenToClient(hwnd, ref lpptPos)))
            {
                x = lpptPos.X;
                y = lpptPos.Y;
            }
            return bRetVal;
        }



    }
}
