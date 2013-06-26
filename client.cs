using System;
using System.Collections.Generic;
using System.Text;

namespace Tr8n
{
    public class Tr8nClient : Tr8nBase
    {
        #region Member Variables
        private static config m_config = null;
        private static string m_defaultLocale = null;
        #endregion

        #region Properties
        public static config config
        {
            get
            {
                if (m_config == null)
                    m_config = new config();
                return m_config;
            }
        }

        public static string defaultLocale
        {
            get
            {
                if (m_defaultLocale == null)
                    m_defaultLocale = Tr8nClient.config["current_locale", "en-US"];
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
        #endregion

    }
}
