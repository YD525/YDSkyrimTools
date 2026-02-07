using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using YDSkyrimTools.SkyrimModManager;
using YDSkyrimTools.SQLManager;
using YDSkyrimTools.TranslateCore;
using YDSkyrimTools.xTranslatorManager;

namespace YDSkyrimTools
{
    public class DeFine
    {
        public const string ResourcesPath = @"\ResourcesData\";

        public static bool PhraseEngineUsing = true;//词组引擎
        public static bool CodeParsingEngineUsing = true;//代码处理引擎
        public static bool ConjunctionEngineUsing = true;//连词引擎
        public static bool BaiDuYunApiUsing = true;//百度翻译引擎
        public static bool YouDaoYunApiUsing = true;//有道翻译引擎
        public static bool GoogleYunApiUsing = true;//谷歌翻译引擎
        public static bool DivCacheEngineUsing = true;//自定义内存翻译引擎(一次性)
        public static bool AutoTranslate = true;//始终关闭自动翻译

        public static string BackupPath = @"\BackUpData\";//自动备份路径

        public static string CurrentVersion = "1.2.3";
        public static LocalSetting GlobalLocalSetting = new LocalSetting();
        public static YDGUI WorkWin = null;
        public static AboutAuthor About = null;
        public static EngineView EngineView = null;
        public static EngineInFo EngineInFoView = null;

        public static int DefPageSize = 100;
        public static SqlCore<SQLiteHelper> GlobalDB = null;

        public static HistoryTool HistoryToolView = null;

        public static void ReLoadConjunction()
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

        public static void Init(YDGUI WorkWin)//初始化
        {
            DeFine.WorkWin = WorkWin;

            GlobalLocalSetting.ReadConfig();
            if (Directory.Exists(GlobalLocalSetting.ModOrganizerConfig.Mo2Path))
            {
                GlobalLocalSetting.ModOrganizerConfig = ModOrganizerHelper.ReadMo2Config(GlobalLocalSetting.ModOrganizerConfig.Mo2Path);
            }

            GlobalDB = new SqlCore<SQLiteHelper>(DeFine.GetFullPath(@"\system.db"));

            GoogleTApi.StartGoogleListenService(true);
            TransDataHelper.StartAutoTranslateService(true);
            ConjunctionHelper.Init();

            About = new AboutAuthor();
            About.Hide();
            EngineView = new EngineView();
            EngineView.Hide();
            EngineInFoView = new EngineInFo();
            EngineInFoView.Hide();

            HistoryToolView = new HistoryTool();
            HistoryToolView.Hide();

        }

        public static string GetFullPath(string Path)
        {
            string GetShellPath = System.Windows.Forms.Application.StartupPath;
            return GetShellPath + Path;
        }

        public static void ExitAny()
        {
            Application.Current.Shutdown(-1);
        }
    }

    public class LocalSetting
    {
        public string BackUpPath = "";
        public string APath = "";
        public string BPath = "";
        public string SkyrimPath = "";
        public MoConfigItem ModOrganizerConfig = new MoConfigItem();
        public bool PlaySound = false;
        public string GoogleKey = "";
        public string BaiDuAppID = "";
        public string BaiDuSecretKey = "";
        public int TransCount = 0;
        public void ReadConfig()
        {
            if (File.Exists(DeFine.GetFullPath(@"\setting.config")))
            {
                var GetSetting = JsonManager.JsonCore.JsonParse<LocalSetting>(FileHelper.ReadFileByStr(DeFine.GetFullPath(@"\setting.config"), Encoding.UTF8));

                this.APath = GetSetting.APath;
                this.BPath = GetSetting.BPath;
                this.SkyrimPath = GetSetting.SkyrimPath;
                this.GoogleKey = GetSetting.GoogleKey;
                this.BaiDuAppID = GetSetting.BaiDuAppID;
                this.BaiDuSecretKey = GetSetting.BaiDuSecretKey;
                this.TransCount = GetSetting.TransCount;
                this.ModOrganizerConfig = GetSetting.ModOrganizerConfig;
                this.PlaySound = GetSetting.PlaySound;
                this.BackUpPath = GetSetting.BackUpPath;
            }
        }

        public void SaveConfig()
        {
            var GetSettingContent = JsonManager.JsonCore.CreatJson(this);

            FileHelper.WriteFile(DeFine.GetFullPath(@"\setting.config"), GetSettingContent,Encoding.UTF8);
        }
    }
}
