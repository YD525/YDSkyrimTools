
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Design;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using YDSkyrimTools.ConvertManager;
using YDSkyrimTools.SkyrimModManager;
using YDSkyrimTools.xTranslatorManager;

namespace YDSkyrimTools.UIManager
{
    public class UIHelper
    {
        public class YDTablePanelControl
        {
            public Grid ParentTablePanel = null;
            public double MoveSpeed = 0.5;

            public YDTablePanelControl(Grid Parent)
            {
                this.ParentTablePanel = Parent;
                Grid GetOptBlock = (ParentTablePanel.Children[1] as Canvas).Children[0] as Grid;
                GetOptBlock.Width = ((ParentTablePanel.Children[0] as StackPanel).Children[0] as Label).ActualWidth;
            }

            public object LockerTableEffect = new object();
            public void ChangeTableEffect(int Step,Action OneAct)
            {
                if (ParentTablePanel != null)
                {
                    StackPanel GetNavs = ParentTablePanel.Children[0] as StackPanel;

                    if (GetNavs.Children.Count > Step)
                    {
                        double CalcOffset = 0;
                        double AutoWidth = 0;

                        for (int i = 0; i < GetNavs.Children.Count; i++)
                        {
                            Label CurrentNav = GetNavs.Children[i] as Label;
                            CalcOffset += CurrentNav.Margin.Left + CurrentNav.Margin.Right;
                            AutoWidth = CurrentNav.ActualWidth;
                            if (i == Step)
                            {
                                break;
                            }                
                            CalcOffset += CurrentNav.ActualWidth;
                        }

                        Grid GetOptBlock = (ParentTablePanel.Children[1] as Canvas).Children[0] as Grid;

                        GetOptBlock.Width = AutoWidth;

                        new Thread(() =>
                        {

                            lock (LockerTableEffect)
                            {
                                SleepHelper NewSleeper = new SleepHelper();
                                double GetTargetOffset = CalcOffset;
                                double GetBlockOffset = 0;

                                ParentTablePanel.Dispatcher.Invoke(new Action(() =>
                                {
                                    GetBlockOffset = Canvas.GetLeft(GetOptBlock);
                                }));

                                while (GetBlockOffset < GetTargetOffset)
                                {
                                    GetBlockOffset+= MoveSpeed;
                                    ParentTablePanel.Dispatcher.Invoke(new Action(() =>
                                    {
                                        Canvas.SetLeft(GetOptBlock, GetBlockOffset);
                                    }));
                                    NewSleeper.SleepByLoop(3);
                                }

                                while (GetBlockOffset > GetTargetOffset)
                                {
                                    GetBlockOffset-= MoveSpeed;
                                    ParentTablePanel.Dispatcher.Invoke(new Action(() =>
                                    {
                                        Canvas.SetLeft(GetOptBlock, GetBlockOffset);
                                    }));
                                    NewSleeper.SleepByLoop(3);
                                }

                                Application.Current.Dispatcher.Invoke(new Action(() => {
                                    OneAct.Invoke();
                                }));
                            }
                        }).Start();
                    }
                }
            }
        }



        public class SleepHelper
        {
            public int LoopCount = 0;

            public void SleepByLoop(int Count)
            {
                if (LoopCount > 0)
                {
                    LoopCount--;
                }
                else
                {
                    LoopCount = Count;
                    Thread.Sleep(1);
                }
            }
        }

        public static BitmapImage ConvertToBitMapByPath(string SourcePath)
        {
            if (File.Exists(SourcePath))
            {
                try
                {
                    return ConvertHelper.BytesToBitmapImage(DataHelper.GetBytesByFilePath(SourcePath));
                }
                catch { return null; }
            }
            return null;
        }



        public static Grid GetLineStyle(int Rowid)
        {
            Grid OneGrid = new Grid();
            OneGrid.Background = new SolidColorBrush(Color.FromRgb(242, 242, 242));
            OneGrid.Tag = Rowid;

            RowDefinition OneRow = new RowDefinition();
            OneRow.Height = new GridLength(1, GridUnitType.Star);

            RowDefinition RowFooter = new RowDefinition();
            RowFooter.Height = new GridLength(3, GridUnitType.Pixel);

            OneGrid.RowDefinitions.Add(OneRow);
            OneGrid.RowDefinitions.Add(RowFooter);

            Grid EmpGrid = new Grid();
            EmpGrid.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            Grid.SetColumn(EmpGrid, 0);
            Grid.SetColumnSpan(EmpGrid, 999);
            Grid.SetRow(EmpGrid, 1);

            OneGrid.Children.Add(EmpGrid);

            return OneGrid;
        }

        public static Grid GetLineStyle(string Com)
        {
            Grid OneGrid = new Grid();
            OneGrid.Background = new SolidColorBrush(Color.FromRgb(242, 242, 242));
            OneGrid.Tag = Com;

            RowDefinition OneRow = new RowDefinition();
            OneRow.Height = new GridLength(1, GridUnitType.Star);

            RowDefinition RowFooter = new RowDefinition();
            RowFooter.Height = new GridLength(5, GridUnitType.Pixel);

            OneGrid.RowDefinitions.Add(OneRow);
            OneGrid.RowDefinitions.Add(RowFooter);

            Grid EmpGrid = new Grid();
            EmpGrid.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            Grid.SetColumn(EmpGrid, 0);
            Grid.SetColumnSpan(EmpGrid, 6);
            Grid.SetRow(EmpGrid, 1);

            OneGrid.Children.Add(EmpGrid);

            return OneGrid;
        }


        public static Grid CreatListHeader(List<string> Tittles)
        {
            Grid HeaderGrid = new Grid();
            HeaderGrid.Background = new SolidColorBrush(Color.FromRgb(215, 215, 215));

            for (int i = 0; i < Tittles.Count; i++)
            {
                ColumnDefinition OneCol = new ColumnDefinition();

                OneCol.Width = new GridLength(1, GridUnitType.Star);

                HeaderGrid.ColumnDefinitions.Add(OneCol);
                Label OneTittle = new Label();
                OneTittle.HorizontalAlignment = HorizontalAlignment.Center;
                OneTittle.VerticalAlignment = VerticalAlignment.Center;
                OneTittle.Content = Tittles[i];
                OneTittle.FontSize = 15;
                Grid.SetColumn(OneTittle, i);

                HeaderGrid.Children.Add(OneTittle);
            }

            return HeaderGrid;
        }


        private static Grid SelectDriveItemLine = null;
        public static void AnyDriveItemLineClick(object sender, MouseButtonEventArgs e)
        {
            SelectDriveItemLine = sender as Grid;
        }




        public static YDGUI WorkingWin = null;

        public static void Init(YDGUI Win)
        {
            if (WorkingWin == null)
            {
                WorkingWin = Win;

                WorkingWin.TMin.Source = UIHelper.ConvertToBitMapByPath(DeFine.GetFullPath(DeFine.ResourcesPath) + @"\PNG\TMin.png");
                WorkingWin.TChangeWin.Source = UIHelper.ConvertToBitMapByPath(DeFine.GetFullPath(DeFine.ResourcesPath) + @"\PNG\TChangeWin.png");
                WorkingWin.TClose.Source = UIHelper.ConvertToBitMapByPath(DeFine.GetFullPath(DeFine.ResourcesPath) + @"\PNG\TClose.png");

                WorkingWin.BaiduEngine.Source = SVGHelper.ShowSvg(ICOResources.Baidu);
                WorkingWin.CodeEngine.Source = SVGHelper.ShowSvg(ICOResources.Code);
                WorkingWin.ConjunctionEngine.Source = SVGHelper.ShowSvg(ICOResources.Conjunction);
                WorkingWin.DivEngine.Source = SVGHelper.ShowSvg(ICOResources.Div);
                WorkingWin.GoogleEngine.Source = SVGHelper.ShowSvg(ICOResources.Google);
                WorkingWin.LanguagesEngine.Source = SVGHelper.ShowSvg(ICOResources.Languages);
                WorkingWin.YouDaoEngine.Source = SVGHelper.ShowSvg(ICOResources.YouDao);

                WorkingWin.TransICO.Source = SVGHelper.ShowSvg(ICOResources.TransICO);
                WorkingWin.MO2ICO.Source = SVGHelper.ShowSvg(ICOResources.MO2ICO);
                WorkingWin.SettingICO.Source = SVGHelper.ShowSvg(ICOResources.SettingICO);

                WorkingWin.AutoKeep.Source = SVGHelper.ShowSvg(ICOResources.Keep);

                WorkingWin.KillTransThread.Source = SVGHelper.ShowSvg(ICOResources.End);
            }
        }


        public static void SecurityUIThread(Window OneWin, Action OneAct)
        {
            if (OneWin != null)
            {
                ThreadPool.QueueUserWorkItem(C =>
                {
                    Thread.Sleep(100);

                    try
                    {
                        OneWin.Dispatcher.BeginInvoke(OneAct);
                    }
                    catch (Exception Ex) { MessageBox.Show(Ex.Message); }
                });
            }
        }

        public static void SecurityUIThread(UIElement OneControl, Action OneAct)
        {
            if (OneControl != null)
            {
                ThreadPool.QueueUserWorkItem(C =>
                {
                    Thread.Sleep(100);

                    try
                    {
                        OneControl.Dispatcher.BeginInvoke(OneAct);
                    }

                    catch (Exception Ex) { MessageBox.Show(Ex.Message); }
                });
            }
        }


        #region FindWaitTranView

        private static Grid SelectWaitTranLine = null;

        public static void AnyWaitTranLineClick(object sender, MouseButtonEventArgs e)
        {
            SelectWaitTranLine = sender as Grid;
            //var GetData = DelegateHelper.GetActionRecvFullInFo(ConvertHelper.ObjToInt((sender as Grid).Tag));
            //DeFine.CurrentLayer.ATarget.Text = GetData.Target;
            //DeFine.CurrentLayer.ADefValue.Text = GetData.DefValue;
            //DeFine.CurrentLayer.ANewValue.Text = GetData.NewValue;
        }

        public static void ModifyText(object sender, KeyEventArgs e)
        {
            if (SelectWaitTranLine != null)
            {
                string GetText = (sender as TextBox).Text;
                int GetKey = ConvertHelper.ObjToInt(SelectWaitTranLine.Tag);

                var GetData = TransDataHelper.SelectLibraryByName(xTranslatorHelper.TransTargetName);

                if (GetData != null)
                {
                    if (GetData.ContainsKey(GetKey))
                    {
                        GetData[GetKey].Result = GetText;
                    }
                }
            }
        }

        public static string ShowPathSelect()
        {
            CommonOpenFileDialog Dialog = new CommonOpenFileDialog();
            Dialog.IsFolderPicker = true;   //设置为选择文件夹
            if (Dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return Dialog.FileName;
            }

            return string.Empty;
        }

        public static Grid CreatWaitTranLine(TransRecvItem OneMsg)
        {
            int GetKey = OneMsg.Text.GetHashCode();
            Grid OneGrid = GetLineStyle(GetKey);
            OneGrid.PreviewMouseDown += AnyWaitTranLineClick;

            ColumnDefinition Col1st = new ColumnDefinition();
            Col1st.Width = new GridLength(130, GridUnitType.Pixel);
            OneGrid.ColumnDefinitions.Add(Col1st);

            ColumnDefinition Col2nd = new ColumnDefinition();
            Col2nd.Width = new GridLength(100, GridUnitType.Pixel);
            OneGrid.ColumnDefinitions.Add(Col2nd);

            ColumnDefinition Col3rd = new ColumnDefinition();
            Col3rd.Width = new GridLength(1, GridUnitType.Star);
            OneGrid.ColumnDefinitions.Add(Col3rd);

            ColumnDefinition Col4th = new ColumnDefinition();
            Col4th.Width = new GridLength(1, GridUnitType.Star);
            OneGrid.ColumnDefinitions.Add(Col4th);

            Label Rowid = new Label();
            Rowid.HorizontalAlignment = HorizontalAlignment.Center;
            Rowid.VerticalAlignment = VerticalAlignment.Center;
            Rowid.Content = GetKey;
            Rowid.FontWeight = FontWeights.Bold;
            Rowid.FontSize += 1;

            Grid.SetColumn(Rowid, 0);
            OneGrid.Children.Add(Rowid);

            Label UPDataTime = new Label();
            UPDataTime.HorizontalAlignment = HorizontalAlignment.Center;
            UPDataTime.VerticalAlignment = VerticalAlignment.Center;
            UPDataTime.Content = OneMsg.UPDataTime.ToString("HH:mm:ss");
            UPDataTime.FontWeight = FontWeights.Bold;
            UPDataTime.FontSize += 1;

            Grid.SetColumn(UPDataTime, 1);
            OneGrid.Children.Add(UPDataTime);


            TextBox Text = new TextBox();
            Text.HorizontalAlignment = HorizontalAlignment.Center;
            Text.VerticalAlignment = VerticalAlignment.Center;
            Text.Text = OneMsg.Text;
            Text.BorderBrush = null;
            Text.BorderThickness = new Thickness(0);
            Text.Background = null;
            Text.IsReadOnly = true;
            Text.FontSize = 13;
            Text.TextWrapping = TextWrapping.Wrap;

            Grid.SetColumn(Text, 2);
            OneGrid.Children.Add(Text);


            TextBox Result = new TextBox();
            Result.HorizontalAlignment = HorizontalAlignment.Center;
            Result.VerticalAlignment = VerticalAlignment.Center;
            Result.AcceptsReturn = true;
            Result.TextWrapping = TextWrapping.Wrap;
            Result.Width = 190;
            Result.FontSize = 13;
            Result.Background = null;
            Result.PreviewKeyUp += ModifyText;
            Result.Text = OneMsg.Result;

            Grid.SetColumn(Result, 3);
            OneGrid.Children.Add(Result);

            return OneGrid;
        }

        #endregion


    }
}
