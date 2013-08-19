using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Tr8n
{
    public class ParamsDictionary
    {
        public Dictionary<string, object> dict = new Dictionary<string, object>();

        public ParamsDictionary()
        {
        }

        public ParamsDictionary(params object[] items)
        {
            if (items != null || items.Length > 0)
            {
                for (int t = 0; t < items.Length; t++)
                {
                    // looks for pairs of items
                    if (items.Length > t)
                    {
                        try
                        {
                            dict.Add((string)items[t], items[t + 1]);
                        }
                        catch
                        {
                            // ignore duplicates 
                        }
                    }
                }
            }
        }

        public void Add(string paramKey, object obj)
        {
            try
            {
                dict.Add(paramKey,obj);
            }
            catch { }

        }

        public string GetString(string paramKey, string defaultVal)
        {
            object data;

            if (dict.TryGetValue(paramKey, out data))
            {
                if (data == null)
                    return defaultVal;
                return (string)data;
            }
            return defaultVal;
        }

        public string GetString(string paramKey)
        {
            return GetString(paramKey, "");
        }

        public object GetObject(string paramKey)
        {
            object data;

            if (dict.TryGetValue(paramKey, out data))
                return data;
            return null;
        }


        /// <summary>
        /// Returns the config item for the given key.
        /// </summary>
        /// <param name="key">Key to lookup separated by colons for the different sublevels (i.e. defaults:enable_tr8n)</param>
        /// <returns>Returns the key value or the default value if the key doesn't exist</returns>
        public bool GetBool(string key, bool defaultVal = false)
        {
            string test = GetString(key, defaultVal ? "1" : "0");
            if (test == "1" || test == "true" || test == "yes")
                return true;
            return false;
        }

        /// <summary>
        /// Returns the config item for the given key.
        /// </summary>
        /// <param name="key">Key to lookup separated by colons for the different sublevels (i.e. defaults:enable_tr8n)</param>
        /// <returns>Returns the key value or the default value if the key doesn't exist</returns>
        public int GetInt(string key, int defaultVal = 0)
        {
            object data = GetObject(key);
            if (data!=null)
            {
                try
                {
                    return Int32.Parse((string)data);
                }
                catch { }
            }
            return defaultVal;
        }


    }

    public class Util
    {

        public static string httpBuildQuery(Dictionary<string, object> paramData)
        {
            if (paramData == null)
                return "";
            StringBuilder sbQuery = new StringBuilder();
            foreach (KeyValuePair<string, object> kvp in paramData)
            {
                if (sbQuery.Length > 0)
                    sbQuery.Append("&");
                sbQuery.AppendFormat("{0}={1}", kvp.Key, HttpUtility.UrlEncode((string)kvp.Value));
            }
            return sbQuery.ToString();
        }

        public static List<string> GetDataTokens(string label)
        {
            List<string> tokens=new List<string>();
            Regex r = new Regex(@"(\{[^_:][\w]*(:[\w]+)?(::[\w]+)?\})");
            MatchCollection matches=r.Matches(label);
            if (matches.Count>0)
            {
                foreach (Match m in matches)
                {
                    for (int t=1;t<m.Groups.Count;t++)
                        tokens.Add(m.Groups[t].Value);
                }
            }
            return tokens;
        }

        public static List<string> GetTransformTokens(string label)
        {
            List<string> tokens = new List<string>();
            Regex r = new Regex(@"(\{[^_:|][\w]*(:[\w]+)?(::[\w]+)?\s*\|\|?[^{^}]+\})");
            MatchCollection matches = r.Matches(label);
            if (matches.Count > 0)
            {
                foreach (Match m in matches)
                    for (int t = 1; t < m.Groups.Count; t++)
                        tokens.Add(m.Groups[t].Value);
            }
            return tokens;
        }

        public static int Nzn(string data,int defaultVal=0)
        {
            try
            {
                return Int32.Parse(data);
            }
            catch { }
            return defaultVal;
        }

    }

}
