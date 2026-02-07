using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Management;
using System.Windows.Documents.DocumentStructures;
using YDSkyrimTools.ConvertManager;
using YDSkyrimTools.SkyrimModManager;
using YDSkyrimTools.SqlManager;
using YDSkyrimTools.SQLManager;
using YDSkyrimTools.TranslateCore;
using YDSkyrimTools.UIManager;

namespace YDSkyrimTools.xTranslatorManager
{
    public class TransDataHelper
    {

        private static bool LockerTranslateService = false;

        public static void StartAutoTranslateService(bool Check)
        {
            if (Check)
            {
                if (!LockerTranslateService)
                {
                    LockerTranslateService = true;

                    new Thread(() =>
                    {

                        while (true)
                        {
                            Thread.Sleep(1000);

                            try
                            {

                                for (int i = 0; i < TransItemArrays.Count; i++)
                                {
                                    if (TransItemArrays[i].SourceName.Equals(xTranslatorHelper.TransTargetName))
                                        if (TransItemArrays[i].TransRecvItems.Count > 0)
                                        {
                                            bool IsClear = true;

                                            for (int ir = 0; ir < TransItemArrays[i].TransRecvItems.Keys.Count; ir++)
                                            {
                                                int GetKey = TransItemArrays[i].TransRecvItems.Keys.ToArray()[ir];

                                                if (TransItemArrays[i].TransRecvItems[GetKey].Result.Length == 0)
                                                {
                                                    IsClear = false;

                                                    try
                                                    {
                                                        List<EngineProcessItem> EngineProcessItems = new List<EngineProcessItem>();
                                                        TransItemArrays[i].TransRecvItems[GetKey].Result = LanguageHelper.OptimizeString(new WordProcess().ProcessWords(ref EngineProcessItems, TransItemArrays[i].TransRecvItems[GetKey].Text, BDLanguage.EN, BDLanguage.CN));
                                                        AddTransLine(TransItemArrays[i].CurrentDB, new DictionaryItem(TransItemArrays[i].TransRecvItems[GetKey].Text, TransItemArrays[i].TransRecvItems[GetKey].Result, 0, 1));

                                                        Thread.Sleep(500);
                                                    }
                                                    catch
                                                    {
                                                        Thread.Sleep(3000);
                                                    }
                                                }
                                            }

                                            if (IsClear && xTranslatorHelper.CurrentStep == 1)
                                            {
                                                xTranslatorHelper.WaitTransCount = 0;
                                            }
                                        }
                                }

                            }
                            catch { }
                        }

                    }).Start();
                }
            }
            else
            {
                LockerTranslateService = false;
            }
        }

        public static Dictionary<int, TransRecvItem> SelectLibraryByName(string LibraryName)
        {
            for (int i = 0; i < TransItemArrays.Count; i++)
            {
                if (TransItemArrays[i].SourceName.Equals(LibraryName))
                {
                    return TransItemArrays[i].TransRecvItems;
                }
            }
            return new Dictionary<int, TransRecvItem>();
        }

        public static PageItem<List<TransRecvItem>> GetAllWaitTrans(string Text, string Result, string DataTime, int PageIndex)
        {
            if (CurrentLibraryName != "")
            {
                if (PageIndex == 0) PageIndex = 1;

                var GetData = SelectLibraryByName(CurrentLibraryName).Values.ToList();

                List<TransRecvItem> Query = GetData.Where(I => I.Text.Contains(Text) && I.Result.Contains(Result) && I.UPDataTime.ToString().Contains(DataTime)).ToList();

                int PageCount = Query.Count / DeFine.DefPageSize;

                if (Query.Count % DeFine.DefPageSize > 0)
                {
                    PageCount++;
                }

                var GetPage = Query.Take(DeFine.DefPageSize * PageIndex).Skip(DeFine.DefPageSize * (PageIndex - 1)).ToList();

                return new PageItem<List<TransRecvItem>>(GetPage, PageIndex, PageCount);
            }

            return null;
        }

        public static string CurrentLibraryName = "";
        public static List<TransItemArray> TransItemArrays = new List<TransItemArray>();

        public static void AddTransLibrary(string LibraryName)
        {
            bool IsNewTrans = true;

            for (int i = 0; i < TransItemArrays.Count; i++)
            {
                if (TransItemArrays[i].SourceName.Equals(LibraryName))
                {
                    TransItemArrays[i].TransRecvItems.Clear();
                    IsNewTrans = false;
                }
            }

            if (IsNewTrans)
            {
                TransItemArray NTransItemArray = new TransItemArray();
                NTransItemArray.SourceName = LibraryName;

                string CreatPath = DeFine.GetFullPath(string.Format(@"\Dictionary\{0}.db", LibraryName));

                if (!File.Exists(CreatPath))
                {
                    File.Copy(DeFine.GetFullPath(@"\Dictionary\TemplateDB.db"), CreatPath);
                }

                NTransItemArray.CurrentDB = new SqlCore<SQLiteHelper>(CreatPath);

                TransItemArrays.Add(NTransItemArray);
            }
        }

        public static void AddTransItem(string LibraryName, TransRecvItem OneRecv)
        {
            CurrentLibraryName = LibraryName;

            for (int i = 0; i < TransItemArrays.Count; i++)
            {
                if (TransItemArrays[i].SourceName.Equals(LibraryName))
                {
                    int GetKey = OneRecv.Text.GetHashCode();

                    OneRecv.Result = HttpUtility.HtmlDecode(SearchTransItem(TransItemArrays[i].CurrentDB, OneRecv.Text));

                    if (!TransItemArrays[i].TransRecvItems.ContainsKey(GetKey))
                    {
                        DeFine.HistoryToolView.AddLog(DateTime.Now, "AddCache", OneRecv.Text);
                        TransItemArrays[i].TransRecvItems.Add(GetKey, OneRecv);
                    }

                    return;
                }
            }
        }


        public static string SearchTransItem(SqlCore<SQLiteHelper> LockerDB, string Text)
        {
            return ConvertHelper.ObjToStr(LockerDB.ExecuteScalar(string.Format("Select Result From Dictionarys Where Text = '{0}'", HttpUtility.HtmlEncode(Text))));
        }

        public static bool AddTransLine(SqlCore<SQLiteHelper> LockerDB, DictionaryItem Item)
        {
            int SelectRowid = ConvertHelper.ObjToInt(LockerDB.ExecuteScalar(string.Format("Select Rowid From Dictionarys Where Text = '{0}'", HttpUtility.HtmlEncode(Item.Text))));

            if (SelectRowid <= 0)
            {
                string SqlOrder = "Insert Into Dictionarys(Text,Result,TransFrom,TransTo)Values('{0}','{1}',{2},{3})";
                int State = LockerDB.ExecuteNonQuery(string.Format(SqlOrder, HttpUtility.HtmlEncode(Item.Text), HttpUtility.HtmlEncode(Item.Result), Item.TransFrom, Item.TransTo));
                if (State != 0)
                {
                    return true;
                }
                return false;
            }
            else
            {
                string SqlOrder = "UPDate Dictionarys Set Text = '{1}',Result = '{2}',TransFrom = {3},TransTo = {4} Where Rowid = {0}";
                int State = LockerDB.ExecuteNonQuery(string.Format(SqlOrder, SelectRowid, HttpUtility.HtmlEncode(Item.Text), HttpUtility.HtmlEncode(Item.Result), Item.TransFrom, Item.TransTo));
                if (State != 0)
                {
                    return true;
                }
                return false;
            }
        }

        public static Thread TextTransService = null;

        public static void StartTransServiceByText(System.Windows.Controls.ProgressBar OneProcess, string FilePath)
        {
            if (File.Exists(FilePath))
            {
                if (TextTransService == null)
                {
                    TextTransService = new Thread(() => 
                    {
                        string GetTrans = TranslateTxt(OneProcess,FilePath);
                        DataHelper.WriteFile(FilePath,GetTrans,Encoding.UTF8);
                        DeFine.WorkWin.KillTransProcessTrd(null, null);
                    });

                    TextTransService.Start();
                }
            }
        }


        public static string ProcessMCMText(string Content)
        {
            string RichText = "";

            foreach (var GetLine in Content.Split(new char[2] { '\r', '\n' }))
            {
                if (GetLine.Trim().Length > 0)
                {
                    if (!GetLine.Trim().StartsWith("#"))
                    {
                        RichText += GetLine + "\r\n";
                    }
                }
            }
            return RichText;
        }

        public static int CurrentProcess = 0;
        public static int NeedProcess = 0;

        public static string TranslateTxt(System.Windows.Controls.ProgressBar OneProcess, string TargetPath)
        {
            string ORichText = "";

            if (File.Exists(TargetPath))
            {
                string GetTargetName = TargetPath.Substring(TargetPath.LastIndexOf(@"\") + @"\".Length);

                xTranslatorHelper.TransTargetName = GetTargetName;

                AddTransLibrary(GetTargetName);

                var GetContent = ProcessMCMText(FileHelper.ReadFileByStr(TargetPath, Encoding.UTF8)).Split('$');

                int MaxProcess = GetContent.Length;

                CurrentProcess = 0;

                NeedProcess = GetContent.Length;

                OneProcess.Dispatcher.Invoke(new Action(() => {
                    OneProcess.Maximum = NeedProcess;
                }));

                foreach (var Get in GetContent)
                {
                    if (Get.Trim().Length > 0)
                    {
                        string GetLine = '$' + Get;

                        if (GetLine.Contains("\t"))
                        {
                            string Key = GetLine.Split('\t')[0].Trim();
                            string Value = GetLine.Split('\t')[1].Trim();

                            AddTransItem(GetTargetName,new TransRecvItem(0,1,Value,""));
                        }
                    }
                }

                NextWait:

                int CurrentState = 0;

                bool IsTransing = false;

                for (int i = 0; i < TransItemArrays.Count; i++)
                {
                    if (TransItemArrays[i].SourceName.Equals(GetTargetName))
                    {
                        for (int ir = 0; ir < TransItemArrays[i].TransRecvItems.Keys.Count; ir++)
                        {
                            int GetKey = TransItemArrays[i].TransRecvItems.Keys.ToArray()[ir];

                            if (TransItemArrays[i].TransRecvItems[GetKey].Result.Length == 0)
                            {
                                IsTransing = true;
                            }
                            else
                            {
                                CurrentState++;
                            }
                        }
                    }
                }

                if (IsTransing)
                {
                    Thread.Sleep(1000);

                    OneProcess.Dispatcher.Invoke(new Action(() => {
                        OneProcess.Value = CurrentState;
                    }));
                    goto NextWait;
                }

                OneProcess.Dispatcher.Invoke(new Action(() => {
                    OneProcess.Value = 0;
                }));

                if (ActionWin.Show("准备开始写入", "准备开始执行写入,确认翻译内容无问题后点击确认即可!", MsgAction.YesNo, MsgType.Info) > 0)
                {
                    foreach (var Get in GetContent)
                    {
                        if (Get.Trim().Length > 0)
                        {
                            string GetLine = '$' + Get;

                            if (GetLine.Contains("\t"))
                            {
                                string Key = GetLine.Split('\t')[0].Trim();
                                string Value = GetLine.Split('\t')[1].Trim();


                                int GetKey = Value.GetHashCode();
                                var GetData = TransDataHelper.SelectLibraryByName(GetTargetName);

                                string GetCNText = Key;

                                if (GetData.ContainsKey(GetKey))
                                {
                                    GetCNText = GetData[GetKey].Result;
                                }

                                GetLine = String.Format("{0}\t {1}", Key, GetCNText);
                            }

                            ORichText += GetLine.Replace("  ", " ") + "\r\n";
                        }

                        CurrentProcess++;

                        OneProcess.Dispatcher.Invoke(new Action(() => {
                            OneProcess.Value++;
                        }));
                    }
                }

                if (ActionWin.Show("字段文件存储确认", string.Format("是否更新字典文件于{0}目录下!", DeFine.GetFullPath(@"\Dictionary\" + GetTargetName + ".db")), MsgAction.YesNo, MsgType.Info) > 0)
                {
                    for (int i = 0; i < TransDataHelper.TransItemArrays.Count; i++)
                    {
                        if (TransDataHelper.TransItemArrays[i].SourceName.Equals(GetTargetName))
                        {
                            foreach (var Get in TransDataHelper.TransItemArrays[i].TransRecvItems)
                            {
                                TransDataHelper.AddTransLine(TransDataHelper.TransItemArrays[i].CurrentDB, new DictionaryItem(Get.Value.Text, Get.Value.Result, 0, 1));
                            }
                        }
                    }
                }
            }

            return ORichText;
        }
    }

    public class DictionaryItem
    {
        public int Rowid = 0;
        public string Text = "";
        public string Result = "";
        public int TransFrom = 0;
        public int TransTo = 0;

        public DictionaryItem(string Text, string Result, int TransFrom, int TransTo)
        {
            this.Text = Text;
            this.Result = Result;
            this.TransFrom = TransFrom;
            this.TransTo = TransTo;
        }

        public DictionaryItem(object Rowid, object Text, object Result, object TransFrom, object TransTo)
        {
            this.Rowid = ConvertHelper.ObjToInt(Rowid);
            this.Text = ConvertHelper.ObjToStr(Text);
            this.Result = ConvertHelper.ObjToStr(Result);
            this.TransFrom = ConvertHelper.ObjToInt(TransFrom);
            this.TransTo = ConvertHelper.ObjToInt(TransTo);
        }
    }

    public class TransItemArray
    {
        public string SourceName = "";
        public Dictionary<int, TransRecvItem> TransRecvItems = new Dictionary<int, TransRecvItem>();
        public SqlCore<SQLiteHelper> CurrentDB = null;
    }

    public class TransRecvItem
    {
        public int Rowid = 0;
        public string Text = "";
        public string Result = "";
        public int From = 0;
        public int To = 0;
        public bool IsTransed = false;
        public DateTime UPDataTime;

        public TransRecvItem(int From, int To, string Text, string Result)
        {
            this.From = From;
            this.To = To;
            this.Text = Text;
            this.Result = Result;
            this.IsTransed = false;
            this.UPDataTime = DateTime.Now;
        }
    }
}
