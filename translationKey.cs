﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;

using Tr8n.tokens;
namespace Tr8n
{
    public class translationKey : Tr8nBase
    {
        #region Member Variables
        private int m_id = 0;
        private string m_label=null;
        private string m_context = null;
        private string m_hashKey = null;
        private string m_locale = null;
        private int m_level = 0;
        private bool m_locked = false;
        private bool m_isLoaded = false;

        private Dictionary<string, List<translation>> m_translations = null;

        #endregion

        #region Properties
        public bool isLocked
        {
            get { return m_locked; }
            set { m_locked = value; }
        }

        public int level
        {
            get { return m_level; }
            set { m_level = value; }
        }

        public int id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        public string description
        {
            get { return context; }
        }

        public translationKeyInterface toInterface
        {
            get
            {
                return new translationKeyInterface
                {
                    label = this.label,
                    description = this.context,
                    level = this.level,
                    locale = this.locale
                };
            }
        }

        public bool LoadKey()
        {
            int line = 0;
            try
            {
                m_isLoaded = true;
                if (m_translations == null)
                    m_translations = new Dictionary<string, List<translation>>();
                else
                    m_translations.Clear();
                json j = apiGet("translation_key", "key=" + hashKey);
                line = 1;
                if (j == null || j.GetField("error").Length > 0)
                    return false;
                line = 2;
                m_id = Util.Nzn(j.GetField("id"),0);
                Dictionary<string, List<translation>> tList = new Dictionary<string, List<translation>>();
                foreach (Dictionary<string, object> item in j.GetFieldList("translations"))
                {
                    line = 3;
                    json jTemp = new json(item);
                    line = 4;
                    translation trans = new translation(Util.Nzn(jTemp.GetField("id")), jTemp.GetField("label"), jTemp.GetField("locale"), Util.Nzn(jTemp.GetField("rank")));
                    line = 5;
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
                    line = 6;
                }
                m_translations = tList;
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("Application","Line "+line+": label="+label+": error: "+ ex.Message);
                m_isLoaded = false;
            }
            return m_isLoaded;

        }

        public Dictionary<string, List<translation>> translations
        {
            get
            {
                if (m_translations != null)
                    return m_translations;
                //if (!m_isLoaded)
                //    LoadKey();
                if (m_translations == null)
                    m_translations = new Dictionary<string, List<translation>>();
                return m_translations;
            }
            set { m_translations = value; }
        }

        public string locale
        {
            get
            {
                if (m_locale == null)
                    m_locale = application.defaultLocale;
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

        public bool hasTranslations(language lan)
        {
            return hasTranslations(lan.locale);
        }

        public bool hasTranslations(string locale)
        {
            List<translation> items;
            if (!translations.TryGetValue(locale, out items))
                return false;
            return items.Count > 0;
        }

        public List<translation> translationsByLocale(string locale)
        {
            List<translation> items;
            if (translations.TryGetValue(locale, out items))
                return items;
            return new List<translation>();
        }

        /// <summary>
        /// Translate a single key
        /// </summary>
        /// <param name="items">tokens and options for the translation -- all options begin with a # (i.e. #descripton, #lang, etc)</param>
        /// <returns>The translated string</returns>
        public string translate(string language,ParamsDictionary pd)
        {
            if (application.config.GetBool("disabled"))
            {
                // translations are turned off
                return decorate(substituteTokens(label,pd),pd);
            }

            // take first valid translation
            string translatedLabel;
            translation trans = firstValidTranslation(language, pd);

            if (trans != null)
                translatedLabel = trans.label;
            else
                translatedLabel = label;
            // substitute tokens
            translatedLabel=substituteTokens(translatedLabel, pd);

            // decorate the string
            translatedLabel = decorate(translatedLabel, pd);

            // add inline if necessary
            if (pd.GetBool("#inline", false))
                translatedLabel = addInlineSpan(translatedLabel);

            return translatedLabel;
        }

        protected string decorate(string translatedLabel,ParamsDictionary pd)
        {
            if (pd.GetBool("#skip_decorations", false))
                return translatedLabel;
            // substitute decoration tokens
            tokenList decTokens;
            string className;
            string styleName;
            string idName;
            string href;
            string attributes;

            int attributePos = 0;

            while (true)
            {
                decTokens= new tokenList("decoration", translatedLabel);
                if (decTokens.tokens.Count < 1)
                    break;
                foreach (decorationToken tok in decTokens.tokens)
                {
                    switch (tok.name)
                    {
                        case "bold":
                            translatedLabel = translatedLabel.Replace(tok.tokenText, string.Format("<strong>{0}</strong>", tok.tokenValue));
                            break;
                        case "italics":
                            translatedLabel = translatedLabel.Replace(tok.tokenText, string.Format("<em>{0}</em>", tok.tokenValue));
                            break;
                        case "quote":
                            translatedLabel = translatedLabel.Replace(tok.tokenText, string.Format("\"{0}\"", tok.tokenValue));
                            break;
                        case "squote":
                            translatedLabel = translatedLabel.Replace(tok.tokenText, string.Format("'{0}'", tok.tokenValue));
                            break;
                        case "span":
                            className = pd.GetString("class");
                            if (className.Length > 0)
                                className = "class=\"" + className + "\" ";
                            styleName = pd.GetString("style");
                            if (styleName.Length > 0)
                                styleName = "style=\"" + styleName + "\" ";
                            idName= pd.GetString("id");
                            if (idName.Length > 0)
                                idName = "id=\"" + idName+ "\" ";
                            if (pd.attributes.Count > attributePos)
                                attributes = pd.attributes[attributePos++];
                            else
                                attributes = "";
                            translatedLabel = translatedLabel.Replace(tok.tokenText, string.Format("<span {0}{1}{2}{3}>{4}</span>",attributes,idName, className, styleName, tok.tokenValue));
                            break;
                        case "link":
                            className = pd.GetString("class");
                            if (className.Length > 0)
                                className = "class=\"" + className + "\" ";
                            styleName = pd.GetString("style");
                            if (styleName.Length > 0)
                                styleName = "style=\"" + styleName + "\" ";
                            href = "href=\"" + pd.GetString("href") + "\" ";
                            if (pd.attributes.Count > attributePos)
                                attributes = pd.attributes[attributePos++];
                            else
                                attributes = "";
                            translatedLabel = translatedLabel.Replace(tok.tokenText, string.Format("<a {0}{1}{2}{3}>{4}</a>",attributes, href, className, styleName, tok.tokenValue));
                            break;
                        case "link1":
                            className = pd.GetString("class1");
                            if (className.Length > 0)
                                className = "class=\"" + className + "\" ";
                            styleName = pd.GetString("style1");
                            if (styleName.Length > 0)
                                styleName = "style=\"" + styleName + "\" ";
                            href = "href=\"" + pd.GetString("href1") + "\" ";
                            if (pd.attributes.Count > attributePos)
                                attributes = pd.attributes[attributePos++];
                            else
                                attributes = "";
                            translatedLabel = translatedLabel.Replace(tok.tokenText, string.Format("<a {0}{1}{2}{3}>{4}</a>",attributes, href, className, styleName, tok.tokenValue));
                            break;
                    }

                }
            }

            return translatedLabel;
        }

        protected string addInlineSpan(string label)
        {
            return string.Format("<span class=\"tr8n_translatable tr8n_{0}translated\" data-translation_key_id=\"{1}\">{2}</span>",hasTranslations(locale) ? "" : "not_",id,label);
        }

        public translation firstValidTranslation(string locale,ParamsDictionary tokenValues=null)
        {
            if (tokenValues == null)
                tokenValues = new ParamsDictionary();
            foreach(translation trans in translationsByLocale(locale))
            {
                if (trans.isValidTranslation(tokenValues))
                    return trans;
            }
            return null;
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

            // substitute transform tokens
            dataTokens = new tokenList("transform", translatedLabel);
            foreach (transformToken tok in dataTokens.tokens)
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

    public class translationKeyInterface
    {
        public string label { get; set; }
        public string description { get; set; }
        public int level { get; set; }
        public string locale { get; set; }
    }
}
