using System;
using System.Collections.Generic;
using System.Web;
using System.Collections;

namespace Tr8n
{
    public class json
    {
        #region Member Variables
        private Dictionary<string, object> m_data = null;
        #endregion

        #region Properties
        public Dictionary<string, object> dataDictionary
        {
            get { return m_data; }
        }
        #endregion

        public json(string data)
        {
            try
            {
                m_data = (Dictionary<string, object>)fastJSON.JSON.Instance.Parse(data);
            }
            catch
            {
                m_data = new Dictionary<string, object>();
            }
        }

        public json(Dictionary<string, object> d)
        {
            m_data = d;
        }

        public string GetField(string key)
        {
            string returnVal = "";
            if (string.IsNullOrEmpty(key))
                return "";
            string[] parts = key.Trim().Split(':');
            if (parts.Length == 1)
                return InternalGetField(parts[0]);
            try
            {
                Dictionary<string, object> dict = InternalGetFieldDictionary(parts[0]);
                for (int t = 1; t < parts.Length - 1; t++)
                {
                    dict = (Dictionary<string, object>)dict[parts[t]];
                }
                returnVal = (string)dict[parts[parts.Length - 1]];
            }
            catch
            {
            }
            return EIN(returnVal);
        }

        public bool GetFieldBool(string key,bool defaultVal)
        {
            if (string.IsNullOrEmpty(key))
                return defaultVal;
            string[] parts = key.Trim().Split(':');
            if (parts.Length == 1)
                return InternalGetFieldBool(parts[0],defaultVal);
            try
            {
                Dictionary<string, object> dict = InternalGetFieldDictionary(parts[0]);
                for (int t = 1; t < parts.Length - 1; t++)
                {
                    dict = (Dictionary<string, object>)dict[parts[t]];
                }
                return (bool)dict[parts[parts.Length - 1]];
            }
            catch
            {
            }
            return defaultVal;
        }

        public string EIN(string data)
        {
            if (string.IsNullOrEmpty(data))
                return "";
            return data;
        }


        public string GetFieldDisplayDate(string key)
        {
            string date = GetField(key+":date");
            string text = GetField(key + ":text");
            if (date.Length == 4 || date.Length<1)
                return text;
            return date;
        }


        public Dictionary<string, object> GetFieldDictionary(string key)
        {
            Dictionary<string, object> returnVal = null;
            if (string.IsNullOrEmpty(key))
                return returnVal;
            string[] parts = key.Trim().Split(':');
            if (parts.Length == 1)
                return InternalGetFieldDictionary(parts[0]);
            try
            {
                Dictionary<string, object> dict = InternalGetFieldDictionary(parts[0]);
                for (int t = 1; t < parts.Length - 1; t++)
                {
                    dict = (Dictionary<string, object>)dict[parts[t]];
                }
                returnVal = (Dictionary<string, object>)dict[parts[parts.Length - 1]];
            }
            catch
            {
            }
            return returnVal;
        }

        public ArrayList GetFieldList(string key)
        {
            ArrayList returnVal = new ArrayList();
            if (string.IsNullOrEmpty(key))
                return returnVal;
            string[] parts = key.Trim().Split(':');
            if (parts.Length == 1)
                return InternalGetFieldList(parts[0]);
            try
            {
                Dictionary<string, object> dict = InternalGetFieldDictionary(parts[0]);
                for (int t = 1; t < parts.Length - 1; t++)
                {
                    dict = (Dictionary<string, object>)dict[parts[t]];
                }
                returnVal = (ArrayList)dict[parts[parts.Length - 1]];
            }
            catch
            {
            }
            return returnVal;
        }


        private string InternalGetField(string key)
        {
            string field = "";
            try
            {
                field = (string)m_data[key];
            }
            catch
            {
                field = "";
            }
            return field;
        }

        private bool InternalGetFieldBool(string key,bool defaultVal)
        {
            try
            {
                return (bool)m_data[key];
            }
            catch
            {
            }
            return defaultVal;
        }
        private Dictionary<string, object> InternalGetFieldDictionary(string key)
        {
            Dictionary<string, object> dict = null;
            try
            {
                dict = (Dictionary<string, object>)m_data[key];
            }
            catch
            {
                dict = new Dictionary<string, object>();
            }
            return dict;
        }

        private ArrayList InternalGetFieldList(string key)
        {
            ArrayList dict = null;
            try
            {
                dict = (ArrayList)m_data[key];
            }
            catch
            {
                dict = new ArrayList();
            }
            return dict;
        }

    }
}