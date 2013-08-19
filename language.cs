using System;
using System.Collections.Generic;
using System.Text;

namespace Tr8n
{
    public class language : Tr8nBase
    {
        #region Constants
        public const string labelContextToken="#description";
        #endregion

        #region Member Variables
        private string m_locale = null;
        private string m_englishName = null;
        private string m_nativeName = null;
        private bool m_rightToLeft = false;
        #endregion

        #region Properties
        public bool rightToLeft
        {
            get { return m_rightToLeft; }
            set { m_rightToLeft = value; }
        }

        public string englishName
        {
            get { return m_englishName == null ? "" : m_englishName; }
            set { m_englishName = value; }
        }

        public string nativeName
        {
            get { return m_nativeName == null ? "" : m_nativeName; }
            set { m_nativeName = value; }
        }

        public string locale
        {
            get
            {
                if (string.IsNullOrEmpty(m_locale))
                    m_locale = application.defaultLocale;
                return m_locale; 
            }
            set { m_locale = value; }
        }

        public string flagUrl
        {
            get
            {
                return string.Format("http://{0}/assets/tr8n/flags/{1}.png",application.host,locale);
            }
        }

        public bool isDefault
        {
            get
            {
                if (application.app == null)
                    return true;
                return locale == application.defaultLocale;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Uses the default locale
        /// </summary>
        public language()
        {
        }

        /// <summary>
        /// Send in a locale
        /// </summary>
        /// <param name="langCode"></param>
        public language(string localeCode)
        {
            if (!string.IsNullOrWhiteSpace(localeCode))
                locale = localeCode;
        }

        public string translate(string label, ParamsDictionary pd)
        {
            string context = pd.GetString(labelContextToken);
            locale = pd.GetString("locale");

            translationKey tempKey = new translationKey(locale, label, context);
            translationKey realKey;
            // is the key in the cache?
            if (!application.translationKeyCache.TryGetValue(tempKey.hashKey, out realKey))
            {
                if (!tempKey.LoadKey())
                {
                    // key isn't even registered, so register it now
                    application.app.registerMissingKey(tempKey, pd.GetString("source"));
                    application.translationKeyCache.TryAdd(tempKey.hashKey, tempKey);
                }
                realKey = tempKey;
            }

            return realKey.translate(locale, pd);
        }

        public string translate(string label, params object[] items)
        {
            ParamsDictionary pd = new ParamsDictionary(items);
            return translate(label, pd);
        }
        #endregion

    }
}
