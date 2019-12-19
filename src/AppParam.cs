using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace VideoMonitor
{
    public class AppParam
    {
        public static int _numbers=0;
        public static List<string> _vstr=new List<string>();

        //只读属性
        public static int Numbers
        {
            get
            {
                return _numbers;
            }
        }

        public static List<string> Vstr
        {
            get
            {
                return _vstr;
            }
        }

       // public static IConfigurationRoot Configuration { get; set; }

        public static void Init()
        {
            try
            {
                JsonConfigHelper jscon = new JsonConfigHelper("app.json");

                int.TryParse(jscon["Numbers"],out int tmpnum);
                _numbers = tmpnum;
                if (_numbers > 1)
                {
                    for (int i = 0; i < _numbers; i++)
                    {
                        int index = i + 1;
                        _vstr.Add(jscon["ProcessName"+ index]); //GetConfigVauleRaw("ProcessName" + index, "")
                    }
                }
                else
                {
                    _vstr.Add(jscon["ProcessName" + _numbers]); //GetConfigVauleRaw("ProcessName" + _numbers, "")
                }

               
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

    }

    public class JsonConfigHelper
    {
        private JObject jObject = null;
        public string this[string key]
        {
            get
            {
                string str = "";
                if (jObject != null)
                {
                    str = GetValue(key);
                }
                return str;
            }
        }
        public JsonConfigHelper(string path)
        {
            jObject = new JObject();
            using (System.IO.StreamReader file = System.IO.File.OpenText(path))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    jObject = JObject.Load(reader);
                }
            };
        }
        public T GetValue<T>(string key) where T : class
        {
            return JsonConvert.DeserializeObject<T>(jObject.SelectToken(key).ToString());
        }
        public string GetValue(string key)
        {
            return Regex.Replace((jObject.SelectToken(key).ToString()), @"\s", "");
        }
    }
}
