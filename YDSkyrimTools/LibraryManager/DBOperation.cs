using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using YDSkyrimTools.ConvertManager;
using YDSkyrimTools.SkyrimModManager;

namespace YDSkyrimTools.LibraryManager
{
    public class DBOperation
    {
        public bool DeleteLanguagesItemByID(int Rowid)
        {
            string SqlOrder = "Delete From Languages Where Rowid = {0}";
            int State = DeFine.GlobalDB.ExecuteNonQuery(string.Format(SqlOrder, Rowid));
            if (State != 0)
            {
                return true;
            }

            return false;
        }

        public bool AddConjunction(ConjunctionItem OneItem)
        {
            if (OneItem.GetRowid() <= 0)
            {
                OneItem.Text = OneItem.Text.ToLower();

                string SqlOrder = "Insert Into Conjunction(StrExist,FrontExist,BehindExist,Text,Result,IgnoreCase,DefaultResult,TIndex)Values('{0}','{1}','{2}','{3}','{4}',{5},'{6}',{7})";
                int State = DeFine.GlobalDB.ExecuteNonQuery(string.Format(SqlOrder,OneItem.StrExist,OneItem.FrontExist,OneItem.BehindExist,OneItem.Text,OneItem.Result,OneItem.IgnoreCase,OneItem.DefaultResult,OneItem.TIndex));
                if (State != 0)
                {
                    return true;
                }
            }

            return false;
        }

        public bool AddLanguage(WordItem OneItem)
        {
            if (OneItem.GetRowid() <= 0)
            {
                OneItem.Text = OneItem.Text.ToLower();

                string SqlOrder = "Insert Into Languages([Type],[From],[To],[Text],[Result])Values({0},{1},{2},'{3}','{4}')";
                int State = DeFine.GlobalDB.ExecuteNonQuery(string.Format(SqlOrder, OneItem.Type, OneItem.From, OneItem.To, OneItem.Text, OneItem.Result));
                if (State != 0)
                {
                    return true;
                }
            }

            return false;
        }
        public List<WordItem> GetWordItems(int Count = -1)
        {
            List<WordItem> WordItems = new List<WordItem>();
            string SqlOrder = "Select * From Languages Where 1=1";
            DataTable NTable = DeFine.GlobalDB.ExecuteQuery(SqlOrder);

            for (int i = 0; i < NTable.Rows.Count; i++)
            {
                if (Count != -1)
                {
                    if (i > Count) break;
                }

                WordItems.Add(new WordItem(NTable.Rows[i]["Type"], NTable.Rows[i]["From"], NTable.Rows[i]["To"], NTable.Rows[i]["Text"], NTable.Rows[i]["Result"]));
            }

            return WordItems;
        }


        public bool DeleteConjunctionItemByID(int Rowid)
        {
            string SqlOrder = "Delete From Conjunction Where Rowid = {0}";
            int State = DeFine.GlobalDB.ExecuteNonQuery(string.Format(SqlOrder, Rowid));
            if (State != 0)
            {
                return true;
            }

            return false;
        }
        public List<ConjunctionItem> GetConjunctionItems(int Count = -1)
        {

            string SqlOrder = "Select * From Conjunction Where 1=1";

            List<ConjunctionItem> Conjunctions = new List<ConjunctionItem>();

            DataTable NTable = DeFine.GlobalDB.ExecuteQuery(SqlOrder);

            for (int i = 0; i < NTable.Rows.Count; i++)
            {
                if (Count != -1)
                {
                    if (i > Count) break;
                }

                Conjunctions.Add(new ConjunctionItem(NTable.Rows[i]["StrExist"], NTable.Rows[i]["FrontExist"], NTable.Rows[i]["BehindExist"], NTable.Rows[i]["Text"], NTable.Rows[i]["Result"], NTable.Rows[i]["IgnoreCase"], NTable.Rows[i]["DefaultResult"], NTable.Rows[i]["TIndex"]));
            }

            return Conjunctions;
        }
        public bool AddConjuncationInLibrary(ConjunctionItem OneItem)
        {
            if (OneItem.GetRowid() <= 0)
            {
                string SqlOrder = "Insert Into(StrExist,FrontExist,BehindExist,[Text],[Result],IgnoreCase,DefaultResult,TIndex)Values('{0}','{1}','{2}','{3}','{4}',{5},'{6}','{7}')";
                int State = DeFine.GlobalDB.ExecuteNonQuery(String.Format(SqlOrder,OneItem.StrExist, OneItem.FrontExist, OneItem.BehindExist, OneItem.Text, OneItem.Result, OneItem.IgnoreCase, OneItem.DefaultResult, OneItem.Text));
                if (State != 0)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class AnyDBItem
    {
        public int Rowid = 0;
        public DataTable GetInFo()
        {
            string SqlOrder = "Select * From {0} Where Rowid = '{0}'";
            return DeFine.GlobalDB.ExecuteQuery(string.Format(SqlOrder, Rowid));
        }
    }

    public class WordItem : AnyDBItem //Any Template
    {
        public int Type = 0;
        public int From = 0;
        public int To = 0;
        public string Text = "";
        public string Result = "";

        public WordItem(object Type, object From, object To, object Text, object Result)
        {
            this.Type = ConvertHelper.ObjToInt(Type);
            this.From = ConvertHelper.ObjToInt(From);
            this.To = ConvertHelper.ObjToInt(To);
            this.Text = ConvertHelper.ObjToStr(Text);
            this.Result = ConvertHelper.ObjToStr(Result);
        }

        public int GetRowid()
        {
            string SqlOrder = "Select Rowid From Languages Where [From] = {0} COLLATE NOCASE And [To] = {1} COLLATE NOCASE And [Text] = '{2}' COLLATE NOCASE And [Result] = '{3}' COLLATE NOCASE";
            this.Rowid = ConvertHelper.ObjToInt(DeFine.GlobalDB.ExecuteScalar(string.Format(SqlOrder, this.From, this.To, this.Text, this.Result)));
            return this.Rowid;
        }
    }


    public class ConjunctionItem : AnyDBItem //English
    {
        public string StrExist = "";
        public string FrontExist = "";
        public string BehindExist = "";
        public string Text = "";
        public string Result = "";
        public int IgnoreCase = 0;
        public string DefaultResult = "";
        public int TIndex = 0;

        public ConjunctionItem(object StrExist, object FrontExist, object BehindExist, object Text, object Result, object IgnoreCase, object DefaultResult, object TIndex)
        {
            this.StrExist = ConvertHelper.ObjToStr(StrExist);
            this.FrontExist = ConvertHelper.ObjToStr(FrontExist);
            this.BehindExist = ConvertHelper.ObjToStr(BehindExist);
            this.Text = ConvertHelper.ObjToStr(Text);
            this.Result = ConvertHelper.ObjToStr(Result);
            this.IgnoreCase = ConvertHelper.ObjToInt(IgnoreCase);
            this.DefaultResult = ConvertHelper.ObjToStr(DefaultResult);
            this.TIndex = ConvertHelper.ObjToInt(TIndex);
        }

        public int GetRowid()
        {
            string SqlOrder = "Select Rowid From Conjunction Where [Text] = '{0}' COLLATE NOCASE And [TIndex] = '{1}' COLLATE NOCASE And Result = '{2}' COLLATE NOCASE And DefaultResult = '{3}' COLLATE NOCASE";
            this.Rowid = ConvertHelper.ObjToInt(DeFine.GlobalDB.ExecuteScalar(string.Format(SqlOrder, this.Text, this.TIndex,this.Result,this.DefaultResult)));
            return this.Rowid;
        }
    }
}
