
using JsonManager;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net.Sockets;
using YDSkyrimTools.ConvertManager;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using YDSkyrimTools.RequestCore;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Serialization;
using System.Windows.Shapes;
using System.Web.UI.WebControls;

namespace YDSkyrimTools.TranslateCore
{

    [DataContract]
    public class AdmAccessToken
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string token_type { get; set; }
        [DataMember]
        public string expires_in { get; set; }
        [DataMember]
        public string scope { get; set; }
    }

    public class AdmAuthentication
    {
        public static readonly string DatamarketAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
        private string clientId;
        private string cientSecret;
        private string request;

        public AdmAuthentication(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.cientSecret = clientSecret;
            //If clientid or client secret has special characters, encode before sending request
            this.request = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(clientSecret));
        }

        public AdmAccessToken GetAccessToken()
        {
            return HttpPost(DatamarketAccessUri, this.request);
        }

        private AdmAccessToken HttpPost(string DatamarketAccessUri, string requestDetails)
        {
            //Prepare OAuth request
            WebRequest webRequest = WebRequest.Create(DatamarketAccessUri);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
            webRequest.ContentLength = bytes.Length;
            using (Stream outputStream = webRequest.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));
                //Get deserialized object from JSON stream
                AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());
                return token;
            }
        }
    }



    public class YouDaoTApi
    {
        public string GetCNByYouDao(string q, string From = "en", string To = "zh-CHS")
        {
            if (!new LanguageHelper().IsEnglishChar(q)) return q;
            try
            {
                string result = "";
                string url = "http://fanyi.youdao.com/translate_o?smartresult=dict&smartresult=rule/";
                string u = "fanyideskweb";
                string c = "Y2FYu%TNSbMCxc3t2u^XT";
                TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
                long millis = (long)ts.TotalMilliseconds;
                string curtime = Convert.ToString(millis);
                Random rd = new Random();
                string f = curtime + rd.Next(0, 9);
                string signStr = u + q + f + c;
                string sign = GetMd5Str_32(signStr);
                Dictionary<String, String> dic = new Dictionary<String, String>();
                dic.Add("i", q);
                dic.Add("from", From);
                dic.Add("to", To);
                dic.Add("smartresult", "dict");
                dic.Add("client", "fanyideskweb");
                dic.Add("salt", f);
                dic.Add("sign", sign);
                dic.Add("lts", curtime);
                dic.Add("bv", GetMd5Str_32("5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36"));
                dic.Add("doctype", "json");
                dic.Add("version", "2.1");
                dic.Add("keyfrom", "fanyi.web");
                dic.Add("action", "FY_BY_REALTlME");
                //dic.Add("typoResult", "false");

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Timeout = 3000;
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                req.Referer = "http://fanyi.youdao.com/";
                req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36";
                req.Headers.Add("Cookie", "OUTFOX_SEARCH_USER_ID=-2030520936@111.204.187.35; OUTFOX_SEARCH_USER_ID_NCOO=798307585.9506682; UM_distinctid=17c2157768a25e-087647b7cf38e8-581e311d-1fa400-17c2157768b8ac; P_INFO=15711476666|1632647789|1|youdao_zhiyun2018|00&99|null&null&null#bej&null#10#0|&0||15711476666; JSESSIONID=aaafZvxuue5Qk5_d9fLWx; ___rl__test__cookies=" + curtime);
                StringBuilder builder = new StringBuilder();
                int i = 0;
                foreach (var item in dic)
                {
                    if (i > 0)
                        builder.Append("&");
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
                byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
                req.ContentLength = data.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string GetData = reader.ReadToEnd();
                    JObject jo = (JObject)JsonConvert.DeserializeObject(GetData);
                    if (jo.Value<string>("errorCode").Equals("0"))
                    {
                        var tgtarray = jo.SelectToken("translateResult").First().Values<string>("tgt").ToArray();
                        result = string.Join("", tgtarray);
                    }
                }

                WordProcess.SendTranslateMsg("翻译引擎(有道)", q, result);

                return result;
            }
            catch { return q; }
        }

        public string GetMd5Str_32(string encryptString)
        {
            byte[] result = Encoding.UTF8.GetBytes(encryptString);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            string encryptResult = BitConverter.ToString(output).Replace("-", "");
            return encryptResult;
        }
    }
    public class GoogleTApi
    {
        public static bool CanTranslate = true;

        public static bool LockerGoogleListenService = false;
        public static void StartGoogleListenService(bool Check)
        {
            if (Check)
            {
                if (!LockerGoogleListenService)
                {
                    LockerGoogleListenService = true;
                }

                new Thread(() => 
                {
                    int Number = 0;

                    while (LockerGoogleListenService)
                    {
                        Thread.Sleep(100);

                        if (!CanTranslate)
                        {
                            Number++;
                            if (Number >= 30)
                            {
                                Number = 0;
                                CanTranslate = true;
                            }
                        }
                        else
                        { 
                        
                        }
                      
                    }
                }).Start();
            }
            else
            {
                LockerGoogleListenService = false;
            }
        }
        public string GoogleTranslate(string Str)
        {
            if (Str.Contains("-"))
            {
                string RichText = "";
                foreach (var GetChar in Str.Split('-'))
                {
                    if (GetChar.Trim().Length > 0)
                    {
                        string GetGGMsg = new GoogleTApi().GetCNByGoogle(GetChar);

                        if (GetGGMsg.Trim().Length > 0)
                        {
                            WordProcess.SendTranslateMsg("翻译引擎(谷歌)(3秒冷却)", Str, GetGGMsg);
                            RichText += GetGGMsg + "-";
                        }
                        else
                        {
                            RichText += GetChar + "-";
                        }
                    }
                }

                if (RichText.EndsWith("-"))
                {
                    RichText = RichText.Substring(0, RichText.Length - 1);
                    return RichText;
                }
                else
                {
                    return Str;
                }
            }
            else
            {
                string GetGGMsg = new GoogleTApi().GetCNByGoogle(Str);

                if (GetGGMsg.Trim().Length > 0)
                {
                    WordProcess.SendTranslateMsg("翻译引擎(谷歌)(3秒冷却)", Str, GetGGMsg);
                    TranslateDBCache.AddCache(Str, 0, 1, GetGGMsg);
                    return GetGGMsg;
                }
                else
                {
                    return Str;
                }
            }
        }
        public string GetGoogleTranslate(string Str)
        {
            if (Str.Length < 230)
            {
                while (!GoogleTApi.CanTranslate)
                {
                    Thread.Sleep(10);
                }

                GoogleTApi.CanTranslate = false;

                return GoogleTranslate(Str);
            }
            else
            {
                string MergeStr = "";

                Str = Str.Replace("，", ",");

                if (Str.Contains(","))
                {
                    foreach (var Get in Str.Split(','))
                    {
                        MergeStr += GoogleTranslate(Get) + "，";
                        Thread.Sleep(3000);
                    }

                    if (MergeStr.EndsWith("，"))
                    {
                        MergeStr = MergeStr.Substring(0, MergeStr.Length - 1);
                    }
                }
                else
                {
                    return Str;
                }

                return MergeStr;
            }
        }
        public string GetCNByGoogleFree(string Text)
        {
            try
            {
                var defaultHandler = new HttpClientHandler();
                var fragileProxy = defaultHandler.Proxy;
                var httpClient = new HttpClient(defaultHandler);
                var Result = httpClient.GetAsync(string.Format("https://translate.googleapis.com/translate_a/single?client=gtx&dt=t&sl=en&tl=zh-CN&q='{0}'".Replace("'", "\""), HttpUtility.HtmlEncode(Text)));
                Task<string> ThisTask = Result.Result.Content.ReadAsStringAsync();
                ThisTask.Wait();
                return ThisTask.Result;
            }
            catch { return String.Empty; }
        }
        public string GetCNByGoogle(string En)
        {
            if (DeFine.GlobalLocalSetting.GoogleKey.Trim().Length == 0)
            {
                string GetContent = GetCNByGoogleFree(En);

                if (GetContent == "")
                {
                    return En;
                }

                GoogleFItem OneFItem = new GoogleFItem(En,GetContent);
                    
                return OneFItem.GContent;
            }

            return string.Empty;
        }
    }

    public class GoogleFItem
    {
        public string GContent;
        public GoogleFItem(string DefContent,string Content)
        {
            if (new WordProcess().HasChinese(Content))
            {
                NextCall:
                Content = Content.Trim();
                if (Content.StartsWith("["))
                {
                    Content = Content.Substring(1);
                    if (Content.EndsWith("]"))
                    {
                        Content = Content.Substring(0, Content.Length - 1);
                    }
                    goto NextCall;
                }

                if (Content.StartsWith("\""))
                {
                    if (Content.Contains(DefContent))
                    {
                        Content = Content.Substring(0, Content.IndexOf(DefContent));
                    }
                    else
                    {
                        if (Content.Contains("\",\"\\\""))
                        {
                            Content = Content.Substring(0, Content.LastIndexOf("\",\"\\\""));
                        }
                        else
                        {
                            GContent = DefContent;
                            return;
                        }
                    }
                }

                NextGet:
                if (Content.StartsWith("\""))
                {
                    Content = Content.Substring(1);

                    if (Content.EndsWith("\""))
                    {
                        Content = Content.Substring(0, Content.Length - 1);
                    }

                    goto NextGet;
                }

                Content = Content.Trim();
                if (Content.StartsWith("“"))
                {
                    Content = Content.Substring(1);
                }
                if (Content.Contains(","))
                {
                    Content = Content.Substring(0, Content.LastIndexOf(","));
                }

                Content = Content.Trim();

                if (Content.EndsWith("”\""))
                {
                    Content = Content.Substring(0, Content.Length - "”\"".Length);
                }
                Content = Content.Trim();
                GContent = Content;
            }
            else
            {
                GContent =  DefContent;
            }
        }
    }



    public class LanguageHelper
    {
        public enum LanguageType
        {
            zh,//中文    
            en,//英文    
            ja,//日文    
            ko,//韩文    
            fr,//法文    
            es,//西班牙文
            pt,//葡萄牙文
            it,//意大利文
            ru,//俄文    
            vi,//越南文    
            de,//德文    
            ar,//阿拉伯文
            id,//印尼文    
            auto,//自动识别
        }


        public bool IsEnglishChar(string strValue)
        {
            string[] Chars = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

            string TempContent = strValue.ToLower();
            foreach (var Get in Chars)
            {
                if (TempContent.Contains(Get.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        public string ProcessCodeSign(string Line,WordTProcess ProcessItems)
        {
            return Line;

            //List<string> AllCode = new List<string>();

            //foreach (var Get in ProcessItems.AllLine)
            //{
            //    if (Get.Code.Trim().Length > 0)
            //    {
            //        AllCode.Add(Get.Code);
            //    }
            //}

            //string TempSign = "";
            //int Offset = -1;

            //string NewLine = "";

            //for (int i = 0; i < Line.Length; i++)
            //{
            //    string GetChar = Line.Substring(i,1);

            //    if (GetChar == "_")
            //    {
            //        TempSign += GetChar;

            //        if (TempSign.Length == 3)
            //        {
            //            Offset++;

            //            TempSign = "";

            //            if (Offset < AllCode.Count)
            //            {
            //                NewLine += AllCode[Offset];
            //            }
            //        }
            //    }
            //    else
            //    {
            //        NewLine += GetChar;
            //        TempSign = "";
            //    }
            //}

            //return NewLine;
        }

        public static string OptimizeString(string Str)
        {
            return Str.Replace("（", "(").Replace("）", ")").Replace("【", "[").Replace("】", "]").Replace("一个一个", "一个").Replace("一个一对", "一对").Replace("一个一套", "一套").Replace("一个一双", "一双");
        }

        public string EnglishToCN(string Str)
        {
            if (Str == "")
            {
                return "";
            }

            string GetCacheStr = TranslateDBCache.FindCache(Str, 0, 1);

            if (GetCacheStr.Trim().Length > 0)
            {
                WordProcess.SendTranslateMsg("从数据库缓存", Str, GetCacheStr);
                return GetCacheStr;
            }

            int AutoSelect = 0;

            if (!DeFine.BaiDuYunApiUsing)
            {
                AutoSelect = 1;
            }
            else
            {
                AutoSelect = 0;
            }

            if (AutoSelect == 1)
            {
                if (DeFine.YouDaoYunApiUsing)
                {
                    WordTProcess OneProcess = new WordTProcess(Str);

                    string RichText = "";

                    foreach (var Get in OneProcess.AllLine)
                    {
                        string GetStr = new YouDaoTApi().GetCNByYouDao(Get.TextMsg);

                        if (GetStr != Get.TextMsg)
                        {
                            if (GetStr.Trim().Length > 0)
                            {
                                RichText += GetStr.Replace("___","") + Get.Code + Get.Char;
                            }
                        }
                        else
                        {
                            RichText += Get.TextMsg.Replace("___", "") + Get.Code + Get.Char;
                        }
                    }

                    RichText = OneProcess.StartContent + RichText;

                    if (new WordProcess().HasChinese(RichText))
                    {
                        TranslateDBCache.AddCache(Str, 0, 1, RichText);
                    }

                    return RichText;
                }

                if (DeFine.GoogleYunApiUsing)
                {
                    WordTProcess OneProcess = new WordTProcess(Str);

                    string CreatTransLine = "";

                    foreach (var Get in OneProcess.AllLine)
                    {
                        CreatTransLine += Get.TextMsg + Get.Char;
                    }

                    string GetChn = new GoogleTApi().GetGoogleTranslate(CreatTransLine);

                    GetChn = ProcessCodeSign(GetChn, OneProcess);

                    GetChn = OneProcess.StartContent + GetChn;

                    if (new WordProcess().HasChinese(GetChn))
                    {
                        TranslateDBCache.AddCache(Str, 0, 1, GetChn);
                    }

                    return GetChn;
                }
            }
            else
            {
                string RichText = "";

                WordTProcess OneProcess = new WordTProcess(Str);

                string CreatTransLine = "";

                foreach (var Get in OneProcess.AllLine)
                {
                    CreatTransLine += Get.TextMsg + Get.Char;
                }

                if(CreatTransLine.Length>0)
                {
                    string GetChn = new BaiDuRequest().GetFreeTranslation(CreatTransLine);

                    if (GetChn.Replace(" ", "").Equals(CreatTransLine.Replace(" ", "")))
                    {
                        GetChn = "";
                    }

                    if (GetChn.Trim() == "")
                    {
                        var GetApiMsg = EnglishTo(CreatTransLine, LanguageType.en, LanguageType.zh);
                        BaiDuApi OneMsg = new BaiDuApi();

                        bool IsTrue = false;
                        try
                        {
                            OneMsg = JsonManager.JsonCore.JsonParse<BaiDuApi>(GetApiMsg);
                            if (OneMsg.trans_result == null == false)
                            {
                                if (OneMsg.trans_result.Length > 0)
                                {
                                    DeFine.GlobalLocalSetting.TransCount += CreatTransLine.Length;

                                    string GetStr = HttpUtility.UrlDecode(OneMsg.trans_result[0].dst);

                                    RichText = GetStr;
                                    WordProcess.SendTranslateMsg("翻译引擎(百度云API)", CreatTransLine, GetStr);
                                    RichText = ProcessCodeSign(RichText, OneProcess);
                                    IsTrue = true;
                                }
                            }
                        }
                        catch { }

                        if (!IsTrue)
                        {
                            RichText = CreatTransLine;
                            RichText = ProcessCodeSign(RichText, OneProcess);
                        }

                    }
                    else
                    {
                        RichText = GetChn;
                        WordProcess.SendTranslateMsg("翻译引擎(百度云)", CreatTransLine, GetChn);
                        RichText = ProcessCodeSign(RichText, OneProcess);
                        RichText = OneProcess.StartContent + RichText;
                    }
                }

                
                string GetMsg = RichText.Replace("。", ".").Replace("，", ",");

                GetMsg = OneProcess.StartContent + GetMsg;

                if (new WordProcess().HasChinese(GetMsg))
                {
                    TranslateDBCache.AddCache(Str, 0, 1, GetMsg);
                }   

                return GetMsg;
            }

            return "";
        }
        public string EnglishTo(string Str, LanguageType SourceType, LanguageType TargetType = LanguageType.zh)
        {
            try
            {
                string From = SourceType.ToString();
                string To = TargetType.ToString();
                string AppId = DeFine.GlobalLocalSetting.BaiDuAppID;
                Random rd = new Random();
                string salt = rd.Next(100000).ToString();
                string secretKey = DeFine.GlobalLocalSetting.BaiDuSecretKey;
                string sign = EncryptString(AppId + Str + salt + secretKey);
                string url = "http://api.fanyi.baidu.com/api/trans/vip/translate?";
                url += "q=" + HttpUtility.UrlEncode(Str);
                url += "&from=" + From;
                url += "&to=" + To;
                url += "&appid=" + DeFine.GlobalLocalSetting.BaiDuAppID;
                url += "&salt=" + salt;
                url += "&sign=" + sign;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.UserAgent = null;
                request.Timeout = 3000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return retString;
            }
            catch { return string.Empty; }
        }

        public string EncryptString(string str)
        {
            MD5 md5 = MD5.Create();
            // 将字符串转换成字节数组
            byte[] byteOld = Encoding.UTF8.GetBytes(str);
            // 调用加密方法
            byte[] byteNew = md5.ComputeHash(byteOld);
            // 将加密结果转换为字符串
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew)
            {
                // 将字节转换成16进制表示的字符串，
                sb.Append(b.ToString("x2"));
            }
            // 返回加密的字符串
            return sb.ToString();
        }

        public List<string> AllEng = new List<string>() {
            "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"
        };



    }


    public class BaiDuApi
    {
        public string from { get; set; }
        public string to { get; set; }
        public Trans_Result[] trans_result { get; set; }
    }

    public class Trans_Result
    {
        public string src { get; set; }
        public string dst { get; set; }
    }

    public class CharLineExtend
    {
        public string TextMsg = "";
        public string Char = "";
        public string Code = "";

        public CharLineExtend(string Msg, string Char,string Code)
        {
            this.TextMsg = Msg;
            this.Char = Char;
            this.Code = Code;
        }
    }

    public class WordTProcess
    {
        public string Content = "";
        public string StartContent = "";

        public List<CharLineExtend> AllLine = new List<CharLineExtend>();

        public WordTProcess(string Content)
        {
            this.Content = Content;

            this.Content = this.Content.Replace("(", "（");
            this.Content = this.Content.Replace(")", "）");
         

            this.Content = this.Content.Replace("（（", "☯（");
            this.Content = this.Content.Replace("））", "）㊣");
            this.Content = this.Content.Replace("（ （", "☯（");
            this.Content = this.Content.Replace("） ）", "）㊣");
            this.Content = this.Content.Replace("（  （", "☯（");
            this.Content = this.Content.Replace("）  ）", "）㊣");


            bool IsCode = false;
            string CodeStr = "";
            string RichText = "";

            for (int i = 0; i < this.Content.Length; i++)
            {
                string GetChar = this.Content.Substring(i, 1);

                if (GetChar == "）" || GetChar == ")")
                {
                    IsCode = false;

                    if (CodeStr.Trim().Length > 0)
                    {
                        AllLine.Add(new CharLineExtend(string.Empty, string.Empty, CodeStr));
                        CodeStr = "";
                    }

                    GetChar = "";
                }
                else
                if (IsCode)
                {
                    CodeStr += GetChar;
                    GetChar = "";
                }
                else
                if (GetChar == "（" || GetChar == "(")
                {
                    IsCode = true;

                    if (RichText.Trim().Length > 0)
                    {
                        AllLine.Add(new CharLineExtend(RichText, string.Empty, string.Empty));
                        RichText = "";
                    }
                    else
                    {
                        RichText = "";
                    }

                    GetChar = "";
                }
               

                if (GetChar == "-")
                {
                    AllLine.Add(new CharLineExtend(RichText,GetChar,CodeStr));
                    RichText = "";
                    GetChar = "";
                }
                else
                if (GetChar == ".")
                {
                    AllLine.Add(new CharLineExtend(RichText, GetChar,CodeStr));
                    RichText = "";
                    GetChar = "";
                }
                else
                if (GetChar == ",")
                {
                    AllLine.Add(new CharLineExtend(RichText, GetChar,CodeStr));
                    RichText = "";
                    GetChar = "";
                }
                else
                if (GetChar == ";")
                {
                    AllLine.Add(new CharLineExtend(RichText, GetChar,CodeStr));
                    RichText = "";
                    GetChar = "";
                }
                else
                {
                    RichText += GetChar;
                }
            }

            if (RichText.Trim().Length > 0)
            {
                AllLine.Add(new CharLineExtend(RichText, string.Empty,CodeStr));
                RichText = "";
            }

            for (int i = 0; i < AllLine.Count; i++)
            {
                if (AllLine[i].Code.Trim().Length > 0)
                {
                    if (ConvertHelper.ObjToInt(AllLine[i].Code.Trim()) > 0)
                    {
                        AllLine[i].TextMsg = "（" + AllLine[i].Code + "）";
                        AllLine[i].Code = "（" + AllLine[i].Code + "）";
                    }
                    else
                    {
                        AllLine[i].TextMsg = "（" + AllLine[i].Code + "）";
                        AllLine[i].Code = "";
                    }
                }
            }

            for (int i = 0; i < AllLine.Count; i++)
            {
                if (AllLine[i].TextMsg.Contains("☯") || AllLine[i].TextMsg.Contains("㊣"))
                {
                    AllLine[i].TextMsg = AllLine[i].TextMsg.Replace("☯", "(").Replace("㊣", ")");
                }
            }
        }
    }






    public class QuickSearch
    {
        public string Key = "";
        public string Value = "";

        public QuickSearch(string Key, string Value)
        {
            this.Key = Key;
            this.Value = Value;
        }

    }

}
