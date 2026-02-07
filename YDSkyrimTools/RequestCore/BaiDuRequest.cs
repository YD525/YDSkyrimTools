
using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Core;

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
using YDSkyrimTools.ConvertManager;
using YDSkyrimTools.DataManager;
using YDSkyrimTools.SkyrimModManager;
using YDSkyrimTools.TranslateCore;
using static System.Net.WebRequestMethods;

namespace YDSkyrimTools.RequestCore
{
    public class BaiDuRequest
    {
        public static RequestAddress BaiDuGetWebPage = new RequestAddress("https://fanyi.baidu.com/", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9", "text/html; charset=utf-8", "", RequestType.HttpGet);

        public static RequestAddress BaiDuGetTranslation = new RequestAddress("https://fanyi.baidu.com/v2transapi?from=en&to=zh", "*/*", "application/x-www-form-urlencoded; charset=UTF-8", "?from=&to=&query=&transtype=&simple_means_flag=&sign=&token=&domain=", RequestType.HttpPost);

        public static RequestAddress BaiDuLangdetect = new RequestAddress("https://fanyi.baidu.com/langdetect", "*/*", "application/x-www-form-urlencoded; charset=UTF-8", "?query=", RequestType.HttpPost);
        public static void ConvertToBaiDuApi(ref RequestAddress Any)
        {
            Any.Headers.Clear();
            Any.Headers.Add("Origin", "https://fanyi.baidu.com");
            Any.Referer = "https://fanyi.baidu.com/";
            Any.Headers.Add("sec-ch-ua", "Google Chrome");
            Any.Headers.Add("sec-ch-ua-platform", "Windows");
            Any.Headers.Add("sec-ch-ua-mobile", "?0");
            Any.Headers.Add("Sec-Fetch-Dest", "empty");
            Any.Headers.Add("Sec-Fetch-Mode", "cors");
            Any.Headers.Add("Sec-Fetch-Site", "same-origin");
        }

        public static void ConvertToBaiDUWebPage(ref RequestAddress Any)
        {
            Any.Headers.Clear();
            Any.Referer = string.Empty;
            Any.Headers.Add("sec-ch-ua", "Google Chrome\";v=\"107\", \"Chromium\";v=\"107\", \"Not=A?Brand\";v=\"24");
            Any.Headers.Add("sec-ch-ua-mobile", "?0");
            Any.Headers.Add("sec-ch-ua-platform", "Windows");
            Any.Headers.Add("Sec-Fetch-Dest", "document");
            Any.Headers.Add("Upgrade-Insecure-Requests", "1");
        }


        public static BaiDuInFo GetBaiDUWebPage()
        {
            RequestAddress NBaiDuGetWebPage = new RequestAddress();
            NBaiDuGetWebPage = BaiDuGetWebPage;
            ConvertToBaiDUWebPage(ref NBaiDuGetWebPage);

            NextCall:

            var GetMsg = RequestHelper.CallRequest(NBaiDuGetWebPage);

            string GetGtk = ConvertHelper.StringDivision(GetMsg.HtmlCode, "window.gtk = \"", "\"");
            string GetToken = ConvertHelper.StringDivision(GetMsg.HtmlCode, "token: '", "'");

            if (GetToken.Trim().Length == 0)
            {
                NBaiDuGetWebPage.Cookies.Cookies = GetMsg.Cookies.Cookies;
                goto NextCall;
            }

            return new BaiDuInFo(NBaiDuGetWebPage.Cookies, GetToken, GetGtk, "", "");
        }

        public static DetectMsg Langdetect(ref BaiDuInFo OneInFo, string Query)
        {
            RequestAddress NBaiDuLangdetect = new RequestAddress();
            NBaiDuLangdetect = BaiDuLangdetect;
            ConvertToBaiDuApi(ref NBaiDuLangdetect);
            NBaiDuLangdetect.IsPayLoad = true;
            NBaiDuLangdetect.Param.SetItem("query", Query);
            NBaiDuLangdetect.Cookies.Cookies = OneInFo.Cookies.Cookies;
            var GetMsg = RequestHelper.CallRequest(NBaiDuLangdetect);
            
            return JsonManager.JsonCore.JsonParse<DetectMsg>(GetMsg.HtmlCode);
        }
        public string GetFreeTranslation(string Query, string From = "en", string To = "zh")
        {
            if (!new LanguageHelper().IsEnglishChar(Query)) return Query;

            int MaxTry = 2;

            var GetTokenItem = GetToken(Query);

            RequestAddress NBaiDuGetTranslation = new RequestAddress();
            NBaiDuGetTranslation = BaiDuGetTranslation;
            ConvertToBaiDuApi(ref NBaiDuGetTranslation);

            NextCall:
       
            NBaiDuGetTranslation.IsPayLoad = true;
           
            DetectMsg GetDetectMsg = Langdetect(ref GetTokenItem, Query);

            NBaiDuGetTranslation.Param.SetItem("from", GetDetectMsg.lan);
            NBaiDuGetTranslation.Param.SetItem("to", To);
            NBaiDuGetTranslation.Param.SetItem("query", Query);
            NBaiDuGetTranslation.Param.SetItem("transtype", "realtime");
            NBaiDuGetTranslation.Param.SetItem("simple_means_flag", "3");
            NBaiDuGetTranslation.Param.SetItem("sign", GetTokenItem.Sign);
            NBaiDuGetTranslation.Param.SetItem("token", GetTokenItem.Token);
            NBaiDuGetTranslation.Param.SetItem("domain", "common");
            NBaiDuGetTranslation.Cookies.Cookies = GetTokenItem.Cookies.Cookies;

            var RequestMsg = RequestHelper.CallRequest(NBaiDuGetTranslation);
            TransMsg GetMsg = JsonManager.JsonCore.JsonParse<TransMsg>(RequestMsg.HtmlCode);
          

            if (GetMsg.trans_result == null)
            {
                if (MaxTry > 0)
                {
                    MaxTry--;
                }
                else
                {
                    return "";
                }

                GetTokenItem = GetToken(Query);

                var a1 = "1234567887654321";
                var ae = TimeHelper.GetTimeStamp();
                var a2 = "{'ua':'" + RequestHelper.UA + "','url':" + "https://fanyi.baidu.com/#zh/en/" + ConvertHelper.UrlEnCode(Query) + "','platform':'Win32','clientTs':" + ae + ",'version':'2.2.0'}";
                string Acs =(ae-20000000) + "_" + ae + "_" + PINHelper.AESEncrypt(a2, a1);
                ConvertToBaiDuApi(ref NBaiDuGetTranslation);
                NBaiDuGetTranslation.Headers.Add("Acs-Token", Acs);

                goto NextCall;
            }

            if (GetMsg.trans_result.data == null)
            {
                GetTokenItem = GetToken(Query);
                if (MaxTry > 0)
                {
                    MaxTry--;
                }
                else
                {
                    return "";
                }
                goto NextCall;
            }

            if (GetMsg.trans_result.data.Length == 0)
            {
                GetTokenItem = GetToken(Query);
                if (MaxTry > 0)
                {
                    MaxTry--;
                }
                else
                {
                    return "";
                }
                goto NextCall;
            }

            return GetMsg.trans_result.data[0].dst;
        }

        public BaiDuInFo GetToken(string Query)
        {
            var GetKey = GetBaiDUWebPage();
            string CurrentToken = GetKey.Token;
            string CurrentGtk = GetKey.GTK;
            GetBaiDUWebPage();
            string JsText = DataHelper.ReadFileByStr(DeFine.GetFullPath(@"\BaiDuSigner.js"), Encoding.UTF8);
            IJsEngineSwitcher engineSwitcher = JsEngineSwitcher.Current;
            engineSwitcher.EngineFactories.Add(new ChakraCoreJsEngineFactory());
            engineSwitcher.DefaultEngineName = ChakraCoreJsEngine.EngineName;
            using (IJsEngine engine = JsEngineSwitcher.Current.CreateDefaultEngine())
            {
                engine.Execute(JsText);
                GetKey.Query = Query;
                GetKey.Sign = engine.CallFunction<string>("token", Query, CurrentGtk);
                return GetKey;
            }
        }

    }


    public class TransMsg
    {
        public Trans_Result trans_result { get; set; }

    }

    public class Trans_Result
    {
        public Datum[] data { get; set; }
        public string from { get; set; }
        public int status { get; set; }
        public string to { get; set; }
        public int type { get; set; }
        public Phonetic[] phonetic { get; set; }
    }

    public class Datum
    {
        public string dst { get; set; }
        public int prefixWrap { get; set; }
        public object[][] result { get; set; }
        public string src { get; set; }
    }

    public class Phonetic
    {
        public string src_str { get; set; }
        public string trg_str { get; set; }
    }

    public class BaiDuInFo
    {
        public RequestCookies Cookies;
        public string Token = "";
        public string GTK = "";
        public string Sign = "";
        public string Query = "";

        public BaiDuInFo(RequestCookies Cookies, string Token, string GTK, string Sign, string Query)
        {
            this.Cookies = Cookies;
            this.Token = Token;
            this.GTK = GTK;
            this.Sign = Sign;
            this.Query = Query;
        }

    }



    public class DetectMsg
    {
        public int error { get; set; }
        public string msg { get; set; }
        public string lan { get; set; }
    }






}
