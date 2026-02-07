using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonManager
{
    public class JsonCore
    {
        public static T JsonParse<T>(string Content) where T : new()
        {
            try {
            return JsonConvert.DeserializeObject<T>(Content);
            }
            catch { return new T(); }
        }

        public static string CreatJson(object Any)
        {
            return JsonConvert.SerializeObject(Any);
        }

        public static JObject GetObj(string Content)
        {
            return JObject.Parse(Content);
        }
    }
}
