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
        #endregion

        #region Properties
        public string locale
        {
            get
            {
                if (m_locale == null)
                    m_locale = Tr8nClient.defaultLocale;
                return m_locale; 
            }
            set { m_locale = value; }
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

        public string translate(string label, params object[] items)
        {
            ParamsDictionary pd = new ParamsDictionary(items);
            string context = pd.GetString(labelContextToken);

            return new translationKey(locale,label,context).translate(pd);
        }
        #endregion

    }
}
