using Microsoft.VisualBasic;
using Microsoft.WindowsAPICodePack.Dialogs;
using MS.WindowsAPICodePack.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using YDSkyrimTools.ConvertManager;
using YDSkyrimTools.FormManager;
using YDSkyrimTools.HotKeyManager;
using YDSkyrimTools.RequestCore;
using YDSkyrimTools.SkyrimEnbManager;
using YDSkyrimTools.SkyrimModManager;
using YDSkyrimTools.TranslateCore;
using YDSkyrimTools.xTranslatorManager;
using static System.Net.Mime.MediaTypeNames;

namespace YDSkyrimTools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public YDListView YDMsgView;

  
        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox OneText = sender as TextBox;
            Clipboard.SetText(OneText.Text);
            MessageBox.Show("以复制到剪切板!");
        }

        protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
           

            return IntPtr.Zero;
        }

        public IntPtr CurrentWndProcHandle;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //DeFine.Init(this);

            new YDGUI().Show();
            //var Lines = EnbHelper.EqualsAndMergeEnbConfig("DefRudyEnb", "YDRudyEnb");

            //string GetZH = BaiDuRequest.GetFreeTranslation(" you can find here!");
            CurrentWndProcHandle = new WindowInteropHelper(this).Handle;
            HwndSource Source = HwndSource.FromHwnd(CurrentWndProcHandle);
            Source.AddHook(new HwndSourceHook(WndProc));

            HotKeyHelper.Init(CurrentWndProcHandle);
           
            YDMsgView = new YDListView(this.TranslateMsg, 35);

            YDMsgView.ExColStyles.Add(new ExColStyle(180, GridUnitType.Pixel));
            YDMsgView.ExColStyles.Add(new ExColStyle(130, GridUnitType.Pixel));
            YDMsgView.ExColStyles.Add(new ExColStyle(2, GridUnitType.Star));

         
            EnbHelper.Init();
            ConjunctionHelper.Init();
            EnbHelper.SendAnyMsg += AnyMsg;

            //xTranslatorHelper.StartListenService(CurrentState,CurrentData);

            SkyrimPath.Text = DeFine.GlobalLocalSetting.SkyrimPath;
            APath.Text = DeFine.GlobalLocalSetting.APath;
            BPath.Text = DeFine.GlobalLocalSetting.BPath;

            TranslateType.Items.Add("MCM配置文件");
            TranslateType.Items.Add("Apropos2(可能会翻车)");

            //string GetContent = EnbHelper.CreatEnbConfig(EnbHelper.EqualsAndMergeEnbConfig("RudyEnbYD", "ReENB"));
            //GC.Collect();

            this.Dispatcher.Invoke(new Action(() => {
                this.Title = string.Format("SkyrimTools Version : {0}", DeFine.CurrentVersion);
            }));

            new Thread(() => 
            {
                while (true)
                {
                    Thread.Sleep(3000); 
                    this.Dispatcher.Invoke(new Action(() => {
                        this.Title = string.Format("SkyrimTools Version : {0},百度翻译Api使用字数:{1}", DeFine.CurrentVersion,DeFine.GlobalLocalSetting.TransCount);
                    }));
                }
            }).Start();
        }


        public void AnyMsg(string Msg)
        {
            AnyMsgL.Items.Add(Msg);
        }
        public void StartModCheck()
        {
            ModCheckReport.Items.Clear();
            ModCheckReport.Items.Add("正在处理中");

            new Thread(() =>
            {
                string VAPath = "";
                string VBPath = "";

                this.Dispatcher.Invoke(new Action(() =>
                {
                    VAPath = APath.Text;
                    VBPath = BPath.Text;
                }));

                var GetAStruct = FileHelper.GetPathStruct(VAPath);
                GetAStruct.Files.Sort((x, y) => -x.CompareTo(y));

                var GetBStruct = FileHelper.GetPathStruct(VBPath);
                GetBStruct.Files.Sort((x, y) => -x.CompareTo(y));


                this.Dispatcher.Invoke(new Action(() =>
                {
                    ModCheckReport.Items.Add(string.Format("A目录签名:{0},B目录签名:{1}", GetAStruct.Files.GetHashCode(), GetBStruct.Files.GetHashCode()));
                }));

                List<ErrorReport> ErrorFiles = new List<ErrorReport>();
                ModHelper.CheckModStruct(GetAStruct, GetBStruct,ref ErrorFiles);


                foreach (var GetErr in ErrorFiles)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        ModCheckReport.Items.Add(string.Format("{0}->{1}", GetErr.Error, GetErr.FilePath));
                    }));
                }
               
                MessageBox.Show("对比差异结束!");

            }).Start();
        }



        public string ObjToStr(object Any)
        {
            if (Any != null)
            {
                return Any.ToString();
            }
            return string.Empty;
        }

        private void SelectNav(object sender, MouseButtonEventArgs e)
        {
            if (sender is Label)
            {
                Label LockerLab = (Label)sender;
                switch (ObjToStr(LockerLab.Content))
                {
                    case "对比":
                        {
                            DeFine.About.Hide();
                            StartModCheck();
                        }
                        break;
                    case "对比Mod差异":
                        {
                            DeFine.About.Hide();
                            CheckModView.Visibility = Visibility.Visible;
                            EnbManagementView.Visibility = Visibility.Hidden;
                            TranslateView.Visibility = Visibility.Hidden;
                        }
                        break;
                    case "Enb管理":
                        {
                            DeFine.About.Hide();
                            CheckModView.Visibility = Visibility.Hidden;
                            EnbManagementView.Visibility = Visibility.Visible;
                            TranslateView.Visibility = Visibility.Hidden;

                            Enbs.Items.Clear();

                            //foreach (var Get in EnbHelper.GetENBs())
                            //{
                            //    Enbs.Items.Add(Get);
                            //}
                        }
                        break;
                    case "翻译核心":
                        {
                            DeFine.About.Hide();
                            ConjunctionHelper.Init();
                            CheckModView.Visibility = Visibility.Hidden;
                            EnbManagementView.Visibility = Visibility.Hidden;
                            TranslateView.Visibility = Visibility.Visible;
                        }
                        break;
                    case "安装":
                        {
                            AnyMsgL.Items.Clear();
                            if (EnbHelper.UsingEnb(ObjToStr(Enbs.SelectedItem), SkyrimPath.Text))
                            {
                                MessageBox.Show("以安装!");
                            }
                        }
                        break;
                    case "卸载":
                        {
                            if (File.Exists(SkyrimPath.Text + @"\ENB.config"))
                            {
                                if (EnbHelper.CancelEnb(FileHelper.ReadFileByStr(SkyrimPath.Text + @"\ENB.config", Encoding.UTF8), SkyrimPath.Text))
                                {
                                    MessageBox.Show("卸载成功!");
                                    File.Delete(SkyrimPath.Text + @"\ENB.config");//最后在移除配置文件
                                }
                                else
                                {
                                    MessageBox.Show("卸载失败!");
                                }
                            }
                            else
                            {
                                MessageBox.Show("不需要卸载!");
                            }
                        }
                        break;

                    case "...":
                        {
                            CommonOpenFileDialog Dialog = new CommonOpenFileDialog();
                            Dialog.IsFolderPicker = true;   //设置为选择文件夹
                            if (Dialog.ShowDialog() == CommonFileDialogResult.Ok)
                            {
                                EnbPath.Text = Dialog.FileName;
                            }
                        }
                        break;
                    case "导入":
                        {
                            if (Directory.Exists(EnbPath.Text))
                            {
                                var GetFiles = Directory.GetFiles(EnbPath.Text);

                                bool IsEnb = false;

                                foreach (var GetPath in GetFiles)
                                {
                                    if (GetPath.Contains("enbseries.ini"))
                                    {
                                        IsEnb = true;
                                        break;
                                    }
                                }
                                if (IsEnb)
                                {
                                    string GetEnbName = Interaction.InputBox("请输入Enb名称");
                                    EnbHelper.ImportEnb(EnbPath.Text, GetEnbName);
                                }
                                else
                                {
                                    MessageBox.Show("这个文件夹不是ENB文件或者不是根文件夹请确保文件夹根目录包含enbseries.ini!");
                                }
                              
                            }
                            else
                            {
                                MessageBox.Show("请先选择enb所在文件夹!");
                            }
                        }
                        break;

                    case "选择翻译":
                        {
                            DeFine.GlobalLocalSetting.SaveConfig();

                            int AutoType = 0;

                            if (ConvertHelper.ObjToStr(TranslateType.SelectedValue).Equals("Apropos2(可能会翻车)"))
                            {
                                YDMsgView.Clear();

                                string GetPath = "";
                                CommonOpenFileDialog Dialog = new CommonOpenFileDialog();
                                Dialog.IsFolderPicker = true;   //设置为选择文件夹
                                if (Dialog.ShowDialog() == CommonFileDialogResult.Ok)
                                {
                                    GetPath = Dialog.FileName;
                                }
                                if (GetPath.Contains(@"\Apropos\db"))
                                {
                                    AproposHelper.TranslatePath(TranslatePercent, GetPath);
                                    AutoType = 1;
                                }
                                else
                                {
                                    MessageBox.Show("请选择Mod根目录Apropos文件夹下的DB文件!");
                                }
                            }
                            else
                            {
                                string GetPath = DataHelper.ShowFileDialog("所有文件(*.*)|*.*", "选择源文件");
                                YDMsgView.Clear();
                                SucessText.Text = "";
                                //TranslateHelper.StartTranslateService(TranslatePercent, SucessText, GetPath, AutoType);
                            }
                        }
                        break;
                    case "终止翻译":
                        {
                            //if (TranslateHelper.CurrentThread != null)
                            //{
                            //    try {
                            //        TranslateHelper.CurrentThread.Abort();
                            //        TranslateHelper.CurrentThread = null;
                            //    } catch { }
                            //}
                        }
                        break;
                    case "引擎控制":
                        {
                            DeFine.EngineView.ActiveThis();
                            DeFine.EngineView.Top = (this.Top + DeFine.EngineView.Height) + 10;
                            DeFine.EngineView.Left = this.Left;
                        }
                        break;
                    case "配置百度API":
                        {
                            string AppID = Interaction.InputBox("Step1 请输入AppID", "AppID", DeFine.GlobalLocalSetting.BaiDuAppID, -1, -1);
                            DeFine.GlobalLocalSetting.BaiDuAppID = AppID;
                            string SecretKey = Interaction.InputBox("Step2 请输入SecretKey", "SecretKey", DeFine.GlobalLocalSetting.BaiDuSecretKey, -1, -1);
                            DeFine.GlobalLocalSetting.BaiDuSecretKey = SecretKey;

                            DeFine.GlobalLocalSetting.SaveConfig();
                        }
                        break;
                    case "获取剪切板内容":
                        {
                            FromText.Text = Clipboard.GetText();
                        }
                        break;
                    case "手动翻译":
                        {
                           //ToText.Text = WordProcess.ProcessWords(FromText.Text, BDLanguage.EN, BDLanguage.CN);
                        }
                        break;
                    case "关于作者":
                        {
                            DeFine.About.Show();
                            DeFine.About.Topmost = true;
                        }
                        break;
                    case "暂时停止":
                        {
                            xTranslatorHelper.StopAny = true;
                        }
                        break;
                    case "恢复交互":
                        {
                            xTranslatorHelper.StopAny = false;
                        }
                        break;

                    case "系统设定":
                        {
                          
                        }
                        break;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HotKeyHelper.UnregisterHotkeys(CurrentWndProcHandle);

            DeFine.GlobalLocalSetting.APath = APath.Text;
            DeFine.GlobalLocalSetting.BPath = BPath.Text;
            DeFine.GlobalLocalSetting.SkyrimPath = SkyrimPath.Text;

            DeFine.GlobalLocalSetting.SaveConfig();

            System.Environment.Exit(0);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            DeFine.ExitAny();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
