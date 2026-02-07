using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YDSkyrimTools.ConvertManager;
using YDSkyrimTools.SkyrimModManager;
using YDSkyrimTools.TranslateCore;

namespace YDSkyrimTools
{
    /// <summary>
    /// Interaction logic for EngineView.xaml
    /// </summary>
    public partial class EngineView : Window
    {
        public EngineView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
        }

        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        public void ActiveThis()
        {
            this.Topmost = true;
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
            this.Topmost = false;
        }

        private void SetPhraseEngineUsing(object sender, MouseButtonEventArgs e)
        {
            if (DeFine.PhraseEngineUsing)
            {
                PhraseEngine.Opacity = 0.2;
                DeFine.PhraseEngineUsing = false;
            }
            else
            {
                PhraseEngine.Opacity = 1;
                DeFine.PhraseEngineUsing = true;
            }
            GetState();
        }

        private void SetCodeParsingEngineUsing(object sender, MouseButtonEventArgs e)
        {
            if (DeFine.CodeParsingEngineUsing)
            {
                CodeParsingEngine.Opacity = 0.2;
                DeFine.CodeParsingEngineUsing = false;
            }
            else
            {
                CodeParsingEngine.Opacity = 1;
                DeFine.CodeParsingEngineUsing = true;
            }
            GetState();
        }

        private void SetConjunctionEngineUsing(object sender, MouseButtonEventArgs e)
        {
            if (DeFine.ConjunctionEngineUsing)
            {
                ConjunctionEngine.Opacity = 0.2;
                DeFine.ConjunctionEngineUsing = false;
            }
            else
            {
                ConjunctionEngine.Opacity = 1;
                DeFine.ConjunctionEngineUsing = true;
            }
            GetState();
        }

        private void SetBaiDuYunApiUsing(object sender, MouseButtonEventArgs e)
        {
            if (DeFine.BaiDuYunApiUsing)
            {
                BaiDuYunApi.Opacity = 0.1;
                DeFine.BaiDuYunApiUsing = false;
            }
            else
            {
                BaiDuYunApi.Opacity = 1;
                DeFine.BaiDuYunApiUsing = true;
            }
            GetState();
        }

        private void SetYouDaoYunApiUsing(object sender, MouseButtonEventArgs e)
        {
            if (DeFine.YouDaoYunApiUsing)
            {
                YouDaoYunApi.Opacity = 0.1;
                DeFine.YouDaoYunApiUsing = false;
            }
            else
            {
                YouDaoYunApi.Opacity = 1;
                DeFine.YouDaoYunApiUsing = true;
            }
            GetState();
        }

        private void SetGoogleYunApiUsing(object sender, MouseButtonEventArgs e)
        {
            if (DeFine.GoogleYunApiUsing)
            {
                GoogleYunApi.Opacity = 0.2;
                DeFine.GoogleYunApiUsing = false;
            }
            else
            {
                GoogleYunApi.Opacity = 1;
                DeFine.GoogleYunApiUsing = true;
            }
            GetState();
        }

        private void SetDivCacheEngineUsing(object sender, MouseButtonEventArgs e)
        {
            if (DeFine.DivCacheEngineUsing)
            {
                DivCacheEngine.Opacity = 0.2;
                DeFine.DivCacheEngineUsing = false;
            }
            else
            {
                DivCacheEngine.Opacity = 1;
                DeFine.DivCacheEngineUsing = true;
            }
            GetState();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void ClearDBTranslateCache(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("警告清理后所有包括以前翻译的内容不在缓存,再次翻译会重新从云端获取,会浪费您以前翻译的结果,会增加字数消耗量!", "您确定要清理以翻译的缓存？", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string SqlOrder = "Delete From CloudTranslation Where 1=1";
                int State = DeFine.GlobalDB.ExecuteNonQuery(SqlOrder);
                if (State != 0)
                {
                    MessageBox.Show("Done!");
                    DeFine.GlobalDB.ExecuteNonQuery("vacuum");
                }
            }
            else
            {
                
            }

        }

        private void AddCacheToTranslate(object sender, RoutedEventArgs e)
        {
            if (Text.Text.Trim().Length > 0)
            {
                DivTranslateEngine.AddCardItem(Text.Text, Result.Text);
                MessageBox.Show("Done!");
                TranslateCaches.Items.Clear();

                foreach (var Get in DivTranslateEngine.CardCaches)
                {
                    TranslateCaches.Items.Add(string.Format("{0}->{1}", Get.Key, Get.Value));
                }
            }
        }

        private void DeleteDivTranslateCard(object sender, RoutedEventArgs e)
        {
            string GetValue = ConvertHelper.ObjToStr(TranslateCaches.SelectedValue);

            if (GetValue.Trim().Length > 0)
            {
                GetValue = GetValue.Substring(0, GetValue.IndexOf("->"));

                if (DivTranslateEngine.CardCaches.ContainsKey(GetValue))
                {
                    DivTranslateEngine.CardCaches.Remove(GetValue);
                    TranslateCaches.Items.Remove(TranslateCaches.SelectedValue);
                    MessageBox.Show("Done!");
                }
            }
           
        }

        public void GetState()
        {
            EngineState.Text = string.Format("词组{0}->代码{1}->连词{2}->百度{3}->有道{4}->谷歌{5}->自定义{6}", DeFine.PhraseEngineUsing, DeFine.CodeParsingEngineUsing, DeFine.ConjunctionEngineUsing, DeFine.BaiDuYunApiUsing, DeFine.YouDaoYunApiUsing, DeFine.GoogleYunApiUsing, DeFine.DivCacheEngineUsing);
        }

        public int CacheRowID = 0;
        private void SearchDBCache(object sender, RoutedEventArgs e)
        {
            string GetText = TranslateDBCache.FindCacheAndID(CacheText.Text, ref CacheRowID, 0, 1);
            if (GetText != string.Empty)
            {
                MessageBox.Show(GetText);
            }
        }

        private void DeleteSearchDBCache(object sender, RoutedEventArgs e)
        {
            if (TranslateDBCache.DeleteCacheByID(CacheRowID))
            {
                MessageBox.Show("已删除!");
                CacheRowID = 0;
            }
        }

        private void SetEngineInFo(object sender, RoutedEventArgs e)
        {
            DeFine.EngineInFoView.ActiveWin();
        }
    }
}
