using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tr8n
{
    public class ParamsDictionary
    {
        public Dictionary<string, object> dict = new Dictionary<string, object>();

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

        public string GetString(string paramKey)
        {
            object data;

            if (dict.TryGetValue(paramKey, out data))
            {
                if (data == null)
                    return "";
                return (string)data;
            }
            return "";
        }

        public object GetObject(string paramKey)
        {
            object data;

            if (dict.TryGetValue(paramKey, out data))
                return data;
            return null;
        }


    }

    public class Shared
    {
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

    }
}
