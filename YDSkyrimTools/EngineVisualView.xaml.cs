using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using YDSkyrimTools.TranslateCore;

namespace YDSkyrimTools
{
    /// <summary>
    /// Interaction logic for EngineVisualView.xaml
    /// </summary>
    public partial class EngineVisualView : Window
    {
        public EngineVisualView()
        {
            InitializeComponent();
        }

        public void ReloadCache()
        {
            TranslateCaches.Items.Clear();

            foreach (var Get in DivTranslateEngine.CardCaches)
            {
                TranslateCaches.Items.Add(string.Format("{0}->{1}", Get.Key, Get.Value));
            }
        }
       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ReloadCache();
        }



        public void SearchTarget(object sender, MouseButtonEventArgs e)
        {
            List<EngineProcessItem> EngineProcessItems = new List<EngineProcessItem>();
            ResultTarget.Text = new WordProcess().ProcessWords(ref EngineProcessItems, TransTarget.Text, BDLanguage.EN, BDLanguage.CN);
           
            Languages.Items.Clear();
            Conjunctions.Items.Clear();
            YunTrans.Items.Clear();

            foreach (var Get in EngineProcessItems)
            {
                if (Get.EngineName == "Languages")
                {
                    Languages.Items.Add(Get.Text+"->"+Get.Result+"->"+Get.State);
                }
                if (Get.EngineName == "Conjunction")
                {
                    Conjunctions.Items.Add(Get.Text+"->"+Get.Result+"->"+Get.State);
                }
                if (Get.EngineName == "YunEngine")
                {
                    YunTrans.Items.Add(Get.Text + "->" + Get.Result + "->" + Get.State);
                }
            }

            ReloadCache();
        }

        private void AddAny(object sender, MouseButtonEventArgs e)
        {
            EngineInFo NEngine = new EngineInFo();
            NEngine.Show();
            NEngine.SelectEngine(this,"Div");
        }

        private void AddLanguages(object sender, MouseButtonEventArgs e)
        {
            EngineInFo NEngine = new EngineInFo();
            NEngine.Show();
            NEngine.SelectEngine(this,"Languages");
        }

        private void AddConjunction(object sender, MouseButtonEventArgs e)
        {
            EngineInFo NEngine = new EngineInFo();
            NEngine.Show();
            NEngine.SelectEngine(this,"Conjunction");
        }

        private void DeleteCache(object sender, MouseButtonEventArgs e)
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

        private void DeleteSelectLanguage(object sender, MouseButtonEventArgs e)
        {
            string GetSelectText = ConvertHelper.ObjToStr(Languages.SelectedItem);
            if (GetSelectText.Trim().Length > 0)
            {
                var GetParam = GetSelectText.Split('-');
                string GetText = GetParam[0].Replace(">", "").Replace("（", "").Replace("）", "");
                GetText = GetText.ToLower();
                string GetResult = GetParam[1].Replace(">", "").Replace("（", "").Replace("）", "");
                GetResult = GetResult.ToLower();
                string SqlOrder = "Delete From Languages Where Text = '{0}' COLLATE NOCASE And Result = '{1}' COLLATE NOCASE";
                int State = DeFine.GlobalDB.ExecuteNonQuery(string.Format(SqlOrder,GetText,GetResult));
                if (State != 0)
                {
                    SearchTarget(null,null);
                }
            }
        }

        private void DeleteSelectConjunction(object sender, MouseButtonEventArgs e)
        {
            string GetSelectText = ConvertHelper.ObjToStr(Conjunctions.SelectedItem);
            if (GetSelectText.Trim().Length > 0)
            {
                var GetParam = GetSelectText.Split('-');
                string GetText = GetParam[0].Replace(">", "").Replace("（", "").Replace("）", "");
                GetText = GetText.ToLower();
                string GetResult = GetParam[1].Replace(">", "").Replace("（", "").Replace("）", "");
                GetResult = GetResult.ToLower();
                ConjunctionDeleteConfirm NConjunctionDeleteConfirm = new ConjunctionDeleteConfirm();
                NConjunctionDeleteConfirm.Text = GetText;
                NConjunctionDeleteConfirm.Result = GetResult;
                NConjunctionDeleteConfirm.Show();
                //string SqlOrder = "Delete From Languages Where Text = '{0}' And Result = '{1}'";
                //int State = DeFine.GlobalDB.ExecuteNonQuery(string.Format(SqlOrder, GetText, GetResult));
                //if (State != 0)
                //{
                //    SearchTarget(null, null);
                //}
            }
        }
    }
}
