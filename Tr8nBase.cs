using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.IO;


namespace Tr8n
{
    public class Tr8nBase
    {
        #region Constants
        public string API_VERSION_PATH = "/tr8n/api/v1/";
        #endregion

        
        #region Member Variables
        private static MD5 m_md5HashGenerator = MD5.Create(); 
        #endregion

        #region Properties
        #endregion

        #region Methods

        /// <summary>
        /// Builds an md5 hash of the input string and returns it in lowercase
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MD5Hash(string input)
        {
            // Convert the input string to a byte array and compute the hash. 
            byte[] data = m_md5HashGenerator.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));

            return sBuilder.ToString();
        }

        /// <summary>
        /// Returns the url data in a string
        /// </summary>
        /// <param name="sURL">URL to load</param>
        /// <param name="timeoutInMilliseconds">Leave this at zero for no timeout</param>
        /// <returns>The text returned by the requested url, or empty string on error</returns>
        public static string GetHttpPage(string sURL,int timeoutInMilliseconds=0)
        {
            string sData = "";
            try
            {
                // create the web request for the given url
                HttpWebRequest objRequest = (HttpWebRequest)System.Net.HttpWebRequest.Create(sURL);
                objRequest.Timeout = timeoutInMilliseconds;
                // set a default user agent
                objRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
                // send the request and get the response
                HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                // put the response in a string
                Encoding enc;
                try
                {
                    enc = Encoding.GetEncoding(objResponse.CharacterSet);
                }
                catch
                {
                    enc = Encoding.UTF8;
                }
                StreamReader sr = new StreamReader(objResponse.GetResponseStream(), enc);
                sData = sr.ReadToEnd();
                sr.Close();
            }
            // ignore exceptions
            catch { }
            return sData;
        }

        /// <summary>
        /// Returns the url data in a string
        /// </summary>
        /// <param name="sURL">URL to load</param>
        /// <returns>The text returned by the requested url</returns>
        protected static string GetHttpPagePost(string sURL, string paramData)
        {
            string sData = "";
            try
            {
                // create the web request for the given url
                HttpWebRequest objRequest = (HttpWebRequest)System.Net.HttpWebRequest.Create(sURL);
                objRequest.Method = "POST";
                objRequest.ContentType = "application/x-www-form-urlencoded";
                byte[] data = Encoding.UTF8.GetBytes(paramData);
                objRequest.ContentLength = data.Length;
                Stream writeStream = objRequest.GetRequestStream();
                writeStream.Write(data, 0, data.Length);
                writeStream.Close();
                // send the request and get the response
                HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                // put the response in a string
                StreamReader sr = new StreamReader(objResponse.GetResponseStream(), true);
                sData = sr.ReadToEnd();
                sr.Close();
            }
            // ignore exceptions
            catch {}
            return sData;
        }

        /// <summary>
        /// Returns the corresponding parameter as an object
        /// </summary>
        /// <param name="paramKey"></param>
        /// <param name="items"></param>
        /// <returns>Returns null if the param doesn't exist</returns>
        public object GetParam(string paramKey, object[] items)
        {
            if (string.IsNullOrEmpty(paramKey) || items==null || items.Length<1)
                return null;

            // look for the key
            for (int t = 0; t < items.Length; t++)
            {
                if (items[t] is string && (string)items[t] == paramKey)
                {
                    // found the token -- now grab the next item as the object if we aren't on the last item
                    if (t < items.Length - 1)
                        return items[t + 1];
                }
            }
            return null;
        }




        #endregion

    }
}
