using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YDSkyrimTools.xTranslatorManager
{
    public class FindMainWindows
    {
        private delegate bool WNDENUMPROC(IntPtr hWnd, int lParam);
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, int lParam);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowW(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        public static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString, int nMaxCount);

        public struct WindowInfo
        {
            public IntPtr hWnd;
            public string szWindowName;
            public string szClassName;
        }

        public WindowInfo[] GetAllDesktopWindows()
        {
            List<WindowInfo> wndList = new List<WindowInfo>();
            //enum all desktop windows
            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                WindowInfo wnd = new WindowInfo();
                StringBuilder sb = new StringBuilder(256);
                //get hwnd
                wnd.hWnd = hWnd;
                //get window name
                GetWindowTextW(hWnd, sb, sb.Capacity);
                wnd.szWindowName = sb.ToString();
                //get window class
                GetClassNameW(hWnd, sb, sb.Capacity);
                wnd.szClassName = sb.ToString();
                //add it into list
                wndList.Add(wnd);
                return true;
            }, 0);
            return wndList.ToArray();
        }

        public bool SearchDeskTopWin(string WindowName, string ClassName, ref IntPtr LockerHwnd)
        {
            foreach (var GetItem in GetAllDesktopWindows())
            {
                if (GetItem.szWindowName != null)
                {
                    if (WindowName.Length > 0)
                        if (GetItem.szWindowName.Contains(WindowName))
                        {
                            if (!GetItem.szClassName.Contains("TPUtilWindow"))
                            {
                                if (ClassName.Trim().Length > 0)
                                {
                                    if (GetItem.szClassName.Contains(ClassName))
                                    {
                                        LockerHwnd = GetItem.hWnd;
                                        return true;
                                    }
                                }
                                else
                                {
                                    LockerHwnd = GetItem.hWnd;
                                    return true;
                                }
                            }
                        }
                }


                if (GetItem.szClassName != null)
                {
                    if (ClassName.Length > 0)
                        if (GetItem.szClassName.Contains(ClassName))
                        {
                            if (!GetItem.szWindowName.Contains("TPUtilWindow"))
                            {
                                if (WindowName.Length > 0)
                                {
                                    if (GetItem.szWindowName.Contains(WindowName))
                                    {
                                        LockerHwnd = GetItem.hWnd;
                                        return true;
                                    }
                                }
                                else
                                {
                                    LockerHwnd = GetItem.hWnd;
                                    return true;
                                }
                            } 
                        }
                }

            }
            return false;
        }
    }
}
