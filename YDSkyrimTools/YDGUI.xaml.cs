using Microsoft.VisualBasic;
using Microsoft.WindowsAPICodePack.Dialogs;
using SharpVectors.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using YDSkyrimTools.ConvertManager;
using YDSkyrimTools.FormManager;
using YDSkyrimTools.RequestCore;
using YDSkyrimTools.SkyrimModManager;
using YDSkyrimTools.SqlManager;
using YDSkyrimTools.TranslateCore;
using YDSkyrimTools.UIManager;
using YDSkyrimTools.xTranslatorManager;
using static YDSkyrimTools.UIManager.UIHelper;

namespace YDSkyrimTools
{
    /// <summary>
    /// Interaction logic for YDGUI.xaml
    /// </summary>
    public partial class YDGUI : Window
    {
        public YDGUI()
        {
            InitializeComponent();
        }

        #region PageInFo

        public string CurrentTabName = "";

        public void AutoReload()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                switch (ConvertHelper.ObjToStr(CurrentTabName))
                {

                    case "当前字典":
                        {
                            FindWaitTransList();
                        }
                        break;

                }
            }));
        }

        public int CurrentPageNo = 0;
        public int CurrentMaxPage = 0;

        private void GotoIndex(object sender, MouseButtonEventArgs e)
        {
            CurrentPageNo = 0;

            AutoReload();
        }

        public void Previous(object sender, MouseButtonEventArgs e)
        {
            if (CurrentPageNo > 0)
            {
                CurrentPageNo--;

                AutoReload();
            }
        }

        private void Next(object sender, MouseButtonEventArgs e)
        {
            if (CurrentMaxPage > CurrentPageNo)
            {
                CurrentPageNo++;

                AutoReload();
            }
        }

        private void GotoEnd(object sender, MouseButtonEventArgs e)
        {
            CurrentPageNo = CurrentMaxPage;

            AutoReload();
        }


        #endregion

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            AutoSetCGView();
            AutoLocationByHistoryLayer();
        }

        public bool IsLeftMouseDown = false;

        private void WinHead_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                IsLeftMouseDown = true;
            }

            if (IsLeftMouseDown)
            {
                try
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        this.DragMove();
                    }));

                    IsLeftMouseDown = false;

                    AutoLocationByCGView();
                }
                catch { }
            }
        }


        private void RMouserEffectByEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border)
            {
                Border LockerGrid = sender as Border;
                if (LockerGrid.Child != null)
                {
                    if (LockerGrid.Child is Image)
                    {
                        Image LockerImg = LockerGrid.Child as Image;
                        LockerImg.Opacity = 0.9;
                    }
                    LockerGrid.Background = new SolidColorBrush(Color.FromRgb(244, 101, 155));
                }
            }
        }

        private void RMouserEffectByLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border)
            {
                Border LockerGrid = sender as Border;
                if (LockerGrid.Child != null)
                {
                    if (LockerGrid.Child is Image)
                    {
                        Image LockerImg = LockerGrid.Child as Image;
                        LockerImg.Opacity = 0.6;
                    }
                    LockerGrid.Background = new SolidColorBrush(Color.FromRgb(247, 127, 172));
                }
            }
        }

        private void MinThis(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        public double DefTop = 0;
        public double DefLeft = 0;

        public double DefWidth = 0;
        public double DefHeight = 0;
        public int SizeChangeState = 0;

        private void MaxThis(object sender, MouseButtonEventArgs e)
        {
            if (SizeChangeState == 0)
            {
                DefWidth = this.Width;
                DefHeight = this.Height;

                DefTop = this.Top;
                DefLeft = this.Left;

                this.Width = SystemParameters.WorkArea.Width - 50;
                this.Height = SystemParameters.WorkArea.Height - 50;

                this.Top = 25;
                this.Left = 25;

                SizeChangeState = 1;
            }
            else
            {
                this.Width = DefWidth;
                this.Height = DefHeight;

                this.Top = DefTop;
                this.Left = DefLeft;

                SizeChangeState = 0;
            }
        }

        private void ExitThis(object sender, MouseButtonEventArgs e)
        {
            if (NeedSaveSetting)
            {
                DeFine.GlobalLocalSetting.SaveConfig();
                NeedSaveSetting = false;
            }

            this.Hide();
            Environment.Exit(-1);
        }
        public void AutoLocationByCGView()
        {
            
        }
        public void AutoLocationByHistoryLayer()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (HistoryToolViewState != 0)
                    if (!DeFine.HistoryToolView.IsLock)
                    {
                        DeFine.HistoryToolView.Width = (this.Width);
                        DeFine.HistoryToolView.Height = this.Height;
                        DeFine.HistoryToolView.Left = this.Left + this.Width + 10;
                        DeFine.HistoryToolView.Top = this.Top;

                        DeFine.HistoryToolView.Show();
                    }
            }));
        }

        public YDTablePanelControl TransTable = null;
        public YDTablePanelControl ModsTable = null;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DeFine.Init(this);

            xTranslatorHelper.StopAny = true;

            UIHelper.Init(this);

            AutoLocationByHistoryLayer();
            AutoLocationByCGView();

            xTranslatorHelper.StartListenService(OneAction, OneActionMsg, CurrentData);
            WordProcess.SendTranslateMsg += AnyTranslateMsg;

            TranslateType.Items.Clear();
            TranslateType.Items.Add("xTranslator(交互式)");
            TranslateType.Items.Add("xTranslator(半交互式)");
            TranslateType.Items.Add("MCM(配置文件)");
            TranslateType.Items.Add("Apropos2(可能会翻车)");
            TranslateType.SelectedValue = TranslateType.Items[0];

            TransTable = new YDTablePanelControl(TransTabelPanel);
            ModsTable = new YDTablePanelControl(ModTablePanel);

            AnyTransTableClick(TransLogCap, null);

            this.Show();
        }

        private void SelectEngine(object sender, MouseButtonEventArgs e)
        {
            string GetContent = ConvertHelper.ObjToStr((sender as SvgViewbox).ToolTip);

            double GetOpacity = ConvertHelper.ObjToDouble((sender as SvgViewbox).Opacity);

            bool OneState = false;

            if (GetOpacity == 1)
            {
                OneState = false;
                (sender as SvgViewbox).Opacity = 0.1;
            }
            else
            {
                OneState = true;
                (sender as SvgViewbox).Opacity = 1;
            }

            switch (GetContent)
            {
                case "本地词组引擎":
                    {
                        DeFine.PhraseEngineUsing = OneState;
                    }
                    break;
                case "代码识别引擎":
                    {
                        DeFine.CodeParsingEngineUsing = OneState;
                    }
                    break;
                case "本地连词引擎":
                    {
                        DeFine.ConjunctionEngineUsing = OneState;
                    }
                    break;
                case "百度云翻译":
                    {
                        DeFine.BaiDuYunApiUsing = OneState;
                    }
                    break;
                case "有道云翻译":
                    {
                        DeFine.YouDaoYunApiUsing = OneState;
                    }
                    break;
                case "谷歌云翻译":
                    {
                        DeFine.GoogleYunApiUsing = OneState;
                    }
                    break;
                case "自定义引擎":
                    {
                        DeFine.DivCacheEngineUsing = OneState;
                    }
                    break;
            }
        }

        private void WinChangeSize(object sender, SizeChangedEventArgs e)
        {
            AutoLocationByCGView();
            AutoLocationByHistoryLayer();

            TranslatePercent.Width = this.Width - Nav.ActualWidth - 150;
        }

        private void AutoStartTranslate(object sender, MouseButtonEventArgs e)
        {
            string GetSelectValue = ConvertHelper.ObjToStr(TranslateType.SelectedValue);
            switch (GetSelectValue)
            {
                case "xTranslator(半交互式)":
                    {
                        new Thread(() =>
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                CurrentFileName.Content = "Find Address...";
                            }));

                            xTranslatorHelper.StopAny = false;

                            Thread.Sleep(500);

                            WinApi.SetForegroundWindow(xTranslatorHelper.xTranslatorFormHwnd);
                            xTranslatorHelper.GetxTranslatorInFo();

                            Thread.Sleep(500);

                            if (xTranslatorHelper.TransTargetName.Trim().Length > 0)
                            {
                                if (xTranslatorHelper.MaxValue <= 0)
                                {
                                    xTranslatorHelper.StopAny = true;
                                    ActionWin.Show("xTranslator异常", "没有可供翻译的字段!", MsgAction.Yes, MsgType.Waring);
                                }
                                else
                                {
                                    xTranslatorHelper.StartAutoSemiInteractive(true);

                                    this.Dispatcher.Invoke(new Action(() =>
                                    {
                                        StartTransBtn.Visibility = Visibility.Collapsed;
                                        ProcessStateBackGround.Background = new SolidColorBrush(Color.FromRgb(147, 85, 108));
                                        AutoKeep.Source = SVGHelper.ShowSvg(ICOResources.Stop);
                                        AutoKeep.ToolTip = "停止";

                                        CurrentFileName.Content = xTranslatorHelper.TransTargetName;
                                        TranslateType.IsEnabled = false;
                                        TranslateType.Opacity = 0.5;
                                        this.Show();
                                        this.Focus();
                                    }));
                                }
                            }
                            else
                            {
                                xTranslatorHelper.StopAny = true;
                                ActionWin.Show("xTranslator异常", "xTranslator没有加载需要翻译的文件!", MsgAction.Yes, MsgType.Waring);
                            }

                        }).Start();
                    }
                    break;
                case "xTranslator(交互式)":
                    {
                        new Thread(() =>
                        {
                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                CurrentFileName.Content = "Find Address...";
                            }));

                            xTranslatorHelper.StopAny = false;

                            Thread.Sleep(500);

                            WinApi.SetForegroundWindow(xTranslatorHelper.xTranslatorFormHwnd);
                            xTranslatorHelper.GetxTranslatorInFo();

                            Thread.Sleep(500);

                            if (xTranslatorHelper.TransTargetName.Trim().Length > 0)
                            {
                                if (xTranslatorHelper.MaxValue <= 0)
                                {
                                    xTranslatorHelper.StopAny = true;
                                    ActionWin.Show("xTranslator异常", "没有可供翻译的字段!", MsgAction.Yes, MsgType.Waring);
                                }
                                else
                                {
                                    new SoundPlayer().PlaySound(Sounds.开始执行翻译);
                                    xTranslatorHelper.SCanTarget(TranslatePercent);

                                    this.Dispatcher.Invoke(new Action(() =>
                                    {
                                        StartTransBtn.Visibility = Visibility.Collapsed;
                                        ProcessStateBackGround.Background = new SolidColorBrush(Color.FromRgb(147, 85, 108));
                                        AutoKeep.Source = SVGHelper.ShowSvg(ICOResources.Stop);
                                        AutoKeep.ToolTip = "停止";

                                        CurrentFileName.Content = xTranslatorHelper.TransTargetName;
                                        TranslateType.IsEnabled = false;
                                        TranslateType.Opacity = 0.5;
                                        this.Show();
                                        this.Focus();
                                    }));
                                }
                            }
                            else
                            {
                                xTranslatorHelper.StopAny = true;
                                ActionWin.Show("xTranslator异常", "xTranslator没有加载需要翻译的文件!", MsgAction.Yes, MsgType.Waring);
                            }

                        }).Start();
                    }
                    break;

                case "MCM(配置文件)":
                    {
                        string GetPath = DataHelper.ShowFileDialog("所有文件(*.*)|*.*", "选择源文件");
                        TransDataHelper.StartTransServiceByText(TranslatePercent, GetPath);

                        new Thread(() =>
                        {
                            Thread.Sleep(1000);

                            CurrentFileName.Dispatcher.Invoke(new Action(() =>
                            {
                                CurrentFileName.Content = xTranslatorHelper.TransTargetName;
                            }));

                            this.Dispatcher.Invoke(new Action(() =>
                            {
                                StartTransBtn.Visibility = Visibility.Collapsed;
                                ProcessStateBackGround.Background = new SolidColorBrush(Color.FromRgb(147, 85, 108));
                                AutoKeep.Source = SVGHelper.ShowSvg(ICOResources.Stop);
                                AutoKeep.ToolTip = "停止";

                                CurrentFileName.Content = xTranslatorHelper.TransTargetName;
                                TranslateType.IsEnabled = false;
                                TranslateType.Opacity = 0.5;
                                this.Show();
                                this.Focus();
                            }));
                        }).Start();
                    }
                    break;
                case "Apropos2(可能会翻车)":
                    {

                    }
                    break;
            }

        }

        private void TransChange(object sender, SelectionChangedEventArgs e)
        {
            xTranslatorHelper.StopAny = true;
            CurrentFileName.Content = string.Empty;
        }

        public YDListView WaitTransListMsgs = null;

        private void AnyTransTableClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Label)
            {
                Label LockerLab = sender as Label;
                CurrentTabName = ConvertHelper.ObjToStr(LockerLab.Content);

                switch (ConvertHelper.ObjToStr(LockerLab.Content))
                {
                    case "当前字典":
                        {
                            new SoundPlayer().PlaySound(Sounds.当前字典);
                            TransTable.ChangeTableEffect(0, new Action(() =>
                            {
                                if (WaitTransListMsgs == null)
                                {
                                    WaitTransListMsgs = new YDListView(WaitTransList, 45);
                                    WaitTransListMsgs.ExColStyles.Add(new ExColStyle(1, GridUnitType.Star));
                                }
                                WaitTransView.Visibility = Visibility.Visible;
                                TransLogView.Visibility = Visibility.Hidden;
                                AutoReload();
                            }));
                        }
                        break;
                    case "实时信息":
                        {
                            new SoundPlayer().PlaySound(Sounds.实时信息);
                            TransTable.ChangeTableEffect(1, new Action(() =>
                            {
                                TransLogView.Visibility = Visibility.Visible;
                                WaitTransView.Visibility = Visibility.Hidden;
                            }));
                        }
                        break;
                }
            }
        }

        private void ClearTransCache(object sender, MouseButtonEventArgs e)
        {
            if (ActionWin.Show("您确定要清理以翻译的缓存？", "警告清理后所有包括以前翻译的内容不在缓存,再次翻译会重新从云端获取,会浪费您以前翻译的结果,会增加字数消耗量!", MsgAction.YesNo, MsgType.Info) > 0)
            {
                string SqlOrder = "Delete From CloudTranslation Where 1=1";
                int State = DeFine.GlobalDB.ExecuteNonQuery(SqlOrder);
                if (State != 0)
                {
                    ActionWin.Show("数据库事务", "Done!", MsgAction.Yes, MsgType.Info);
                    DeFine.GlobalDB.ExecuteNonQuery("vacuum");
                }
                else
                {

                }
            }
            else
            {

            }
        }

        private void ChangeTransProcessState(object sender, MouseButtonEventArgs e)
        {
            if (xTranslatorHelper.StopAny == true)
            {
                ProcessStateBackGround.Background = new SolidColorBrush(Color.FromRgb(147, 85, 108));

                xTranslatorHelper.StopAny = false;
                AutoKeep.Source = SVGHelper.ShowSvg(ICOResources.Stop);
                AutoKeep.ToolTip = "停止";

                new SoundPlayer().PlaySound(Sounds.继续执行事务);
            }
            else
            {
                ProcessStateBackGround.Background = new SolidColorBrush(Color.FromRgb(247, 137, 178));

                xTranslatorHelper.StopAny = true;
                AutoKeep.Source = SVGHelper.ShowSvg(ICOResources.Keep);
                AutoKeep.ToolTip = "继续";

                new SoundPlayer().PlaySound(Sounds.挂起事务);
            }
        }


        public void KillTransProcessTrd(object sender, MouseButtonEventArgs e)
        {
            if (xTranslatorHelper.LastPostWin != IntPtr.Zero)
            {
                try {
                    WinApi.MoveWindow(xTranslatorHelper.LastPostWin, 0, 0, 300, 180, true);
                } catch { }
            }
            
            xTranslatorHelper.StartAutoSemiInteractive(false);

            if (TransDataHelper.TextTransService != null)
            {
                try
                {
                    TransDataHelper.TextTransService.Abort();
                }
                catch { }

                TransDataHelper.TextTransService = null;
            }

            if (ConvertHelper.ObjToStr(AutoKeep.ToolTip) == "停止")
            {
                ChangeTransProcessState(AutoKeep, null);
            }

            xTranslatorHelper.KillScanThread();

            this.Dispatcher.Invoke(new Action(() =>
            {
                TranslateType.IsEnabled = true;
                TranslateType.Opacity = 1;

                StartTransBtn.Visibility = Visibility.Visible;
                TranslatePercent.Value = 0;
            }));

            TransDataHelper.TransItemArrays.Clear();
        }

        public int HistoryToolViewState = 0;
        private void AutoHideHistoryToolView(object sender, MouseButtonEventArgs e)
        {
            if (HistoryToolViewState == 1)
            {
                HistoryToolViewState = 0;
                AutoHideHistoryToolViewBtn.Content = "显示浮动层";
                DeFine.HistoryToolView.Hide();

                new SoundPlayer().PlaySound(Sounds.隐藏悬浮层);
            }
            else
            {
                HistoryToolViewState = 1;
                AutoHideHistoryToolViewBtn.Content = "隐藏浮动层";
                DeFine.HistoryToolView.Show();

                AutoLocationByHistoryLayer();

                new SoundPlayer().PlaySound(Sounds.显示悬浮层);
            }
        }



        private void MainNavSelect(object sender, MouseButtonEventArgs e)
        {
            if (NeedSaveSetting)
            {
                DeFine.GlobalLocalSetting.SaveConfig();
                NeedSaveSetting = false;
            }

            if (sender is Grid)
            {
                string GetTag = ConvertHelper.ObjToStr((sender as Grid).Tag);
                switch (GetTag)
                {
                    case "TransView":
                        {
                            new SoundPlayer().PlaySound(Sounds.英汉互译选中语音);
                            TranslateCoreView.Visibility = Visibility.Visible;
                            MoCoreView.Visibility = Visibility.Hidden;
                            SettingView.Visibility = Visibility.Hidden;
                            Effect1.Background = new SolidColorBrush(Color.FromRgb(43, 43, 43));
                            Effect2.Background = new SolidColorBrush(Color.FromRgb(60, 60, 60));
                            Effect3.Background = new SolidColorBrush(Color.FromRgb(60, 60, 60));
                        }
                        break;
                    case "MoView":
                        {
                            new SoundPlayer().PlaySound(Sounds.更多功能选中语音);
                            TranslateCoreView.Visibility = Visibility.Hidden;
                            MoCoreView.Visibility = Visibility.Visible;
                            SettingView.Visibility = Visibility.Hidden;
                            Effect1.Background = new SolidColorBrush(Color.FromRgb(60, 60, 60));
                            Effect2.Background = new SolidColorBrush(Color.FromRgb(43, 43, 43));
                            Effect3.Background = new SolidColorBrush(Color.FromRgb(60, 60, 60));

                            if (!System.IO.Directory.Exists(DeFine.GlobalLocalSetting.ModOrganizerConfig.Mo2Path))
                            {
                                ActionWin.Show("提示", "需要配置Mo路径,请选择Mo的安装路径(根目录)", MsgAction.Yes, MsgType.Info);
                                string GetSelectPath = UIHelper.ShowPathSelect();

                                if (ModOrganizerHelper.CheckMo2(GetSelectPath))
                                {
                                    DeFine.GlobalLocalSetting.ModOrganizerConfig = ModOrganizerHelper.ReadMo2Config(GetSelectPath);
                                    DeFine.GlobalLocalSetting.SaveConfig();
                                }
                                else
                                {
                                    ActionWin.Show("提示", "配置错误未检测到设置路径下的ModOrganizer.exe", MsgAction.Yes, MsgType.Info);
                                }
                            }

                            ModLoaders.Items.Clear();

                            foreach (var GetLoaderItem in DeFine.GlobalLocalSetting.ModOrganizerConfig.ModLoaders)
                            {
                                ModLoaders.Items.Add(GetLoaderItem.LoadName);

                            }
                            if (DeFine.GlobalLocalSetting.ModOrganizerConfig.ModLoaders.Count > 0)
                            {
                                ModLoaders.SelectedValue = ModLoaders.Items[ModLoaders.Items.Count - 1];
                            }

                            ModMsgView.Items.Clear();
                            foreach (var GetMsg in ModOrganizerHelper.CheckSkyrimNeeds(DeFine.GlobalLocalSetting.ModOrganizerConfig))
                            {
                                ModMsgView.Items.Add(GetMsg);
                            }

                            AutoReloadModInFo();
                        }
                        break;
                    case "SettingView":
                        {
                            TranslateCoreView.Visibility = Visibility.Hidden;
                            MoCoreView.Visibility = Visibility.Hidden;
                            SettingView.Visibility = Visibility.Visible;
                            Effect1.Background = new SolidColorBrush(Color.FromRgb(60, 60, 60));
                            Effect2.Background = new SolidColorBrush(Color.FromRgb(60, 60, 60));
                            Effect3.Background = new SolidColorBrush(Color.FromRgb(43, 43, 43));

                            if (DeFine.GlobalLocalSetting.PlaySound)
                            {
                                PlaySound.IsChecked = true;
                            }
                            else
                            {
                                PlaySound.IsChecked = false;
                            }

                            ModOrganizerPath.Text = DeFine.GlobalLocalSetting.ModOrganizerConfig.Mo2Path;
                            BackUpPath.Text = DeFine.GlobalLocalSetting.BackUpPath;
                        }
                        break;

                }
            }
        }




        #region WaitTransList

        public void AnyTranslateMsg(string EngineName, string Text, string Result)
        {
            TransLog.Dispatcher.Invoke(new Action(() =>
            {
                if (TransLog.Text.Split(new char[2] { '\r', '\n' }).Length > 100)
                {
                    TransLog.Text = "";
                }
                TransLog.Text += DateTime.Now.ToString() + "->\r\n" + EngineName + "->" + Text + "->" + Result + "\r\n";
                TransLog.ScrollToEnd();
            }));
        }

        private void StartSearch(object sender, MouseButtonEventArgs e)
        {
            AutoReload();
        }

        public int LockerFindWaitTransList = 0;

        public void FindWaitTransList()
        {
            if (LockerFindWaitTransList == 0)
            {
                LockerFindWaitTransList++;

                string GetTransText = STransText.Text;
                string GetTransResult = STransResult.Text;
                string GetTransDataTime = STransDataTime.Text;
                new Thread(() =>
                {
                    try
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            WaitTransListMsgs.Clear();
                        }));

                        PageItem<List<TransRecvItem>> GetFindWaitTrans = null;


                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            GetFindWaitTrans = TransDataHelper.GetAllWaitTrans(GetTransText, GetTransResult, GetTransDataTime, CurrentPageNo);
                        }));

                        if (GetFindWaitTrans != null)
                        {
                            CurrentPageNo = GetFindWaitTrans.PageNo;
                            CurrentMaxPage = GetFindWaitTrans.MaxPage;

                            if (CurrentMaxPage != 0)
                            {
                                this.Dispatcher.Invoke(new Action(() =>
                                {
                                    WaitTransListCap.Content = string.Format("{0}/{1}", CurrentPageNo, CurrentMaxPage);
                                }));
                            }
                            else
                            {
                                this.Dispatcher.Invoke(new Action(() =>
                                {
                                    WaitTransListCap.Content = "Null";
                                }));
                            }

                            foreach (var GetAction in GetFindWaitTrans.CurrentPage)
                            {
                                this.Dispatcher.Invoke(new Action(() =>
                                {
                                    WaitTransListMsgs.AddRow(UIHelper.CreatWaitTranLine(GetAction));
                                }));
                            }

                            if (CurrentMaxPage + CurrentPageNo != 1)
                            {
                                if (CurrentMaxPage < CurrentPageNo)
                                {
                                    Previous(null, null);
                                }
                            }
                        }
                    }
                    catch { }

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        WaitTransList_PreviewMouseUp(WaitTransListCaption, null);
                    }));

                    LockerFindWaitTransList--;
                }).Start();
            }
        }

        private void WaitTransList_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (WaitTransListMsgs != null)
            {
                try
                {

                    Grid GetGrid = (sender as Grid);

                    for (int ir = 0; ir < WaitTransListMsgs.GetMainGrid().Children.Count; ir++)
                    {
                        Grid GetChild = WaitTransListMsgs.GetMainGrid().Children[ir] as Grid;

                        if (GetChild.ColumnDefinitions.Count == GetGrid.ColumnDefinitions.Count)
                        {
                            for (int i = 0; i < GetGrid.ColumnDefinitions.Count; i++)
                            {
                                var GetWidth = GetGrid.ColumnDefinitions[i].Width;
                                GetChild.ColumnDefinitions[i].Width = GetWidth;
                            }
                        }
                    }

                }
                catch { }
            }
        }


        #endregion

        #region Mo2View

        private void SelectModMsgChange(object sender, SelectionChangedEventArgs e)
        {
            SelectValue.Text = ConvertHelper.ObjToStr(ModMsgView.SelectedValue);
        }

        private void AnyModTableClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Label)
            {
                Label LockerLab = sender as Label;
                CurrentTabName = ConvertHelper.ObjToStr(LockerLab.Content);

                switch (ConvertHelper.ObjToStr(LockerLab.Content))
                {
                    case "前置检测":
                        {
                            ModsTable.ChangeTableEffect(0, new Action(() =>
                            {
                                CheckMod.Visibility = Visibility.Hidden;
                                CheckINeed.Visibility = Visibility.Visible;
                                ModMsgView.Items.Clear();
                                foreach (var GetMsg in ModOrganizerHelper.CheckSkyrimNeeds(DeFine.GlobalLocalSetting.ModOrganizerConfig))
                                {
                                    ModMsgView.Items.Add(GetMsg);
                                }
                            }));
                        }
                        break;
                    case "游戏执行记录":
                        {
                            ModsTable.ChangeTableEffect(1, new Action(() =>
                            {
                                CheckMod.Visibility = Visibility.Hidden;
                                CheckINeed.Visibility = Visibility.Hidden;

                            }));
                        }
                        break;
                    case "备份Mod":
                        {
                            ModsTable.ChangeTableEffect(2, new Action(() =>
                            {
                                CheckMod.Visibility = Visibility.Visible;
                                CheckINeed.Visibility = Visibility.Hidden;

                                if (DeFine.GlobalLocalSetting.BackUpPath.Trim().Length > 0)
                                {
                                    if (DeFine.GlobalLocalSetting.ModOrganizerConfig.mod_directory.Trim().Length > 0)
                                    {
                                        ModOrganizerHelper.StartBackupService(CheckModPercent, CurrentPath, CheckMsg);
                                    } 
                                }
                                
                                BSourcePath.Content = DeFine.GlobalLocalSetting.ModOrganizerConfig.mod_directory;
                                BTargetPath.Content = DeFine.GlobalLocalSetting.BackUpPath;
                            }));
                        }
                        break;
                }
            }
        }

        public string CBValue(ComboBox OneCombox)
        {
            string SelectValue = "";
            OneCombox.Dispatcher.Invoke(new Action(() =>
            {
                SelectValue = ConvertHelper.ObjToStr(OneCombox.SelectedValue);
            }));
            return SelectValue;
        }

        public void AutoReloadModInFo()
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                InstallModCount.Content = string.Format("已安装:{0}", DeFine.GlobalLocalSetting.ModOrganizerConfig.Mods.Count);
            }));
            this.Dispatcher.Invoke(new Action(() =>
            {
                EnableModCount.Content = string.Format("已启用:{0}", DeFine.GlobalLocalSetting.ModOrganizerConfig.GetEnableMods(CBValue(ModLoaders)).Count);
            }));
            this.Dispatcher.Invoke(new Action(() =>
            {
                DisableModCount.Content = string.Format("已停用:{0}", DeFine.GlobalLocalSetting.ModOrganizerConfig.GetDisableMods(CBValue(ModLoaders)).Count);
            }));
        }

        private void ModSelectChange(object sender, SelectionChangedEventArgs e)
        {
            AutoReloadModInFo();
        }

      

        #endregion

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Hide();
            Environment.Exit(-1);
        }

        private void ShowEngineView(object sender, MouseButtonEventArgs e)
        {
            new EngineVisualView().Show();
        }

        private void ConfigBaiduYun(object sender, MouseButtonEventArgs e)
        {
            string AppID = Interaction.InputBox("Step1 请输入AppID", "AppID", DeFine.GlobalLocalSetting.BaiDuAppID, -1, -1);
            DeFine.GlobalLocalSetting.BaiDuAppID = AppID;
            string SecretKey = Interaction.InputBox("Step2 请输入SecretKey", "SecretKey", DeFine.GlobalLocalSetting.BaiDuSecretKey, -1, -1);
            DeFine.GlobalLocalSetting.BaiDuSecretKey = SecretKey;

            DeFine.GlobalLocalSetting.SaveConfig();
        }

        public bool NeedSaveSetting = false;

        public void AutoSetCGView()
        {
        }

        private void PlaySound_Click(object sender, RoutedEventArgs e)
        {
            if (((sender) as CheckBox).IsChecked == true)
            {
                DeFine.GlobalLocalSetting.PlaySound = true;
                DeFine.GlobalLocalSetting.SaveConfig();
            }
            else
            {
                DeFine.GlobalLocalSetting.PlaySound = false;
                DeFine.GlobalLocalSetting.SaveConfig();
            }
        }

        private void ModOrganizerPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            DeFine.GlobalLocalSetting.ModOrganizerConfig.Mo2Path = ModOrganizerPath.Text;
            NeedSaveSetting = true;
        }

        private void ShowMo2Select(object sender, MouseButtonEventArgs e)
        {
            string GetSelectPath = UIHelper.ShowPathSelect();
            ModOrganizerPath.Text = GetSelectPath;
        }

        private void AutoShowLocation(object sender, MouseButtonEventArgs e)
        {
            Label LockerLab = null;
            if (sender is Border)
            {
                LockerLab = (sender as Border).Child as Label;
            }
            else
            if(sender is Label)
            {
                LockerLab = sender as Label;
            }

            if (LockerLab != null)
            {
                switch (ConvertHelper.ObjToStr(LockerLab.Content))
                {
                    case "SkyrimPath":
                        {
                            System.Diagnostics.Process.Start("explorer.exe", DeFine.GlobalLocalSetting.ModOrganizerConfig.gamePath);
                        }
                        break;
                    case "Mo2Path":
                        {
                            System.Diagnostics.Process.Start("explorer.exe", DeFine.GlobalLocalSetting.ModOrganizerConfig.Mo2Path);
                        }
                        break;
                    case "ModPath":
                        {
                            System.Diagnostics.Process.Start("explorer.exe", DeFine.GlobalLocalSetting.ModOrganizerConfig.mod_directory);
                        }
                        break;

                }
            }
        }

        private void ClearOverWrite(object sender, MouseButtonEventArgs e)
        {
            if (ActionWin.Show("您确定要清理OverWrite?", "警告清理后之前老存档会被损坏,且需要重新刷Fnis!", MsgAction.YesNo, MsgType.Info) > 0)
            {
                ModOrganizerHelper.ClearOverWrite();
            }
            else
            {

            }
        }

        private void ShowBackUpPathSelect(object sender, MouseButtonEventArgs e)
        {
            string GetSelectPath = UIHelper.ShowPathSelect();
            BackUpPath.Text = GetSelectPath;
        }

        private void BackUpPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            DeFine.GlobalLocalSetting.BackUpPath = BackUpPath.Text;
            NeedSaveSetting = true;
        }
    }
}
