using BLCTClassLibrary.WpfLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YDSkyrimTools.UIManager;

namespace YDSkyrimTools
{
    /// <summary>
    /// Interaction logic for HistoryTool.xaml
    /// </summary>
    public partial class HistoryTool : Window
    {
        public HistoryTool()
        {
            InitializeComponent();
        }

        public void AddLog(DateTime OneTime,string Source,string To)
        {
            this.Dispatcher.Invoke(new Action(() => {

                if (TextBody.Children.Count > 1024)
                {
                    TextBody.Children.RemoveAt(0);
                }

                StackPanel OneStackPanel = new StackPanel();
                OneStackPanel.Orientation = Orientation.Horizontal;
                OneStackPanel.HorizontalAlignment = HorizontalAlignment.Center;
                OneStackPanel.VerticalAlignment = VerticalAlignment.Center;
                OneStackPanel.Margin = new Thickness(0, 5, 0, 5);

                FormatedText SourceText = new FormatedText();
                SourceText.FontSize = 20;
                SourceText.StrokeThickness = 1;
                SourceText.Stroke = new SolidColorBrush(Color.FromRgb(255, 153, 241));
                SourceText.StretchSize = 1;
                SourceText.FontWeight = FontWeights.UltraBold;

                LinearGradientBrush OneLineBrush = new LinearGradientBrush();
                OneLineBrush.EndPoint = new Point(0.5, 1);
                OneLineBrush.StartPoint = new Point(0.5, 0);

                GradientStop NewColorA = new GradientStop();
                NewColorA.Color = Color.FromRgb(255, 112, 223);
                NewColorA.Offset = 0;

                GradientStop NewColorB = new GradientStop();
                NewColorB.Color = Color.FromRgb(250, 92, 185);
                NewColorB.Offset = 0.489;

                GradientStop NewColorC = new GradientStop();
                NewColorC.Color = Color.FromRgb(248, 0, 146);
                NewColorC.Offset = 0.998;

                OneLineBrush.GradientStops.Add(NewColorA);
                OneLineBrush.GradientStops.Add(NewColorB);
                OneLineBrush.GradientStops.Add(NewColorC);

                SourceText.Fill = OneLineBrush;

                SourceText.Text = string.Format("{0}-{1}", OneTime.ToString("HH:mm:ss"), Source);

                OneStackPanel.Children.Add(SourceText);

                FormatedText ToText = new FormatedText();
                ToText.FontSize = 20;
                ToText.StretchSize = 1;
                ToText.FontWeight = FontWeights.UltraBold;
                ToText.Fill = new SolidColorBrush(Color.FromRgb(255, 143, 1));
                ToText.Text = string.Format(" -> {0}", To);

                OneStackPanel.Children.Add(ToText);

                TextBody.Children.Add(OneStackPanel);

                if (CanMoveToEnd)
                {
                    Content.ScrollToEnd();
                }
                else
                { 
                
                }

                this.Activate();

            }));
        }


        public bool IsLeftMouseDown = false;

        private void WinHead_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsLock)
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
                    }
                    catch { }
                }
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AutoLocker.Source = SVGHelper.ShowSvg(ICOResources.Locker);

            //new Thread(() =>
            //{

            //    while (true)
            //    {
            //        Thread.Sleep(100);

            //        AddLog(DateTime.Now, "TestStrA", "测试字符串A");
            //    }

            //}).Start();
        }

        public bool CanMoveToEnd = true;

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!IsLock)
            {
                BackGroundLayer.Opacity = 0.5; 
            }
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsLock)
            {
                BackGroundLayer.Opacity = 0.1;
            }
        }


        public bool IsLock = false;

        private void AutoLock(object sender, MouseButtonEventArgs e)
        {
            if (!IsLock)
            {
                IsLock = true;
                AutoLocker.Source = SVGHelper.ShowSvg(ICOResources.Locker);
                this.Topmost = true;
                BackGroundLayer.Opacity = 0;
                TopControl.Opacity = 0.5;
                Content.IsHitTestVisible = false;
            }
            else
            {
                IsLock = false;
                AutoLocker.Source = SVGHelper.ShowSvg(ICOResources.UnLocker);
                this.Topmost = false;
                BackGroundLayer.Opacity = 0.1;
                TopControl.Opacity = 1;
                Content.IsHitTestVisible = true;
            }
        }

        private void ScrollMouseEnter(object sender, MouseEventArgs e)
        {
            CanMoveToEnd = false;
        }

        private void ScrollMouseLeave(object sender, MouseEventArgs e)
        {
            CanMoveToEnd = true;
        }
    }
}
