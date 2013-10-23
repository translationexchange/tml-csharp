using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using System.Collections.Concurrent;

namespace Tr8n
{
    public class application : Tr8nBase
    {
        #region Member Variables
        private static config m_config = null;
        private static string m_defaultLocale = null;
        private static application m_app = null;
        private static ConcurrentDictionary<string, translationKey> m_translationKeyCache = new ConcurrentDictionary<string, translationKey>();
        private static ConcurrentDictionary<string, translationKey> m_translationKeyCacheInline = new ConcurrentDictionary<string, translationKey>();
        private static string m_loadedLanguageFile = null;

        private ConcurrentDictionary<string,Dictionary<string,translationKey>> m_missingKeysBySource=new ConcurrentDictionary<string,Dictionary<string,translationKey>>();
        private string m_currentLocale = null;
        private language m_currentLanguage = null;
        private Tr8nCookie m_tr8nCookie = null;
        private ParamsDictionary m_globalTranslationOptions = new ParamsDictionary();

        #endregion

        #region Properties
        public static string host { get; set; }
        public static string clientId { get; set; }
        public static string clientSecret { get; set; }

        private static List<language> m_languages = null;
        private static Dictionary<string, language> m_languagesByLocale = null;
        private static List<source> sources { get; set; }


        public ParamsDictionary globalTranslationOptions
        {
            get { return m_globalTranslationOptions; }
        }

        public Tr8nCookie tr8nCookie
        {
            get
            {
                if (m_tr8nCookie == null)
                    m_tr8nCookie = new Tr8nCookie();
                return m_tr8nCookie;
            }
        }

        public static config config
        {
            get
            {
                if (m_config == null)
                    m_config = new config();
                return m_config;
            }
        }

        public language currentLanguage
        {
            get
            {
                if (m_currentLanguage == null)
                {
                    language test;
                    if (languagesByLocale.TryGetValue(currentLocale, out test))
                        m_currentLanguage = test;
                    else
                    {
                        if (languagesByLocale.TryGetValue(defaultLocale, out test))
                            m_currentLanguage = test;
                        else
                            m_currentLanguage = new language(defaultLocale);
                    }
                }
                return m_currentLanguage;
            }
        }

        public static ConcurrentDictionary<string, translationKey> translationKeyCache
        {
            get { return m_translationKeyCache; }
        }

        public static ConcurrentDictionary<string, translationKey> translationKeyCacheInline
        {
            get { return m_translationKeyCacheInline; }
        }

        public static application app
        {
            get { return m_app; }
            set { m_app = value; }
        }

        private static Dictionary<string, language> languagesByLocale
        {
            get
            {
                if (m_languagesByLocale == null)
                {
                    Dictionary<string, language> lanDict = new Dictionary<string, language>();
                    foreach (language lan in languages)
                    {
                        if (!lanDict.ContainsKey(lan.locale))
                            lanDict.Add(lan.locale.ToLower(), lan);
                    }
                    m_languagesByLocale = lanDict;
                }
                return m_languagesByLocale;
            }
        }

        public static List<language> languages
        {
            get
            {
                if (m_languages != null)
                    return m_languages;

                json j = apiGet("application/languages","");
                List<language> list = new List<language>();
                if (j != null)
                {

                    foreach (Dictionary<string, object> item in j.GetFieldList("results"))
                    {

                        json jTemp = new json(item);
                        language lan = new language(jTemp.GetField("locale").ToLower());
                        lan.englishName = jTemp.GetField("english_name");
                        lan.nativeName = jTemp.GetField("native_name");
                        lan.rightToLeft = jTemp.GetFieldBool("right_to_left", false);

                        list.Add(lan);
                    }

                }
                m_languages = list;
                return m_languages;
            }
        }

        public string currentLocale
        {
            get
            {
                if (m_currentLocale == null)
                    m_currentLocale = application.defaultLocale;
                return m_defaultLocale;
            }
            set { m_defaultLocale = value; }
        }


        public static bool isInitialized
        {
            get { return m_app == null ? false : true; }
        }

        public static string defaultLocale
        {
            get
            {
                if (m_defaultLocale == null)
                    m_defaultLocale = application.config["current_locale", "en-US"].ToLower();
                return m_defaultLocale;
            }
            set { m_defaultLocale = value; }
        }

        public static string defaultLanguage
        {
            get
            {
                if (defaultLocale.Length > 2)
                    return defaultLocale.Substring(0, 2);
                return defaultLocale;
            }
            set { m_defaultLocale = value; }
        }


        #endregion

        #region Methods

        public application()
        {
            m_app = this;
        }

        public static application init(string host,string clientId,string clientSecret,Dictionary<string,object> options)
        {
            application app = new application();
            application.host = host;
            application.clientId = clientId;
            application.clientSecret = clientSecret;
            application.app = app;
            return app;
        }

        /// <summary>
        /// Sets the config file you are using
        /// </summary>
        /// <param name="configFile">Full file path and name of the config file</param>
        /// <param name="mode">The mode of the config file (i.e. dev, staging, live, default) -- Uses default by default</param>
        public static void SetConfig(string configFile, string mode = null)
        {
            m_config = new config(configFile);
            m_config.mode = mode;
        }

        public static string translate(string label, params object[] items)
        {
            return new language().translate(label, items);
        }

        public static void LoadLanguageTranslationFile(string filename,bool forceReload=false)
        {
            if (filename == m_loadedLanguageFile && forceReload == false)
                return;
            System.Diagnostics.EventLog.WriteEntry("Application", "LoadLanguageFile:"+filename);

            translationKeyCache.Clear();
            m_loadedLanguageFile = filename;
            StreamReader sr = new StreamReader(filename);
            while (true)
            {
                string line = sr.ReadLine();
                if (line == null)
                    break;
                if (line.Length < 1)
                    continue;
                try
                {
                    json jSource = new json(line);
                    translationKey k = new translationKey("", jSource.GetField("label"), jSource.GetField("description"));
                    k.id = Util.Nzn(jSource.GetField("id"), 0);

                    Dictionary<string, List<translation>> tList = new Dictionary<string, List<translation>>();
                    // only do the languages we are supporting right now
                    foreach (language lan in application.languages)
                    {
                        // don't store any english translations
                        if (lan.locale.StartsWith("en"))
                            continue;
                        foreach (Dictionary<string, object> item in jSource.GetFieldList("translations:" + lan.localeProperCase))
                        {
                            json jTemp = new json(item);
                            translation trans = new translation(Util.Nzn(jTemp.GetField("id")), jTemp.GetField("label"), lan.locale, Util.Nzn(jTemp.GetField("rank")));
                            List<translation> items;
                            if (tList.TryGetValue(trans.locale, out items))
                            {
                                items.Add(trans);
                            }
                            else
                            {
                                items = new List<translation>();
                                items.Add(trans);
                                tList.Add(trans.locale, items);
                            }
                        }
                        k.translations = tList;
                    }
                    application.translationKeyCache.TryAdd(k.hashKey, k);
                }
                catch { }
            }
            sr.Close();
            System.Diagnostics.EventLog.WriteEntry("Application", "LanguageFileLoaded:"+application.translationKeyCache.Count+" items" );

        }

        public void registerMissingKey(translationKey transKey, string source = null)
        {
            if (string.IsNullOrEmpty(source))
                source = "DefaultSource";
            Dictionary<string, translationKey> keys;
            if (!m_missingKeysBySource.TryGetValue(source, out keys))
            {
                keys = new Dictionary<string, translationKey>();
                m_missingKeysBySource.TryAdd(source, keys);
            }
            try
            {
                keys.Add(transKey.hashKey, transKey);
            }
            catch { }
        }

        public void submitMissingKeys()
        {
            if (m_missingKeysBySource.Count < 1)
                return;

            List<sourceKeyInterface> list = new List<sourceKeyInterface>();
            foreach (KeyValuePair<string, Dictionary<string, translationKey>> kvp in m_missingKeysBySource)
            {
                if (kvp.Value.Count < 1)
                    continue;
                sourceKeyInterface ski = new sourceKeyInterface();
                ski.source = kvp.Key;
                foreach (KeyValuePair<string, translationKey> tkey in kvp.Value)
                    ski.keys.Add(tkey.Value.toInterface);
                list.Add(ski);
            }
            if (list.Count > 0)
            {
                string data = new JavaScriptSerializer().Serialize(list);
                json j = api("source/register_keys", string.Format("source_keys={0}", System.Web.HttpUtility.UrlEncode(data)));
            }

            m_missingKeysBySource.Clear();
        }

        public static bool localeIsSupported(string locale)
        {
            if (string.IsNullOrEmpty(locale))
                return false;
            if (languagesByLocale.ContainsKey(locale.ToLower()))
                return true;

            return false;
        }

        
        #endregion

    }
}
