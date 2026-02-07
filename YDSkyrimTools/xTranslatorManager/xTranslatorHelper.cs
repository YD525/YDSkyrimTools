

using JavaScriptEngineSwitcher.Core.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using YDSkyrimTools.ConvertManager;
using YDSkyrimTools.TranslateCore;
using YDSkyrimTools.UIManager;
using static System.Net.Mime.MediaTypeNames;

namespace YDSkyrimTools.xTranslatorManager
{
    public class xTranslatorHelper
    {
        public static IntPtr xTranslatorFormHwnd = new IntPtr();
        public static IntPtr xTranslatorOperateHwnd = new IntPtr();
        public static string TransTargetName = "";

        public static bool StopAny = false;
        public static bool IsReload = false;
        [DllImport("user32.dll")]
        public static extern int EnumChildWindows(IntPtr hWndParent, CallBack lpfn, int lParam);
        public delegate bool CallBack(IntPtr hwnd, int lParam);

        public static int IsEnd = 1;

        private static int ListenServiceCount = 0;
        public static void StartListenService(System.Windows.Shapes.Ellipse OneAction, System.Windows.Controls.Label CurrentState, System.Windows.Controls.Label CurrentData)
        {
            if (ListenServiceCount == 0)
            {
                ListenServiceCount++;
                new Thread(() =>
                {

                    while (true)
                    {
                        Thread.Sleep(200);

                        try
                        {
                            if (Process.GetProcessesByName("xTranslator").Length > 0)
                            {
                                if (IsEnd == 1)
                                {
                                    new SoundPlayer().PlaySound(Sounds.检测到进程);
                                    IsEnd = 0;
                                }
                            }
                            else
                            {
                                if (IsEnd == 0)
                                {
                                    new SoundPlayer().PlaySound(Sounds.停止监控进程);
                                    IsEnd = 1;
                                }
                            }

                            if (!StopAny)
                            {
                                if (Process.GetProcessesByName("xTranslator").Length > 0)
                                {
                                    if (!IsReload)
                                    {
                                        IsReload = true;

                                        GetxTranslatorInFo();
                                    }

                                    //var GetText = xTranslatorHelper.FindxTranslatorFormContent();

                                    //if (GetText != null)
                                    //    if (GetText.Length > 0)
                                    //        CurrentData.Dispatcher.Invoke(new Action(() =>
                                    //        {
                                    //            CurrentData.Content = GetText;
                                    //        }));

                                    CurrentState.Dispatcher.Invoke(new Action(() =>
                                    {
                                        CurrentState.Content = "Find xTranslator. LockHandle:" + xTranslatorFormHwnd.ToString() + ". CalcOptAddress:" + xTranslatorOperateHwnd.ToString();
                                        OneAction.Fill = new SolidColorBrush(Color.FromRgb(255, 116, 198));
                                    }));
                                }
                                else
                                {

                                    CurrentState.Dispatcher.Invoke(new Action(() =>
                                    {
                                        CurrentState.Content = "Wait xTranslator Startup.";
                                        OneAction.Fill = new SolidColorBrush(Color.FromRgb(244, 154, 12));
                                    }));
                                }
                            }
                            else
                            {
                                if (IsReload)
                                {
                                    IsReload = false;
                                }
                                CurrentState.Dispatcher.Invoke(new Action(() =>
                                {
                                    CurrentState.Content = "Find xTranslator. LockHwnd:" + xTranslatorFormHwnd.ToString() + ".State: Stop";
                                    OneAction.Fill = new SolidColorBrush(Color.FromRgb(255, 116, 198));
                                }));
                            }

                        }
                        catch (Exception ex)
                        {

                        }
                    }

                }).Start();
            }
        }


        public static string[] SplitByLenth(string text, int length)  // 扩展方法: 按字数分割文本
        {
            int paragraphCount = (int)Math.Ceiling(((double)(text.Length) / length));
            string[] paragraphs = new string[paragraphCount];
            for (int i = 0; i < paragraphs.Length; i++)
            {
                paragraphs[i] = text.Substring(i * length, (text.Length - i * length > length ? length : text.Length - i * length));
            }
            return (paragraphs);
        }

        public static void SendBigText(IntPtr InputHwnd,string BigText)
        {
            List<string> Contents = SplitByLenth(BigText,100).ToList();

            foreach (var Get in Contents)
            {
                SendText(InputHwnd, Get);
                Thread.Sleep(5);
            }

        }

        #region SemiInt

        public static string AutoSetTranslatorContent()
        {
            IntPtr InputHwnd = new IntPtr();
            new FindMainWindows().SearchDeskTopWin("", "TForm2", ref InputHwnd);

            return FindxTranslatorFormHwndR(InputHwnd);
        }

        public static string FindxTranslatorFormHwndR(IntPtr CurrentHwnd)
        {
            if (IsWindowVisible(CurrentHwnd))
            {
                if (FristSetTransWin == 0)
                {
                    WinApi.MoveWindow(CurrentHwnd, 99999, 0, 100, 100, true);
                    FristSetTransWin = 1;
                }

                FindWindows NFindWindows = new FindWindows(CurrentHwnd, "TPanel", "Panel1", 5);
                NFindWindows = new FindWindows(NFindWindows.FoundHandle, "TPanel", "Panel3", 5);

                IntPtr GetRoot = NFindWindows.FoundHandle;

                FindWindows GetSendHwnd = new FindWindows(GetRoot, "TPanel", "", 5);
                GetSendHwnd = new FindWindows(GetSendHwnd.FoundHandle, "TPanel", "Panel4", 5);
                GetSendHwnd = new FindWindows(GetSendHwnd.FoundHandle, "TPanel", "Panel9", 5);
                var GetSourceSendHwnd = GetSendHwnd.SearchChildClassHwnd("TSynEdit");

                NFindWindows = new FindWindows(NFindWindows.FoundHandle, "TPanel", "Panel5", 5);
                //Panel5
                var GetSourceTextHwnd = NFindWindows.SearchChildClassHwnd("TSynEdit");

                if (GetSourceSendHwnd != IntPtr.Zero)
                {
                    if (GetSourceTextHwnd != IntPtr.Zero)
                    {
                        SendMethodR(CurrentHwnd, GetSourceTextHwnd, GetSourceSendHwnd);
                    }
                }
                //EnumChildWindows(NFindWindows.FoundHandle, new CallBack(GetTextBoxHwnd), 0);
                //NFindWindows = new FindWindows(NFindWindows.FoundHandle, "TPanel", "Panel5", 5);
                //EnumChildWindows(NFindWindows.FoundHandle, new CallBack(SendMethod), 0);

                return xTranslatorContent;
            }

            return null;
        }


        public static bool SendMethodR(IntPtr MainPostForm, IntPtr GetSourceTextHwnd, IntPtr GetSendHwnd)
        {
            LastPostWin = MainPostForm;
            xTranslatorContent = string.Empty;

            while (!WinApi.IsWindowVisible(MainPostForm))
            {
                Thread.Sleep(50);
            }
            Thread.Sleep(50);

            if (AutoShowXtransContentForm == 0)
            {
                AutoShowXtransContentForm++;

                try
                {
                    WinApi.MoveWindow(xTranslatorHelper.LastPostWin, 0, 0, 300, 180, true);
                }
                catch { }
            }

            if (GetTransSourceText(GetSourceTextHwnd, ref xTranslatorContent))
            {
                string GetCNTranslate = "";

                int GetKey = xTranslatorContent.GetHashCode();
                var GetData = TransDataHelper.SelectLibraryByName(TransTargetName);

                if (GetData.ContainsKey(GetKey))
                {
                    GetCNTranslate = GetData[GetKey].Result;
                }
                else
                {
                    List<EngineProcessItem> EngineProcessItems = new List<EngineProcessItem>();

                    GetCNTranslate = LanguageHelper.OptimizeString(new WordProcess().ProcessWords(ref EngineProcessItems, xTranslatorContent, BDLanguage.EN, BDLanguage.CN));
                }

                WinApi.SetForegroundWindow(MainPostForm);

                if (GetCNTranslate.Length < 100)
                {
                    SendText(GetSendHwnd, GetCNTranslate);
                    Thread.Sleep(50);
                }
                else
                {
                    SendBigText(GetSendHwnd,GetCNTranslate);
                    Thread.Sleep(50);
                }

                Thread.Sleep(50);

                while (WinApi.IsWindowVisible(MainPostForm))
                {
                    Thread.Sleep(50);
                    WinApi.SetForegroundWindow(MainPostForm);
                    Thread.Sleep(100);
                    WinApi.SendCtrlAndEnter(MainPostForm);
                    Thread.Sleep(50);
                }

                Thread.Sleep(50);

                WinApi.AnySendDown(xTranslatorOperateHwnd);
            }
            else
            {
             
            }
            return true;
        }

        #endregion

        public static int AutoShowXtransContentForm = 0;
        public static bool LockerAutoSemiInteractive = false;
        public static void StartAutoSemiInteractive(bool Check)
        {
            if (Check)
            {
                if (!LockerAutoSemiInteractive)
                {
                    LockerAutoSemiInteractive = true;

                    AutoShowXtransContentForm = 0;

                    new Thread(() =>
                    {

                        while (LockerAutoSemiInteractive)
                        {
                            Thread.Sleep(500);

                            while (xTranslatorHelper.StopAny)
                            {
                                Thread.Sleep(1000);
                            }

                            AutoSetTranslatorContent();
                        }

                    }).Start();
                }
            }
            else
            {
                LockerAutoSemiInteractive = false;
            }
        }

        public static bool SendText(IntPtr Hwnd, string NewContent)
        {
            int State = WinApi.AnySendMessageR(Hwnd, new StringBuilder(NewContent.Replace("|", " | ")));
            if (State != 0)
            {
                return true;
            }

            return false;
        }
        public static string FindxTranslatorFormContent(bool IsCache)
        {
            IntPtr InputHwnd = new IntPtr();
            new FindMainWindows().SearchDeskTopWin("", "TForm2", ref InputHwnd);

            return FindxTranslatorFormHwnd(InputHwnd, IsCache);
        }

        public static int MaxValue = 0;
        public static int RealXCount = 0;
        public static int CurrentCount = 0;



        public static void SelectTranslatorNextItem()
        {
            WinApi.SetForegroundWindow(xTranslatorFormHwnd);
            WinApi.AnySendDown(xTranslatorOperateHwnd);
        }




        public static bool IsGarbled(string Text)
        {
            // 判断韩语
            if (System.Text.RegularExpressions.Regex.IsMatch(Text, @"^[\uac00-\ud7ff]+$"))
            {
                return true;
            }

            // 判断日语
            if (System.Text.RegularExpressions.Regex.IsMatch(Text, @"^[\u0800-\u4e00]+$"))
            {
                return true;
            }

            // 判断中文
            if (System.Text.RegularExpressions.Regex.IsMatch(Text, @"^[\u4e00-\u9fa5]+$")) // 如果是中文
            {
                return true;
            }

            //[\u3400-\u4db5]
            Match TitleMatch = Regex.Match(Text, "[\u3400-\u4db5]", RegexOptions.IgnoreCase);

            if (ConvertHelper.ObjToStr(TitleMatch.Value).Length > 0)
            {
                return true;
            }

            TitleMatch = Regex.Match(Text, "[\u4e00-\u9fa5]", RegexOptions.IgnoreCase);

            if (ConvertHelper.ObjToStr(TitleMatch.Value).Length > 0)
            {
                return true;
            }
            //[\u4e00-\u9fa5]

            return false;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        public static string xTranslatorContent = "";

        public static int FristSetTransWin = 0;

        public static string FindxTranslatorFormHwnd(IntPtr CurrentHwnd, bool IsCache)
        {
            if (IsWindowVisible(CurrentHwnd))
            {
                if (FristSetTransWin == 0)
                {
                    WinApi.MoveWindow(CurrentHwnd, 99999, 0, 100, 100, true);
                    FristSetTransWin = 1;
                }

                FindWindows NFindWindows = new FindWindows(CurrentHwnd, "TPanel", "Panel1", 5);
                NFindWindows = new FindWindows(NFindWindows.FoundHandle, "TPanel", "Panel3", 5);

                IntPtr GetRoot = NFindWindows.FoundHandle;

                FindWindows GetSendHwnd = new FindWindows(GetRoot, "TPanel", "", 5);
                GetSendHwnd = new FindWindows(GetSendHwnd.FoundHandle, "TPanel", "Panel4", 5);
                GetSendHwnd = new FindWindows(GetSendHwnd.FoundHandle, "TPanel", "Panel9", 5);
                var GetSourceSendHwnd = GetSendHwnd.SearchChildClassHwnd("TSynEdit");

                NFindWindows = new FindWindows(NFindWindows.FoundHandle, "TPanel", "Panel5", 5);
                //Panel5
                var GetSourceTextHwnd = NFindWindows.SearchChildClassHwnd("TSynEdit");

                if (GetSourceSendHwnd != IntPtr.Zero)
                {
                    if (GetSourceTextHwnd != IntPtr.Zero)
                    {
                        SendMethod(CurrentHwnd, GetSourceTextHwnd, GetSourceSendHwnd, IsCache);
                    }
                }
                //EnumChildWindows(NFindWindows.FoundHandle, new CallBack(GetTextBoxHwnd), 0);
                //NFindWindows = new FindWindows(NFindWindows.FoundHandle, "TPanel", "Panel5", 5);
                //EnumChildWindows(NFindWindows.FoundHandle, new CallBack(SendMethod), 0);

                return xTranslatorContent;
            }

            return null;
        }

        public const int WM_GETTEXTLENGTH = 0x000E;
        public const int EM_LINEINDEX = 0xBB;
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public static bool GetTransSourceText(IntPtr TextBoxHwnd, ref string Text)
        {
            StringBuilder ClassName = new StringBuilder(2555);
            if (WinApi.GetClassName(TextBoxHwnd, ClassName, 2555) != 0)
            {
                if (ClassName.ToString().Equals("TSynEdit"))
                {
                    int TextLen = SendMessage(TextBoxHwnd, WM_GETTEXTLENGTH, 0, 0);
                    TextLen = (TextLen*2)+1;

                    IntPtr Buffer = Marshal.AllocHGlobal((TextLen));
                   
                    WinApi.SendMessageA(TextBoxHwnd, EM_LINEINDEX, (uint)TextLen, (IntPtr)0);
                    WinApi.SendMessageW2(TextBoxHwnd, WinApi.WM_GETTEXT, (uint)TextLen, Buffer);

                    string SourceText = Marshal.PtrToStringUni(Buffer);

                    Marshal.FreeHGlobal(Buffer);

                    if (SourceText.Trim().Length > 0)
                    {
                        if (!IsGarbled(SourceText))
                        {
                            Text = SourceText;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static string SelectClassName(IntPtr Hwnd)
        {
            StringBuilder Cache = new StringBuilder(256);
            FindMainWindows.GetClassNameW(Hwnd, Cache, Cache.Capacity);
            return Cache.ToString();
        }

        public static IntPtr LastPostWin = new IntPtr();
        public static bool SendMethod(IntPtr MainPostForm, IntPtr GetSourceTextHwnd, IntPtr GetSendHwnd, bool IsCache)
        {
            LastPostWin = MainPostForm;
            xTranslatorContent = string.Empty;

            while (!WinApi.IsWindowVisible(MainPostForm))
            {
                Thread.Sleep(50);
            }
            Thread.Sleep(50);

            if (GetTransSourceText(GetSourceTextHwnd, ref xTranslatorContent))
            {
                string GetCNTranslate = "";


                if (!IsCache)
                {
                    int GetKey = xTranslatorContent.GetHashCode();
                    var GetData = TransDataHelper.SelectLibraryByName(TransTargetName);

                    if (GetData.ContainsKey(GetKey))
                    {
                        GetCNTranslate = GetData[GetKey].Result;
                    }
                    else
                    {
                        Thread.Sleep(50);

                        while (WinApi.IsWindowVisible(MainPostForm))
                        {
                            WinApi.SetForegroundWindow(MainPostForm);
                            WinApi.AnySendESC(MainPostForm);
                            Thread.Sleep(50);
                        }

                        Thread.Sleep(50);
                        WinApi.SetForegroundWindow(xTranslatorOperateHwnd);
                        WinApi.AnySendDown(xTranslatorOperateHwnd);
                        Thread.Sleep(50);
                        WinApi.AnySendEnter(xTranslatorOperateHwnd);
                    }
                }

                WinApi.SetForegroundWindow(MainPostForm);

                if (GetCNTranslate.Length < 55)
                {
                    if (!IsCache)
                    {
                        SendText(GetSendHwnd, GetCNTranslate);
                        Thread.Sleep(50);
                    }

                }
                else
                {
                    if (!IsCache)
                    {
                        SendBigText(GetSendHwnd, GetCNTranslate);
                        Thread.Sleep(50);
                    }
                }


                if (IsCache)
                {
                    if (!IsGarbled(xTranslatorContent))
                    {
                        if (!new WordProcess().HasChinese(xTranslatorContent))
                        {
                            TransDataHelper.AddTransItem(TransTargetName, new TransRecvItem(0, 1, xTranslatorContent, string.Empty));
                        }
                    }

                    Thread.Sleep(50);

                    while (WinApi.IsWindowVisible(MainPostForm))
                    {
                        WinApi.SetForegroundWindow(MainPostForm);
                        WinApi.AnySendESC(MainPostForm);
                        Thread.Sleep(50);
                    }

                    Thread.Sleep(50);
                    WinApi.SetForegroundWindow(xTranslatorFormHwnd);
                    WinApi.AnySendDown(xTranslatorOperateHwnd);
                    Thread.Sleep(50);
                    WinApi.AnySendEnter(xTranslatorOperateHwnd);
                }
                else
                {
                    Thread.Sleep(50);

                    while (WinApi.IsWindowVisible(MainPostForm))
                    {
                        WinApi.SetForegroundWindow(MainPostForm);
                        WinApi.SendCtrlAndEnter(MainPostForm);
                        Thread.Sleep(50);
                    }

                    Thread.Sleep(50);
                    WinApi.SetForegroundWindow(xTranslatorOperateHwnd);
                    WinApi.AnySendDown(xTranslatorOperateHwnd);
                    Thread.Sleep(50);
                    WinApi.AnySendEnter(xTranslatorOperateHwnd);
                }

                return false;
            }
            else
            {
                Thread.Sleep(50);

                while (WinApi.IsWindowVisible(MainPostForm))
                {
                    WinApi.SetForegroundWindow(MainPostForm);
                    WinApi.AnySendESC(MainPostForm);
                    Thread.Sleep(50);
                }

                Thread.Sleep(50);
                WinApi.SetForegroundWindow(xTranslatorOperateHwnd);
                WinApi.AnySendDown(xTranslatorOperateHwnd);
                Thread.Sleep(50);
                WinApi.AnySendEnter(xTranslatorOperateHwnd);
            }
            return true;
        }

        public static bool FindMainHwnd(ref IntPtr WinHwnd, ref IntPtr LockerHwnd)
        {
            IntPtr MainHwnd = IntPtr.Zero;
            new FindMainWindows().SearchDeskTopWin("xTranslator", "TForm1", ref MainHwnd);

            WinHwnd = MainHwnd;

            if (MainHwnd != IntPtr.Zero)
            {
                FindWindows NFindWindows = new FindWindows(MainHwnd, "TPanel", "", 5);
                NFindWindows = new FindWindows(NFindWindows.FoundHandle, "TPageControl", "", 5);
                var GetParentHwnd = NFindWindows.SearchChildClassHwnd("STRINGS");

                StringBuilder Title = new StringBuilder(255);
                WinApi.GetWindowText((int)GetParentHwnd, Title, 255);
                string GetTittle = Title.ToString();

                if (GetTittle.Contains("/"))
                {
                    string GetProcess = ConvertHelper.StringDivision(GetTittle, "[", "]");

                    int GetMaxValue = ConvertHelper.ObjToInt(GetProcess.Split('/')[0]);

                    if (MaxValue != GetMaxValue)
                    {
                        MaxValue = GetMaxValue;
                        RealXCount = ConvertHelper.ObjToInt(GetProcess.Split('/')[1]);
                        CurrentCount = 0;
                    }
                }

                FindWindows GetInputHwnd = new FindWindows(GetParentHwnd, "TVirtualStringTree", "", 5);
                LockerHwnd = GetInputHwnd.FoundHandle;
                return true;
            }

            return false;

        }

        public static void GetxTranslatorInFo()
        {
            xTranslatorHelper.FindMainHwnd(ref xTranslatorFormHwnd, ref xTranslatorOperateHwnd);

            foreach (var Get in new FindWindows().FindChildClassHwnds("TPanel", xTranslatorFormHwnd))
            {
                foreach (var GetCombox in new FindWindows().FindChildClassHwnds("TComboBox", xTranslatorFormHwnd))
                {
                    int MaxLength = 999;

                    IntPtr Buffer = Marshal.AllocHGlobal((MaxLength + 1) * 2);

                    WinApi.SendMessageA(GetCombox, EM_LINEINDEX, (uint)MaxLength, (IntPtr)0);
                    WinApi.SendMessageW2(GetCombox, WinApi.WM_GETTEXT, (uint)MaxLength, Buffer);

                    string SourceText = Marshal.PtrToStringUni(Buffer);

                    Marshal.FreeHGlobal(Buffer);

                    if (SourceText.Trim().Length > 0)
                    {
                        if (SourceText.Contains("."))
                        {
                            string GetCaption = "";
                            if (SourceText.Contains("]"))
                            {
                                GetCaption = SourceText.Split(']')[1];
                            }
                            else
                            {
                                GetCaption = SourceText;
                            }

                            TransTargetName = GetCaption;

                            return;
                        }
                    }
                }
            }
        }


        public static int WaitTransCount = 0;
        public static int ScanCount = 0;
        public static int CurrentStep = 0;


        public static void KillScanThread()
        {
            if (SCanThread != null)
            {
                WinApi.MoveWindow(LastPostWin, 0, 0, 300, 180, true);

                LastPostWin = IntPtr.Zero;

                try
                {
                    SCanThread.Abort();
                    SCanThread = null;
                }
                catch { }


                CurrentStep = 0;
                ScanCount = 0;
                WaitTransCount = 0;

                TransDataHelper.TransItemArrays.Clear();


            }
        }



        private static Thread SCanThread = null;

        public static void SCanTarget(System.Windows.Controls.ProgressBar OneProcess)
        {
            if (SCanThread != null) return;

            if (ScanCount == 0)
            {
                CurrentStep = 0;

                SCanThread = new Thread(() =>
                {
                    ScanCount++;

                    FristSetTransWin = 0;
                    TransDataHelper.AddTransLibrary(TransTargetName);

                    OneProcess.Dispatcher.Invoke(new Action(() =>
                    {
                        OneProcess.Maximum = MaxValue * 2;
                        OneProcess.Value = 0;
                    }));

                    WaitTransCount = MaxValue;

                    Thread.Sleep(1000);

                    for (int i = 0; i < MaxValue; i++)
                    {
                        while (xTranslatorHelper.StopAny)
                        {
                            Thread.Sleep(1000);
                        }

                        WinApi.SetForegroundWindow(xTranslatorFormHwnd);
                        WinApi.AnySendUP(xTranslatorOperateHwnd);

                        Thread.Sleep(5);
                    }

                    Thread.Sleep(1000);

                    WinApi.SetForegroundWindow(xTranslatorFormHwnd);
                    WinApi.AnySendEnter(xTranslatorOperateHwnd);

                    Thread.Sleep(200);

                    int CreatOffset = 0;

                    CreatOffset = (MaxValue + 1);

                    while (CreatOffset > 0)
                    {
                        while (xTranslatorHelper.StopAny)
                        {
                            Thread.Sleep(1000);
                        }

                        Thread.Sleep(100);

                        var GetValue = FindxTranslatorFormContent(true);

                        Thread.Sleep(100);

                        if (GetValue != null)
                            if (GetValue.Trim().Length > 0)
                            {
                                CreatOffset--;
                                OneProcess.Dispatcher.Invoke(new Action(() =>
                                {
                                    OneProcess.Value++;
                                }));
                            }
                    }

                    FindxTranslatorFormContent(true);
        
                    Thread.Sleep(200);

                    new SoundPlayer().PlaySound(Sounds.第一阶段完成);

                    Thread.Sleep(500);
                    WinApi.SetForegroundWindow(LastPostWin);
                    WinApi.AnySendESC(LastPostWin);
                   
                    for (int i = 0; i < ((RealXCount + 1) + (RealXCount - MaxValue)) + 5; i++)
                    {
                        while (xTranslatorHelper.StopAny)
                        {
                            Thread.Sleep(1000);
                        }

                        WinApi.SetForegroundWindow(xTranslatorFormHwnd);
                        WinApi.AnySendUP(xTranslatorOperateHwnd);
                        Thread.Sleep(5);
                    }

                    CurrentStep = 1;

                    while (WaitTransCount > 0)
                    {
                        Thread.Sleep(1000);
                    }

                    Thread.Sleep(500);

                    if (ActionWin.Show("准备开始写入", "准备开始执行写入,确认翻译内容无问题后点击确认即可!", MsgAction.YesNo, MsgType.Info) > 0)
                    {
                        WinApi.SetForegroundWindow(xTranslatorFormHwnd);
                        WinApi.AnySendUP(xTranslatorOperateHwnd);
                        WinApi.AnySendEnter(xTranslatorOperateHwnd);

                        CreatOffset = (MaxValue + 1);

                        while (CreatOffset > 0)
                        {
                            while (xTranslatorHelper.StopAny)
                            {
                                Thread.Sleep(1000);
                            }
                            var GetValue = FindxTranslatorFormContent(false);

                            Thread.Sleep(100);



                            if (GetValue != null)
                                if (GetValue.Trim().Length > 0)
                                {
                                    CreatOffset--;
                                    OneProcess.Dispatcher.Invoke(new Action(() =>
                                    {
                                        OneProcess.Value++;
                                    }));
                                }

                        }

                        Thread.Sleep(100);
                        FindxTranslatorFormContent(false);
                        Thread.Sleep(100);

                        CurrentStep = 2;

                        new SoundPlayer().PlaySound(Sounds.第二阶段完成);

                        if (ActionWin.Show("字段文件存储确认", string.Format("是否更新字典文件于{0}目录下!", DeFine.GetFullPath(@"\Dictionary\" + TransTargetName + ".db")), MsgAction.YesNo, MsgType.Info) > 0)
                        {
                            for (int i = 0; i < TransDataHelper.TransItemArrays.Count; i++)
                            {
                                if (TransDataHelper.TransItemArrays[i].SourceName.Equals(TransTargetName))
                                {
                                    foreach (var Get in TransDataHelper.TransItemArrays[i].TransRecvItems)
                                    {
                                        TransDataHelper.AddTransLine(TransDataHelper.TransItemArrays[i].CurrentDB, new DictionaryItem(Get.Value.Text, Get.Value.Result, 0, 1));
                                    }
                                }
                            }
                        }
                    }

                    Thread.Sleep(5000);

                    WinApi.MoveWindow(LastPostWin, 0, 0, 300, 180, true);

                    ScanCount--;

                    DeFine.WorkWin.Dispatcher.Invoke(new Action(() =>
                    {
                        DeFine.WorkWin.KillTransProcessTrd(null, null);
                    }));

                });
                SCanThread.Start();
            }
        }
    }
}
