using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Tr8n
{
    public class config
    {
        #region Member Variables
        private string m_filename = null;
        private Dictionary<string, object> m_items = null;

        #endregion

        #region Properties
        public string filename
        {
            get { return m_filename== null ? "" : m_filename; }
        }

        public Dictionary<string, object> items
        {
            get
            {
                if (m_items == null)
                    m_items = new Dictionary<string, object>();
                return m_items;
            }
        }

        #endregion

        #region Methods
        public config()
        {
        }

        public config(string configFilePath)
        {
            Load(configFilePath);
        }

        /// <summary>
        /// Loads the current config file or one that you send in and makes it the current config
        /// </summary>
        /// <param name="configFilePath">The full path to the config file to load</param>
        /// <returns>true is the config is loaded successfully, else false</returns>
        public bool Load(string configFilePath = null)
        {
            if (configFilePath != null)
                m_filename = configFilePath;

            StreamReader sr = null;
            int maxIndentDepth = 100;
            string[] prefixes = new string[maxIndentDepth];
            try
            {
                sr = new StreamReader(m_filename);
                while (true)
                {
                    string line = sr.ReadLine();
                    if (line == null)
                        break;
                    if (line.Length < 1 || line.StartsWith("#") || line=="---")
                        continue;
                    int indent = CountWhiteSpace(line);
                    if (indent >= maxIndentDepth)
                        continue;
                    // remove comments
                    int pos = line.IndexOf('#');
                    if (pos > 0)
                        line = line.Substring(0, pos).Trim();
                    pos = line.IndexOf(':');
                    if (pos < 0)
                        continue;
                    prefixes[indent] = line.Substring(0,pos+1).Trim();
                    line = line.Substring(pos + 1).Trim();
                    if (line.Length > 0)
                    {
                        try
                        {
                            string keyToUse=string.Join("", prefixes, 0, indent+1);
                            // remove the last colon on the key
                            items.Add(keyToUse.Substring(0,keyToUse.Length-1), line);
                        }
                        catch { }
                    }

                }
            }
            catch
            {
                return false;
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }

            return true;
        }

        private int CountWhiteSpace(string line)
        {
            if (string.IsNullOrEmpty(line))
                return 0;
            int count = 0;
            foreach (char c in line)
            {
                if (!Char.IsWhiteSpace(c))
                    break;
                count++;
            }
            return count;
        }


        /// <summary>
        /// Returns the config item for the given key.
        /// </summary>
        /// <param name="key">Key to lookup separated by colons for the different sublevels (i.e. defaults:enable_tr8n)</param>
        /// <returns>Returns the key value or the default value if the key doesn't exist</returns>
        public string GetString(string key, string defaultVal = "")
        {
            object data;
            if (items.TryGetValue(key, out data))
                return (string)data;
            return defaultVal;
        }

        /// <summary>
        /// Returns the config item for the given key.
        /// </summary>
        /// <param name="key">Key to lookup separated by colons for the different sublevels (i.e. defaults:enable_tr8n)</param>
        /// <returns>Returns the key value or the default value if the key doesn't exist</returns>
        public bool GetBool(string key, bool defaultVal = false)
        {
            object data;
            if (items.TryGetValue(key, out data))
            {
                string test = (string)data;
                if (test == "1" || test == "true" || test == "yes")
                    return true;
                return false;
            }
            return defaultVal;
        }

        /// <summary>
        /// Returns the config item for the given key.
        /// </summary>
        /// <param name="key">Key to lookup separated by colons for the different sublevels (i.e. defaults:enable_tr8n)</param>
        /// <returns>Returns the key value or the default value if the key doesn't exist</returns>
        public int GetInt(string key, int defaultVal = 0)
        {
            object data;
            if (items.TryGetValue(key, out data))
            {
                try
                {
                    return Int32.Parse((string)data);
                }
                catch { }
            }
            return defaultVal;
        }


        #endregion
    }
}
