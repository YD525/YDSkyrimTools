using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for ConjunctionDeleteConfirm.xaml
    /// </summary>
    public partial class ConjunctionDeleteConfirm : Window
    {
        public ConjunctionDeleteConfirm()
        {
            InitializeComponent();
        }

        public string Text = "";
        public string Result = "";

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InFos.Items.Clear();

            DataTable NTable = DeFine.GlobalDB.ExecuteQuery(string.Format("Select Rowid,* From Conjunction Where Result = '{0}' COLLATE NOCASE or DefaultResult = '{0}' COLLATE NOCASE or Text = '{1}' COLLATE NOCASE", Result,Text));

            for (int i = 0; i < NTable.Rows.Count; i++)
            {
                string CreatStr = "{0}- 检测:{1},   前:{2},   后:{3},   文本:{4},   结果:{5},   大小写:{6},   失败结果:{7},   优先级:{8}";
                InFos.Items.Add(string.Format(CreatStr, NTable.Rows[i]["Rowid"], 
                    NTable.Rows[i]["StrExist"], NTable.Rows[i]["FrontExist"], NTable.Rows[i]["BehindExist"],
                    NTable.Rows[i]["Text"], NTable.Rows[i]["Result"], NTable.Rows[i]["IgnoreCase"],
                    NTable.Rows[i]["DefaultResult"], NTable.Rows[i]["TIndex"]));
            }
        }

        private void DeleteInFoItem(object sender, RoutedEventArgs e)
        {
            string GetText = ConvertHelper.ObjToStr(InFos.SelectedItem);
            if (GetText.Trim().Length > 0)
            {
                int GetRowid = ConvertHelper.ObjToInt(GetText.Split('-')[0]);

                if (ActionWin.Show("确认要执行删除吗?", "请谨慎操作!,连词层级相连包括逻辑.删除错误有可能影响整个引擎的准确性!", MsgAction.YesNo, MsgType.Info) > 0)
                {
                    string DeleteOrder = "Delete From Conjunction Where Rowid = " + GetRowid.ToString();

                    if (DeFine.GlobalDB.ExecuteNonQuery(DeleteOrder) !=0)
                    {
                        ConjunctionHelper.ConjunctionItems.Clear();

                        string SqlOrder = "Select *,Rowid From Conjunction Where 1=1";

                        DataTable NTable = DeFine.GlobalDB.ExecuteQuery(string.Format(SqlOrder));

                        for (int i = 0; i < NTable.Rows.Count; i++)
                        {
                            ConjunctionHelper.ConjunctionItems.Add(new ConjunctItem(NTable.Rows[i]["StrExist"], NTable.Rows[i]["FrontExist"], NTable.Rows[i]["BehindExist"], NTable.Rows[i]["Text"], NTable.Rows[i]["Result"], NTable.Rows[i]["IgnoreCase"], NTable.Rows[i]["DefaultResult"], NTable.Rows[i]["TIndex"]));
                        }

                        ConjunctionHelper.ConjunctionItems.Sort((x, y) => x.CompareTo(y));
                    }

                    this.Close();
                }
                else
                {
                    this.Close();
                }
            }
        }
    }
}
