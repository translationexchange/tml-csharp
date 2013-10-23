using System;
using System.Collections.Generic;
using System.Text;

namespace Tr8n
{
    public class source : Tr8nBase
    {
        #region Member Variables
        private string m_sourceKey = null;
        private string m_locale = null;
        private string m_inlineId = null;
        #endregion

        #region Properties
        public string sourceKey
        {
            get
            {
                return m_sourceKey == null ? "" : m_sourceKey;
            }
            set
            {
                m_sourceKey = value == null ? "" : value.Trim().ToLower();
            }
        }

        public string locale
        {
            get { return m_locale == null ? "" : m_locale; }
            set { m_locale = value; }
        }

        public bool isInline
        {
            get{return string.IsNullOrEmpty(m_inlineId) ? false : true;}
        }

        public string cacheKey
        {
            get
            {
                return locale + ":" + sourceKey+":"+m_inlineId;
            }
        }
        #endregion

        #region Methods
        public source(string sourceKey,string locale,string inlineId)
        {
            this.sourceKey = sourceKey;
            this.locale = locale;
            m_inlineId = inlineId;
        }

        public void Load()
        {
            // This is a temporary cache, so just clear it after it has 10000 keys
            if (application.translationKeyCacheInline.Count > 10000)
                application.translationKeyCacheInline.Clear();

            if (application.translationKeyCacheInline.ContainsKey(cacheKey))
                return;
            application.translationKeyCacheInline.TryAdd(cacheKey, null);


            json j = apiGet("source/translations", "source=" + System.Web.HttpUtility.UrlEncode(sourceKey)+"&locale="+locale);
            if (j == null || j.GetField("error").Length > 0)
                return;

            foreach (Dictionary<string, object> sourceItem in j.GetFieldList("results"))
            {
                json jSource = new json(sourceItem);
                translationKey k = new translationKey(jSource.GetField("locale"), jSource.GetField("label"), jSource.GetField("description"));
                k.id = Util.Nzn(jSource.GetField("id"), 0);
                k.isLocked = jSource.GetFieldBool("locked", false);

                Dictionary<string, List<translation>> tList = new Dictionary<string, List<translation>>();
                foreach (Dictionary<string, object> item in jSource.GetFieldList("translations:"+locale.ToLower()))
                {
                    json jTemp = new json(item);
                    translation trans = new translation(Util.Nzn(jTemp.GetField("id")), jTemp.GetField("label"), jTemp.GetField("locale"), Util.Nzn(jTemp.GetField("rank")));
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

                application.translationKeyCacheInline[k.hashKey]=k;
            }
        }

        #endregion

    }

    public class sourceKeyInterface
    {
        public string source { get; set; }
        public List<translationKeyInterface> keys = new List<translationKeyInterface>();
    }

}
