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
        private string m_mode = "default";

        #endregion

        #region Properties
        public string this[string key,string defaultVal=""]
        {
            get
            {
                return GetString(key,defaultVal);
            }
        }

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

        /// <summary>
        /// The mode if the override settings to use in the config file.  By default, it is set to "default" which will use all the default settings
        /// If the mode is set to something else (i.e. "dev") is will use that section of the config and fall back to default settings when dev settings don't exist.
        /// </summary>
        public string mode
        {
            get { return m_mode; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    m_mode = "default";
                else
                    m_mode = value;
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

        public void Reload()
        {
            Load(null);
        }

        /// <summary>
        /// Loads the current config file or one that you send in and makes it the current config
        /// </summary>
        /// <param name="configFilePath">The full path to the config file to load</param>
        /// <returns>true is the config is loaded successfully, else false</returns>
        public bool Load(string configFilePath = null)
        {
            items.Clear();
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
                if (c == '\t')
                    count += 4;
                else
                    count++;
            }
            return count;
        }


        private bool GetDataObject(string key, out object data)
        {
            if (items.TryGetValue(mode + ":" + key, out data))
                return true;
            if (mode != "default")
            {
                if (items.TryGetValue("default:" + key, out data))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the config item for the given key.
        /// </summary>
        /// <param name="key">Key to lookup separated by colons for the different sublevels (i.e. defaults:enable_tr8n)</param>
        /// <returns>Returns the key value or the default value if the key doesn't exist</returns>
        public string GetString(string key, string defaultVal = "")
        {
            object data;
            if (GetDataObject(key,out data))
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
            if (GetDataObject(key, out data))
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
            if (GetDataObject(key, out data))
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
