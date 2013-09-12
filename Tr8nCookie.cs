using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
namespace Tr8n
{
    public class Tr8nCookie
    {
        private bool m_exists = false;
        private string m_locale = null;
        private int m_translatorId = -1;
        private bool m_inlineTranslation = false;

        public bool exists
        {
            get { return m_exists; }
        }

        public string locale
        {
            get { return m_locale == null ? "" : m_locale; }
        }

        public int translatorId
        {
            get { return m_translatorId; }
        }

        public bool inlineTranslation
        {
            get { return m_inlineTranslation; }
        }

        public Tr8nCookie()
        {
            ParseCookie();
        }

        private void ParseCookie()
        {
            try
            {
                HttpCookie c = HttpContext.Current.Request.Cookies["tr8n_" + application.clientId];
                m_exists = false;
                if (c == null)
                    return;
                if (string.IsNullOrEmpty(c.Value))
                    return;
                string[] parts = HttpUtility.UrlDecode(c.Value).Split('.');
                if (parts.Length < 2)
                    return;
                string data = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(HttpUtility.UrlDecode(parts[1])));
                json j = new json(data);
                if (j == null)
                    return;
                m_locale = j.GetField("locale");
                m_translatorId = Util.Nzn(j.GetField("translator:id"), 0);
                m_inlineTranslation = j.GetFieldBool("translator:inline", m_inlineTranslation);
                m_exists = true;
            }
            catch {
                m_exists = false;
            }
        }
    }
}
