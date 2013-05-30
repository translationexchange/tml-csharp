using System;
using System.Collections.Generic;
using System.Text;


namespace Tr8n
{
    public class translationKey : Tr8nBase
    {
        #region Member Variables
        private string m_label=null;
        private string m_context = null;
        private string m_hashKey = null;
        #endregion

        #region Properties
        public string label
        {
            get { return m_label==null ? "" : m_label; }
        }

        public string context
        {
            get { return m_context == null ? "" : m_context; }
        }

        public string hashKey
        {
            get
            {
                if (m_hashKey != null)
                    return m_hashKey;
                m_hashKey = MD5Hash(label + ";;;" + context);
                return m_hashKey;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="label">The label or text to be translated (a tml string)</param>
        /// <param name="items"></param>
        public translationKey(string label, string context=null)
        {
            m_label = label;
            m_context = context;
        }

        /// <summary>
        /// Translate a single key
        /// </summary>
        /// <param name="items">tokens and options for the translation -- all options begin with a # (i.e. #descripton, #lang, etc)</param>
        /// <returns>The translated string</returns>
        public string translate(params object[] items)
        {
            // parse tokens

            // get all translations for the hash key

            // take first valid translation

            // substitute data

            // return decorated string
            return label;
        }
        #endregion
    }
}
