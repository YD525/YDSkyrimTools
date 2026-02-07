using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Controls;
using YDSkyrimTools.SkyrimModManager;


namespace YDSkyrimTools.TranslateCore
{
    public class TranslateHelper
    {
        public static Dictionary<string, string> Caches = new Dictionary<string, string>();

        public static string TranslateToCN(string ENText)
        {
            var Str = System.Web.HttpUtility.UrlEncode(ENText);
            string Url = string.Format("https://ajax.googleapis.com/ajax/services/language/translate?v=1.0&q={0}&langpair=zh|en", Str);

            HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create(Url);
            HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();

            System.IO.Stream Stream = Response.GetResponseStream();
            System.IO.StreamReader StreamReader = new System.IO.StreamReader(Stream);

            var Result = StreamReader.ReadToEnd();

            StreamReader.Close();
            Stream.Close();
            Response.Close();

            return Result;
        }

  

  
    

        public static void UsingTxtCache(string SourcePath)
        {
            var GetContent = FileHelper.ReadFileByStr(SourcePath,Encoding.UTF8);

            foreach (var Get in GetContent.Split('$'))
            {
                if (Get.Trim().Length > 0)
                {
                    string GetLine = '$' + Get;

                    if (GetLine.Contains("\t"))
                    {
                        string Key = GetLine.Split('\t')[0].Trim();
                        if (!Caches.ContainsKey(Key))
                        Caches.Add(Key, GetLine.Substring(GetLine.IndexOf("\t")+ "\t".Length));
                    }
                }
            }
        }
        public static void UPDateByCacheTxt(string SourcePath)
        {
            var GetContent = FileHelper.ReadFileByStr(SourcePath, Encoding.UTF8);

            string RichText = "";
            foreach (var Get in GetContent.Split(new char[2] { '\r', '\n' }))
            {
                if (Get.Trim().Length > 0)
                {
                    string GetLine = Get;

                    if (GetLine.Contains("\t"))
                    {
                        string GetKey = GetLine.Split('\t')[0].Trim();

                        if (Caches.ContainsKey(GetKey))
                        {
                            GetLine = string.Format("{0}\t {1}",GetKey, Caches[GetKey]);
                        }
                    }

                    RichText += GetLine + "\r\n";
                }
            }
        }
    }
}
