using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using YDSkyrimTools.ConvertManager;
using YDSkyrimTools.SkyrimModManager;

namespace YDSkyrimTools.SkyrimEnbManager
{
    public class EnbHelper
    {
        public static List<MainNameCards> MainNameCardss = new List<MainNameCards>();

        public delegate void AnyMsg(string Msg);

        public static AnyMsg SendAnyMsg;

        public static List<EnbConfigItems> EqualsAndMergeEnbConfig(string EnbAName,string EnbBName)
        {
            var GetAItems = ProcessEnb(EnbAName);
            var GetBItems = ProcessEnb(EnbBName);

            List<EnbConfigItems> TempBItems = new List<EnbConfigItems>();
            TempBItems.AddRange(GetBItems);

            string CurrentName = "";
            for (int ir = 0; ir < GetAItems.Count; ir++)
            {
                var GetA = GetAItems[ir];

                foreach (var GetB in GetBItems)
                {
                    if (GetA.MainName.Equals(GetB.MainName))
                    {
                        if (CurrentName != GetA.MainName)
                        {
                            CurrentName = GetA.MainName;
                            System.Windows.Forms.MessageBox.Show("开始处理级:" + CurrentName+"("+ GetChn(GetA.MainName) +")");
                        }
                        if (TempBItems.Contains(GetB))
                        {
                            TempBItems.Remove(GetB);
                        }

                        for (int i= 0; i < GetA.AllSetting.Count;i++)
                        {
                            var GetAItem = GetA.AllSetting[i];

                            if (GetAItem.Contains("="))
                            {
                                string GetAName = GetAItem.Split('=')[0];
                                string GetAValue = GetAItem.Split('=')[1];

                                foreach (var GetBItem in GetB.AllSetting)
                                {
                                    if (GetBItem.Contains("="))
                                    {
                                        string GetBName = GetBItem.Split('=')[0];
                                        string GetBValue = GetBItem.Split('=')[1];

                                        if (GetAName.Equals(GetBName))
                                        {
                                            if (!GetAValue.Equals(GetBValue))
                                            {
                                                //GetA.AllSetting[i] = GetBName + "=" + GetBValue;
                                                if (System.Windows.Forms.MessageBox.Show(string.Format("{0}\r\nA:{1}->B:{2}\r\n 是否应用改变到A?", GetA.MainName+"("+ GetChn(GetA.MainName) + ")", GetAName + "=" + GetAValue, GetBName + "=" + GetBValue), "发现不同", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                                {
                                                    GetA.AllSetting[i] = GetBName + "=" + GetBValue;
                                                }
                                            }
                                          
                                        }
                                    }
                                }
                            }
                         
                        }
                    }
                }

                GetAItems[ir] = GetA;
            }

            foreach (var Get in TempBItems)
            {
                GetAItems.Add(Get);
            }

           return GetAItems;
        }

        public static void Init()
        {
            MainNameCardss.Clear();
            MainNameCardss.Add(new MainNameCards("EFFECT", "效果"));
            MainNameCardss.Add(new MainNameCards("GLOBAL", "全局"));
            MainNameCardss.Add(new MainNameCards("COLORCORRECTION", "色彩校正"));
            MainNameCardss.Add(new MainNameCards("WEATHER", "天气"));
            MainNameCardss.Add(new MainNameCards("TIMEOFDAY", "时间(天)"));
            MainNameCardss.Add(new MainNameCards("ADAPTATION", "适应"));
            MainNameCardss.Add(new MainNameCards("DEPTHOFFIELD", "景深"));
            MainNameCardss.Add(new MainNameCards("BLOOM", "光晕效果"));
            MainNameCardss.Add(new MainNameCards("LENS", "镜头"));
            MainNameCardss.Add(new MainNameCards("SKY", "天空"));
            MainNameCardss.Add(new MainNameCards("ENVIRONMENT", "环境"));
            MainNameCardss.Add(new MainNameCards("SKYLIGHTING", "真实阴影普照"));
            MainNameCardss.Add(new MainNameCards("SSAO_GAME", "屏幕空间环境光遮蔽"));
            MainNameCardss.Add(new MainNameCards("SSAO_SSIL", "环境光遮蔽质量"));
            MainNameCardss.Add(new MainNameCards("OBJECT", "物体"));
            MainNameCardss.Add(new MainNameCards("VEGETATION", "植物"));
            MainNameCardss.Add(new MainNameCards("EYE", "视力"));
            MainNameCardss.Add(new MainNameCards("SKINSPECULAR", "皮肤的"));
            MainNameCardss.Add(new MainNameCards("WINDOWLIGHT", "窗玻璃"));
            MainNameCardss.Add(new MainNameCards("LIGHTSPRITE", "闪电"));
            MainNameCardss.Add(new MainNameCards("VOLUMETRICFOG", "容积雾"));
            MainNameCardss.Add(new MainNameCards("FIRE", "火焰"));
            MainNameCardss.Add(new MainNameCards("PARTICLE", "微粒"));
            MainNameCardss.Add(new MainNameCards("COMPLEXFIRELIGHTS", "复杂的火光"));
            MainNameCardss.Add(new MainNameCards("COMPLEXPARTICLELIGHTS", "复杂的粒子光"));
            MainNameCardss.Add(new MainNameCards("IMAGEBASEDLIGHTING", "基于图像的光照效果"));
            MainNameCardss.Add(new MainNameCards("RAIN", "雨"));
            MainNameCardss.Add(new MainNameCards("SUBSURFACESCATTERING", "次表面散射"));
            MainNameCardss.Add(new MainNameCards("REFLECTION", "反射"));
            MainNameCardss.Add(new MainNameCards("UNDERWATER", "水下"));
            MainNameCardss.Add(new MainNameCards("VOLUMETRICRAYS", "体积分析"));
            MainNameCardss.Add(new MainNameCards("LENSFLARE_GAME", "镜头光晕"));
            MainNameCardss.Add(new MainNameCards("PROCEDURALSUN", "程序太阳"));
            MainNameCardss.Add(new MainNameCards("CLOUDSHADOWS", "云影"));
            MainNameCardss.Add(new MainNameCards("SHADOW", "阴影"));
            MainNameCardss.Add(new MainNameCards("GAMEVOLUMETRICRAYS", "配子体积分析"));
            MainNameCardss.Add(new MainNameCards("GRASS", "草"));
            MainNameCardss.Add(new MainNameCards("WATER", "水"));
            MainNameCardss.Add(new MainNameCards("RAYS", "射线"));
            MainNameCardss.Add(new MainNameCards("SUNGLARE", "阳光"));
            MainNameCardss.Add(new MainNameCards("RAINWETSURFACES", "雨湿面"));
            MainNameCardss.Add(new MainNameCards("WETSURFACES", "潮湿面"));
            MainNameCardss.Add(new MainNameCards("NORMALMAPPINGSHADOWS", "法线贴图阴影"));
            MainNameCardss.Add(new MainNameCards("Quality", "质量"));
            MainNameCardss.Add(new MainNameCards("SNOW", "雪"));
        }

        public static string GetChn(string Value)
        {
            foreach (var Get in MainNameCardss)
            {
                if (Get.Name.Equals(Value))
                {
                    return Get.CHNName;
                }
            }

            return string.Empty;
        }

        public static string CreatEnbConfig(List<EnbConfigItems> Source)
        {
            string RichText = "";

            foreach (var Get in Source)
            {
                RichText += "[" + Get.MainName + "]\r\n";

                foreach (var GetLv1 in Get.AllSetting)
                {
                    RichText += GetLv1 + "\r\n";
                }
            }

            return RichText;
        }


        public static List<EnbConfigItems> ProcessEnb(string EnbName)
        {
            string ConfigPath = DeFine.GetFullPath(@"\ENB\") + EnbName + @"\enbseries.ini";//enbseries;

            List<EnbConfigItems> EnbConfigItems = new List<EnbConfigItems>();

            string CurrentName = "";
            List<string> AllSet = new List<string>();

            foreach (var Get in FileHelper.ReadFileByStr(ConfigPath,Encoding.UTF8).Split(new char[2] {'\r','\n'}))
            {
                if (Get.Trim().Length > 0)
                {
                    if (Get.Contains("[")&& Get.Contains("]"))
                    {
                        if (AllSet.Count > 0)
                        {
                            EnbConfigItems.Add(new EnbConfigItems(CurrentName,AllSet));
                            AllSet = new List<string>();
                        }
                        CurrentName = ConvertHelper.StringDivision(Get, "[", "]");
                    }
                    else
                    {
                        AllSet.Add(Get);
                    }
                }
            }

            if (CurrentName.Trim().Length > 0)
            {
                EnbConfigItems.Add(new EnbConfigItems(CurrentName, AllSet));
            }

            return EnbConfigItems;
        }
        public static bool ImportEnb(string Path, string EnbName)
        {
            try
            {
                string TargetPath = DeFine.GetFullPath(@"\ENB\");

                if (!Directory.Exists(TargetPath))
                {
                    Directory.CreateDirectory(TargetPath);
                }

                if (Path.EndsWith(@"\"))
                {
                    Path = Path.Substring(0, Path.Length - 1);
                }

                //string GetLastDirName = Path.Substring(Path.LastIndexOf(@"\")+ @"\".Length);

                if (!Directory.Exists(TargetPath + EnbName))
                FileHelper.CopyDir(Path, TargetPath + EnbName);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public static List<string> GetENBs()
        {
            //List<string> Enbs = new List<string>();

            //var EnbStruct = FileHelper.GetPathStruct(DeFine.GetFullPath(@"\ENB\"));

            //foreach (var Get in EnbStruct)
            //{
            //    if (Get.ScanDeep == 1)
            //    {
            //        Enbs.Add(Get.CurrentPath);
            //    }
            //}

            //return Enbs;

            return null;
        }

        public static bool UsingEnb(string Name, string TargetPath)
        {
            //if (Directory.Exists(DeFine.GetFullPath(@"\ENB\" + Name)))
            //{
            //    OneEnb NOneEnb = new OneEnb(FileHelper.GetPathStruct(DeFine.GetFullPath(@"\ENB\" + Name)),Name);
            //    string Content = JsonManager.JsonCore.CreatJson(NOneEnb);

            //    if (File.Exists(TargetPath + @"\ENB.config"))
            //    {
            //        CancelEnb(FileHelper.ReadFileByStr(TargetPath + @"\ENB.config", Encoding.UTF8), TargetPath);
            //    }

            //    FileHelper.WriteFile(TargetPath + @"\ENB.config", Content, Encoding.UTF8);

            //    try
            //    {
            //        foreach (var Get in NOneEnb.FileTree.Files)
            //        {
            //            if (File.Exists(TargetPath + @"\" + Get.CurrentPath))
            //            {
            //                File.Delete(TargetPath + @"\" + Get.CurrentPath);
            //                SendAnyMsg.Invoke("Deleted:"+ TargetPath + @"\" + Get.CurrentPath);
            //            }

            //            if (File.Exists(Get.SourcePath))
            //            {
            //                File.Copy(Get.SourcePath, TargetPath + @"\" + Get.CurrentPath);
            //                SendAnyMsg.Invoke("Writed:" + TargetPath + @"\" + Get.CurrentPath);
            //            }
            //            else
            //            {
            //                if (Directory.Exists(Get.SourcePath))
            //                {
            //                    if (!Directory.Exists(TargetPath + @"\" + Get.CurrentPath))
            //                    {
            //                        Directory.CreateDirectory(TargetPath + @"\" + Get.CurrentPath);
            //                        SendAnyMsg.Invoke("Created:" + TargetPath + @"\" + Get.CurrentPath);
            //                    }
            //                }
            //            }
            //        }
            //        return true;
            //    }
            //    catch (Exception Ex)
            //    {
            //        System.Windows.Forms.MessageBox.Show(Ex.Message);
            //    }

            //}

            return false;
        }

        public static bool CancelEnb(string JsonContent, string TargetPath)
        {
            try
            {
                //OneEnb NOneEnb = JsonManager.JsonCore.JsonParse<OneEnb>(JsonContent);

                //if (NOneEnb.EnbName.Trim().Length > 0)
                //{
                //    foreach (var Get in NOneEnb.FileTree)
                //    {
                //        if (File.Exists(TargetPath + @"\" + Get.CurrentPath))
                //        {
                //            File.Delete(TargetPath + @"\" + Get.CurrentPath);
                //            SendAnyMsg.Invoke("Deleted:" + TargetPath + @"\" + Get.CurrentPath);
                //        }
                //        else
                //        {
                //            if (Directory.Exists(TargetPath + @"\" + Get.CurrentPath))
                //            {
                //                if (FileHelper.GetPathStruct(TargetPath + @"\" + Get.CurrentPath).Count == 0)
                //                {
                //                    Directory.Delete(TargetPath + @"\" + Get.CurrentPath);
                //                    SendAnyMsg.Invoke("DeletedByPath:" + TargetPath + @"\" + Get.CurrentPath);
                //                }
                //            }
                //        }
                          
                //    }
                //}

                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class OneEnb
    {
        public STreeItem FileTree = new STreeItem();
        public string EnbName = "";

        public OneEnb() { }
        public OneEnb(STreeItem FileTree, string EnbName)
        {
            this.FileTree = FileTree;
            this.EnbName = EnbName;
        }
    }

    public class MainNameCards
    {
        public string Name = "";
        public string CHNName = "";

        public MainNameCards(string Name, string CHNName)
        {
            this.Name = Name;
            this.CHNName = CHNName;
        }
    }

    public class EnbConfigItems
    {
        public string MainName = "";
        public List<string> AllSetting = new List<string>();

        public EnbConfigItems(string MainName, List<string> AllSetting)
        {
            this.MainName = MainName;
            this.AllSetting = AllSetting;
        }
    }
}
