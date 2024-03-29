﻿using System;
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
        public const string API_VERSION_PATH = "/tr8n/api/v1/";
        #endregion

        
        #region Member Variables
        private static MD5 m_md5HashGenerator = MD5.Create();
        private static UptimeMonitor m_tr8nMonitor=null;
        #endregion

        #region Properties
        public static UptimeMonitor tr8nMonitor
        {
            get
            {
                if (m_tr8nMonitor == null)
                {
                    UptimeMonitor u = new UptimeMonitor(application.config.GetInt("remote:failure_count", 3), application.config.GetInt("remote:failure_window_in_seconds", 60), application.config.GetInt("remote:failure_reattempt_after_seconds", 120));
                    m_tr8nMonitor = u;
                }
                return m_tr8nMonitor;
            }
        }

        #endregion

        #region Methods

        public Tr8nBase(object[] attributes = null)
        {
            // attributes come in pairs of 2
            if (attributes != null && attributes.Length>1)
            {
                for(int t=0;t<attributes.Length;t+=2)
                {
                    this.GetType().GetProperty((string)attributes[t]).SetValue(this, attributes[t + 1], null);
                }
            }
        }

        /// <summary>
        /// Calls the TR8N api with the given path and data params
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dataParams"></param>
        /// <returns>A json object with the results</returns>
        public static json api(string path, string dataParams)
        {
            System.Diagnostics.EventLog.WriteEntry("Application", "API:" + path + "?" + dataParams);
            if (!tr8nMonitor.shouldAttempt)
                return new json("");
            string url = string.Format("http://{0}/tr8n/api/{1}?client_id={2}", application.config["remote:host"], path,application.config["remote:client_id"]);
            HttpStatusCode statusCode;
            string data = GetHttpPagePost(url, dataParams, out statusCode, application.config.GetInt("remote:host_timeout_ms", 5000));
            if (string.IsNullOrEmpty(data))
                tr8nMonitor.BadResponse();
            return new json(data);
        }

        /// <summary>
        /// Calls the TR8N api with the given path and data params
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dataParams"></param>
        /// <returns>A json object with the results</returns>
        public static json apiGet(string path, string dataParams,int timeoutInMS=0)
        {
            return new json(apiGetString(path,dataParams,timeoutInMS));
        }

        /// <summary>
        /// Calls the TR8N api with the given path and data params
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dataParams"></param>
        /// <returns>A json object with the results</returns>
        public static string apiGetString(string path, string dataParams,int timeoutInMS=0)
        {
            System.Diagnostics.EventLog.WriteEntry("Application", "APIGet:" + path + "?" + dataParams);
            if (!tr8nMonitor.shouldAttempt)
                return "";
            string url = string.Format("http://{0}/tr8n/api/{1}?client_id={2}", application.config["remote:host"], path, application.config["remote:client_id"]);
            HttpStatusCode statusCode;
            int timeout;
            if (timeoutInMS==0)
                timeout=application.config.GetInt("remote:host_timeout_ms", 5000);
            else
                timeout=timeoutInMS;
            string data = GetHttpPage(url + "&" + dataParams, out statusCode, timeout);
            if (string.IsNullOrEmpty(data))
                tr8nMonitor.BadResponse();
            return data;
        }

        /*
        public static object executeRequest(string path, Dictionary<string,object> paramData, Dictionary<string, object> options=null)
        {
            if (paramData == null)
                paramData=new Dictionary<string,object>();
            if (options==null)
                options=new Dictionary<string,object>();
            string url = string.Format("{0}{1}{2}", options["host"], API_VERSION_PATH, path);
            string postFields=Util.httpBuildQuery(paramData);
            string resultData;
            HttpStatusCode statusCode;

            if (options["method"] == "POST")
            {
                resultData = GetHttpPagePost(url, postFields,out statusCode);
            }
            else
            {
                url += "?" + postFields;
                resultData = GetHttpPage(url,out statusCode);
            }
            if (statusCode != HttpStatusCode.OK)
                throw new Tr8nException("Got HTTP response: "+statusCode.ToString());
            json data=new json(resultData);
            if (data == null || data.GetField("error").Length > 0)
                throw new Tr8nException("Data Error: " + data.GetField("error"));
            return processResponse(data,options);
        }
         */

        /*
        public static object processResponse(json data, Dictionary<string, object> options)
        {
            if (data.GetField("results").Length > 0)
            {
                object 
            }
            if (data.GetField("class").Length == 0)
                return data;

        }
        */

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
        public static string GetHttpPage(string sURL,out HttpStatusCode statusCode,int timeoutInMilliseconds=0)
        {
            string sData = "";
            statusCode = HttpStatusCode.OK;
            try
            {
                // create the web request for the given url
                HttpWebRequest objRequest = (HttpWebRequest)System.Net.HttpWebRequest.Create(sURL);
                objRequest.Timeout = timeoutInMilliseconds;
                // set a default user agent
                objRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
                // send the request and get the response
                HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                statusCode = objResponse.StatusCode;
                // put the response in a string
                Encoding enc=Encoding.UTF8;
                //try
                //{
                //    enc = Encoding.GetEncoding(objResponse.CharacterSet);
                //}
                //catch
                //{
                //    enc = Encoding.UTF8;
                //}
                StreamReader sr = new StreamReader(objResponse.GetResponseStream(), enc);
                
                sData = sr.ReadToEnd();
                sr.Close();
            }
            // ignore exceptions
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("Application", "GetHttpPageException:" +sURL+":"+ ex.Message);
            }

            return sData;
        }

        /// <summary>
        /// Returns the url data in a string
        /// </summary>
        /// <param name="sURL">URL to load</param>
        /// <returns>The text returned by the requested url</returns>
        protected static string GetHttpPagePost(string sURL, string paramData, out HttpStatusCode statusCode, int timeoutInMilliseconds = 0)
        {
            string sData = "";
            statusCode = HttpStatusCode.OK;
            try
            {
                // create the web request for the given url
                HttpWebRequest objRequest = (HttpWebRequest)System.Net.HttpWebRequest.Create(sURL);
                objRequest.Timeout = timeoutInMilliseconds;
                objRequest.Method = "POST";
                objRequest.ContentType = "application/x-www-form-urlencoded";
                byte[] data = Encoding.UTF8.GetBytes(paramData);
                objRequest.ContentLength = data.Length;
                Stream writeStream = objRequest.GetRequestStream();
                writeStream.Write(data, 0, data.Length);
                writeStream.Close();
                // send the request and get the response
                HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
                statusCode = objResponse.StatusCode;
                // put the response in a string
                StreamReader sr = new StreamReader(objResponse.GetResponseStream(), true);
                sData = sr.ReadToEnd();
                sr.Close();
            }
            // ignore exceptions
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("Application", "GetHttpPagePostException:" +sURL+":"+paramData+":"+ ex.Message);
            }
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
