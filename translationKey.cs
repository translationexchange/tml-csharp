using System;
using System.Collections.Generic;
using System.Text;

using Tr8n.tokens;
namespace Tr8n
{
    public class translationKey : Tr8nBase
    {
        #region Member Variables
        private string m_label=null;
        private string m_context = null;
        private string m_hashKey = null;
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
        public translationKey(string locale,string label, string context=null)
        {
            m_label = label;
            m_context = context;
            this.locale = locale;
        }

        /// <summary>
        /// Translate a single key
        /// </summary>
        /// <param name="items">tokens and options for the translation -- all options begin with a # (i.e. #descripton, #lang, etc)</param>
        /// <returns>The translated string</returns>
        public string translate(ParamsDictionary pd)
        {
            if (Tr8nClient.config.GetBool("disabled"))
            {
                // translations are turned off
                return substituteTokens(label,pd);
            }

            // parse tokens

            // get all translations for the hash key

            // take first valid translation
            translation trans=firstValidTranslation(label);

            // substitute tokens
            string translatedLabel=substituteTokens(trans.translatedLabel, pd);

            // return decorated string


            return translatedLabel;
        }

        public translation firstValidTranslation(string labelToTranslate)
        {
            translation trans = new translation(labelToTranslate);
            trans.translatedLabel = labelToTranslate;
            return trans;
        }

        public string substituteTokens(string translatedLabel,ParamsDictionary pd)
        {
            // substitute data tokens
            tokenList dataTokens = new tokenList("data", translatedLabel);
            foreach (dataToken tok in dataTokens.tokens)
            {
                string replaceVal = "";
                object data = pd.GetObject(tok.name);
                if (data != null)
                {
                    replaceVal = data.ToString();
                }
                translatedLabel = translatedLabel.Replace(tok.tokenText, replaceVal);
            }

            // substitute decoration tokens

            return translatedLabel;
        }
        #endregion

    }
}
