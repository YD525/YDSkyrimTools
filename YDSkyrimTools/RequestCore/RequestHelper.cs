using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace YDSkyrimTools.RequestCore
{
    public class RequestHelper
    {
        public static string UA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36";//游览器伪造头

        public static RequestMsg CallRequest(RequestAddress NewRequest)
        {
            RequestMsg CurrentMsg = null;
            switch (NewRequest.Type)
            {
                case RequestType.HttpGet:
                    CurrentMsg = RequestHelper.HttpGet(NewRequest.Address, NewRequest.Accept, NewRequest.ContentType, NewRequest.Param, NewRequest.Headers, NewRequest.Cookies, NewRequest.TimeOut,NewRequest.Referer);
                    break;
                case RequestType.HttpPost:
                    CurrentMsg = RequestHelper.HttpPost(NewRequest.Address, NewRequest.Accept, NewRequest.ContentType, NewRequest.Param, NewRequest.Headers, NewRequest.Cookies, NewRequest.IsPayLoad, NewRequest.TimeOut,NewRequest.Referer);
                    break;
            }
            return CurrentMsg;
        }


        private static HttpClient BearerClient = new HttpClient();

        public async static Task<string> CreateUserPost(Uri Url, byte[] Content, string Token)
        {
            BearerClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            BearerClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

            var Req = new HttpRequestMessage(HttpMethod.Post, Url);

            Req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.Split(' ')[1]);

            Req.Content = new ByteArrayContent(
                Content
            );

            var Response = await BearerClient.SendAsync(Req);

            string GetHtmlCode = await Response.Content.ReadAsStringAsync();

            return GetHtmlCode;
        }


     

        public static HttpItem GetRequest(RequestAddress NewRequest)
        {
            HttpItem CurrentMsg = new HttpItem();
            switch (NewRequest.Type)
            {
                case RequestType.HttpGet:
                    CurrentMsg = RequestHelper.GetHttpGet(NewRequest.Address, NewRequest.Accept, NewRequest.ContentType, NewRequest.Param, NewRequest.Headers, NewRequest.Cookies, NewRequest.TimeOut);
                    break;
                case RequestType.HttpPost:
                    CurrentMsg = RequestHelper.GetHttpPost(NewRequest.Address, NewRequest.Accept, NewRequest.ContentType, NewRequest.Param, NewRequest.Headers, NewRequest.Cookies, NewRequest.IsPayLoad, NewRequest.TimeOut);
                    break;
            }
            return CurrentMsg;
        }

        public static string GetHtml(HttpItem Item)
        {
            HttpHelper NewHttpHelper = new HttpHelper();
            var Get = NewHttpHelper.GetHtml(Item);
            return Get.Html;
        }
        public static RequestMsg HttpPost(string Url, string Accept, string ContentType, ParamItem ParmaList, WebHeaderCollection Headers, RequestCookies Cookies, bool IsPayLoad, int MsTimeOut,string Referer)
        {
            string GetFormat = "";
            if (IsPayLoad)
            {
                foreach (var GetItem in ParmaList.AllParam)
                {
                    GetFormat += GetItem.Name + "=" + GetItem.Value + "&";
                }
                if (GetFormat.EndsWith("&")) GetFormat = GetFormat.Substring(0, GetFormat.Length - 1);
            }
            else
            {
                int Index = 0;
                foreach (var GetItem in ParmaList.AllParam)
                {
                    if (Index == 0)
                    {
                        GetFormat += "?" + GetItem.Name + "=" + GetItem.Value;
                    }
                    else
                    {
                        GetFormat += "&" + GetItem.Name + "=" + GetItem.Value;
                    }
                    Index++;
                }
                Url = Url + GetFormat;
            }


            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = Url,
                UserAgent = UA,
                Method = "post",
                Header = Headers,
                Accept = Accept,
                Postdata = GetFormat,
                Cookie = Cookies.Cookies,
                ContentType = ContentType,
                Timeout = MsTimeOut,
                Referer = Referer
            };
            try
            {
                item.Header.Add("Accept-Encoding", " gzip");
            }
            catch { }
            HttpResult result = http.GetHtml(item);
            return new RequestMsg(result.ResponseUri, result.Html, result.Cookie, result.Header);
        }

        public static RequestMsg HttpGet(string Url, string Accept, string ContentType, ParamItem ParmaList, WebHeaderCollection Headers, RequestCookies Cookies, int MsTimeOut,string Referer)
        {
            string GetFormat = "";
            int Index = 0;
            foreach (var GetItem in ParmaList.AllParam)
            {
                if (Index == 0)
                {
                    GetFormat += "?" + GetItem.Name + "=" + GetItem.Value;
                }
                else
                {
                    GetFormat += "&" + GetItem.Name + "=" + GetItem.Value;
                }
                Index++;
            }

            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = Url + GetFormat,
                UserAgent = UA,
                Method = "get",
                Header = Headers,
                Accept = Accept,
                Cookie = Cookies.Cookies,
                ContentType = ContentType,
                Timeout = MsTimeOut,
                KeepAlive=true,
                Referer = Referer
            };
            item.Header.Add("Accept-Encoding", " gzip");
            HttpResult result = http.GetHtml(item);
            return new RequestMsg(result.ResponseUri, result.Html, result.Cookie, result.Header);
        }


        public static HttpItem GetHttpPost(string Url, string Accept, string ContentType, ParamItem ParmaList, WebHeaderCollection Headers, RequestCookies Cookies, bool IsPayLoad, int MsTimeOut)
        {
            string GetFormat = "";
            if (IsPayLoad)
            {
                foreach (var GetItem in ParmaList.AllParam)
                {
                    GetFormat += GetItem.Name + "=" + GetItem.Value + "&";
                }
                if (GetFormat.EndsWith("&")) GetFormat = GetFormat.Substring(0, GetFormat.Length - 1);
            }
            else
            {
                int Index = 0;
                foreach (var GetItem in ParmaList.AllParam)
                {
                    if (Index == 0)
                    {
                        GetFormat += "?" + GetItem.Name + "=" + GetItem.Value;
                    }
                    else
                    {
                        GetFormat += "&" + GetItem.Name + "=" + GetItem.Value;
                    }
                    Index++;
                }
                Url = Url + GetFormat;
            }


            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = Url,
                UserAgent = UA,
                Method = "post",
                Header = Headers,
                Accept = Accept,
                Postdata = GetFormat,
                Cookie = Cookies.Cookies,
                ContentType = ContentType,
                Timeout = MsTimeOut
            };
            try
            {
                item.Header.Add("Accept-Encoding", " gzip");
            }
            catch
            {
            }

            return item;
        }

        public static HttpItem GetHttpGet(string Url, string Accept, string ContentType, ParamItem ParmaList, WebHeaderCollection Headers, RequestCookies Cookies, int MsTimeOut)
        {
            string GetFormat = "";
            int Index = 0;
            foreach (var GetItem in ParmaList.AllParam)
            {
                if (Index == 0)
                {
                    GetFormat += "?" + GetItem.Name + "=" + GetItem.Value;
                }
                else
                {
                    GetFormat += "&" + GetItem.Name + "=" + GetItem.Value;
                }
                Index++;
            }

            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = Url + GetFormat,
                UserAgent = UA,
                Method = "get",
                Header = Headers,
                Accept = Accept,
                Cookie = Cookies.Cookies,
                ContentType = ContentType,
                Timeout = MsTimeOut
            };
            try
            {
                item.Header.Add("Accept-Encoding", " gzip");
            }
            catch { }
            return item;
        }

    }

    public class RequestCookies
    {
        public string Cookies = "";

        public RequestCookies(string Cookies)
        {
            this.Cookies = Cookies;
        }
        public RequestCookies()
        {
        }
        public string GetCookies(string Name)
        {
            //this.Cookies = this.Cookies.Replace(",", ";");

            if (this.Cookies.Contains(";"))
            {
                foreach (var Get in this.Cookies.Split(';'))
                {
                    if (Get.Contains("="))
                    {
                        if (Name.ToLower().Equals((Get.Split('=')[0]).ToLower()))
                        {
                            return Get.Split('=')[1];
                        }
                    }
                }
            }
            else
            {
                if (this.Cookies.Contains("="))
                {
                    if (Name.ToLower().Equals((this.Cookies.Split('=')[0]).ToLower()))
                    {
                        return this.Cookies.Split('=')[1];
                    }
                }
            }
            return null;
        }

        public bool DeleteCookies(string Name)
        {
            List<Param> AllParma = new List<Param>();
            if (this.Cookies.Contains(";"))
            {
                foreach (var Get in this.Cookies.Split(';'))
                {
                    if (Get.Contains("="))
                    {
                        string GetName = ((Get.Split('=')[0]).ToLower()).Trim();
                        if (!Name.ToLower().Equals(GetName))
                        {
                            AllParma.Add(new Param(Get.Split('=')[0], Get.Split('=')[1]));
                        }
                    }
                }
            }
            else
            {
                if (this.Cookies.Contains("="))
                {
                    if (!Name.ToLower().Equals(((this.Cookies.Split('=')[0]).ToLower()).Trim()))
                    {
                        AllParma.Add(new Param(this.Cookies.Split('=')[0], this.Cookies.Split('=')[1]));
                    }
                }
            }

            this.Cookies = string.Empty;
            foreach (var Get in AllParma)
            {
                this.Cookies += Get.Name + "=" + Get.Value + ";";
            }
            if (this.Cookies.EndsWith(";"))
            {
                this.Cookies = this.Cookies.Substring(0, this.Cookies.Length - 1);
            }
            if (AllParma.Count > 0) return true;
            return false;
        }

        public bool AddCookies(string Name, string Value)
        {
            //this.Cookies = this.Cookies.Replace(",", ";");

            List<Param> AllParma = new List<Param>();
            
            if (this.Cookies.Contains(";"))
            {
                foreach (var Get in this.Cookies.Split(';'))
                {
                    if (Get.Contains("="))
                    {
                        AllParma.Add(new Param(Get.Split('=')[0], Get.Split('=')[1]));
                    }
                }
            }
            else
            {
                if (this.Cookies.Contains("="))
                {
                    AllParma.Add(new Param(this.Cookies.Split('=')[0], this.Cookies.Split('=')[1]));
                }
            }
            bool IsFristItem = true;
            for (int i = 0; i < AllParma.Count; i++)
            {
                if (AllParma[i].Name.ToLower().Equals(Name.ToLower()))
                {
                    AllParma[i].Value = Value;
                    IsFristItem = false;
                }
            }
            if (IsFristItem)
            {
                AllParma.Add(new Param(Name, Value));
            }

            this.Cookies = string.Empty;
            foreach (var Get in AllParma)
            {
                this.Cookies += Get.Name + "=" + Get.Value + ";";
            }
            if (this.Cookies.EndsWith(";"))
            {
                this.Cookies = this.Cookies.Substring(0, this.Cookies.Length - 1);
            }
            if (AllParma.Count > 0) return true;
            return false;
        }
    }

    public class Param
    {
        public string Name = "";
        public string Value = "";
        public Param(string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }


    }

    public class ParamItem
    {
        public List<Param> AllParam = new List<Param>();
        public ParamItem(string Param)
        {
            string CurrentParma = Param;
            if (CurrentParma.StartsWith("?"))
            {
                CurrentParma = CurrentParma.Substring(1, CurrentParma.Length - 1);
            }
            foreach (var Get in CurrentParma.Split('&'))
            {
                if (Get.Contains("=")) AllParam.Add(new Param(Get.Split('=')[0], Get.Split('=')[1]));
            }
        }
        public ParamItem()
        {

        }
        public bool SetItem(string Name, string Value)
        {
            for (int i = 0; i < this.AllParam.Count; i++)
            {
                if (this.AllParam[i].Name.ToLower().Equals(Name.ToLower()))
                {
                    this.AllParam[i].Value = Value;
                    return true;
                }
            }
            return false;
        }
    }

    public class RequestMsg
    {
        public Uri WebUrl = null;
        public WebHeaderCollection ReturnHeaders = new WebHeaderCollection();
        public string HtmlCode = "";
        public RequestCookies Cookies = new RequestCookies();


        public RequestMsg()
        {

        }
        public RequestMsg(string WebUrl, string HtmlCode, string Cookies, WebHeaderCollection Headers)
        {
            try
            {
                this.WebUrl = new Uri(WebUrl);
                this.HtmlCode = HtmlCode;
                this.Cookies = new RequestCookies(Cookies);
                this.ReturnHeaders = Headers;
            }
            catch { }
        }
    }

    public enum RequestType
    {
        HttpGet = 0, HttpPost = 1
    }
    public class RequestAddress
    {
        public int TimeOut = 10000;
        public string Address = "";
        public string Accept = "";
        public string ContentType = "";
        public string Referer = "";
        public bool IsPayLoad = true;

        public RequestCookies Cookies = new RequestCookies();
        public WebHeaderCollection Headers = new WebHeaderCollection();

        public ParamItem Param = new ParamItem();

        public RequestType Type = new RequestType();

        public RequestAddress()
        {

        }
        public RequestAddress(string Address, string Accept, string ContentType, string Param, RequestType SelectType, string Cookies = null)
        {
            this.Accept = Accept;
            this.ContentType = ContentType;
            if (Cookies == null)
            {
                this.Cookies = new RequestCookies();
            }
            else
            {
                this.Cookies = new RequestCookies(Cookies);
            }

            this.Type = SelectType;

            this.Address = Address;
            this.Param = new ParamItem(Param);
        }
    }
}
