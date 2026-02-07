using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YDSkyrimTools.ConvertManager;
using YDSkyrimTools.LibraryManager;
using YDSkyrimTools.TranslateCore;

namespace YDSkyrimTools
{
    /// <summary>
    /// Interaction logic for EngineInFo.xaml
    /// </summary>
    public partial class EngineInFo : Window
    {
        public EngineInFo()
        {
            InitializeComponent();
        }
        public EngineVisualView LockerView;
        public void SelectEngine(EngineVisualView View, string Name)
        {
            this.LockerView = View;
            if (Name == "Languages")
            {
                Conjunction.Visibility = Visibility.Collapsed;
                Div.Visibility = Visibility.Collapsed;
                LanguagesSetting.Visibility = Visibility.Visible;
            }
            if (Name == "Conjunction")
            {
                Languages.Visibility = Visibility.Collapsed;
                Div.Visibility = Visibility.Collapsed;
                ConjunctionSetting.Visibility = Visibility.Visible;
            }
            if (Name == "Div")
            {
                Languages.Visibility = Visibility.Collapsed;
                Conjunction.Visibility = Visibility.Collapsed;
                Currency.Visibility = Visibility.Hidden;
                CacheSetting.Visibility = Visibility.Visible;
            }
        }

        public void ActiveWin()
        {
            this.Show();
            this.Activate();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        public int GetComboxBoxValue(ComboBox OneBox)
        {
            string GetStr = ConvertHelper.ObjToStr(OneBox.SelectedValue);
            return int.Parse(GetStr.Split('|')[0]);
        }
        private void AnyButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Label)
            {
                Label LockerLab = (Label)sender;
                string GetButtonName = ConvertHelper.ObjToStr(LockerLab.Content);
                int GetTag = ConvertHelper.ObjToInt(LockerLab.Tag);
                switch (GetButtonName)
                {
                    case "添加到词库":
                        {
                            if (GetTag == 0)
                            {
                                if (Text.Text.Trim().Length > 0)
                                {
                                    if (OneDBOperation.AddLanguage(new WordItem(GetComboxBoxValue(Type), GetComboxBoxValue(From), GetComboxBoxValue(To), Text.Text, Result.Text)))
                                    {
                                        LockerView.SearchTarget(null, null);
                                        this.Close();
                                    }
                                    else
                                    {
                                        MessageBox.Show("添加失败,重复的数据!");
                                    }
                                }
                            }
                        }
                    break;
                    case "添加到缓存":
                        {
                            if (EndContent.Text.Trim().Length > 0)
                            {
                                DivTranslateEngine.AddCardItem(EndContent.Text, TargetContent.Text);
                                LockerView.ReloadCache();
                                this.Close();
                            }
                        }
                    break;
                    case "添加到连词库":
                        {
                            if (CText.Text.Trim().Length > 0)
                            {
                                int IsIgnoreCase = 0;

                                if (IgnoreCase.IsChecked == true)
                                {
                                    IsIgnoreCase = 1;
                                }

                                ConjunctionItem OneItem = new ConjunctionItem(StrExist.Text,FrontExist.Text,BehindExist.Text,CText.Text,CResult.Text, IsIgnoreCase, CDefaultResult.Text, TIndex.Text);
                                if (OneDBOperation.AddConjunction(OneItem))
                                {
                                    DeFine.ReLoadConjunction();
                                    LockerView.SearchTarget(null, null);
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show("添加失败,重复的数据!");
                                }
                            }
                        }
                        break;
                }

            }
        }

        public DBOperation OneDBOperation = null;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OneDBOperation = new DBOperation();

            Type.Items.Clear();
            Type.Items.Add("0|单词");
            Type.Items.Add("1|词组");

            Type.SelectedValue = Type.Items[0];

            From.Items.Clear();
            From.Items.Add("0|英文");
            From.Items.Add("1|中文");

            From.SelectedValue = From.Items[0];

            To.Items.Clear();
            To.Items.Add("0|英文");
            To.Items.Add("1|中文");

            To.SelectedValue = To.Items[1];

            LanguagesSetting.Visibility = Visibility.Hidden;
            CacheSetting.Visibility = Visibility.Hidden;
            ConjunctionSetting.Visibility = Visibility.Hidden;
        }
    }
}
