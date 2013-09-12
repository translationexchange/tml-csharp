using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tr8n.tokens
{

    public class tokenBase
    {
        #region Member Variables
        protected string m_tokenText = null;
        protected string m_name = null;
        protected string m_tokenValue = null;

        protected static char[] m_nameEndingChars = { ':', '|', '}' }; 

        #endregion

        #region Properties
        public string tokenText
        {
            get { return m_tokenText == null ? "" : m_tokenText; }
        }


        public string name
        {
            get
            {
                if (m_name == null)
                {
                    if (tokenText.Length < 2)
                        m_name = "";
                    else
                    {
                        int pos = tokenText.IndexOfAny(m_nameEndingChars, 1);
                        if (pos > 1)
                            m_name = tokenText.Substring(1, pos-1);
                        else
                            m_name = tokenText.Substring(1);
                        if (m_name.EndsWith("}"))
                            m_name=m_name.Substring(0,m_name.Length-1);
                        m_name=m_name.Trim();
                    }
                }
                return m_name;
            }
        }

        public virtual string tokenValue
        {
            get
            {
                return m_tokenValue == null ? "" : m_tokenValue;
            }
        }

        #endregion

        #region Methods
        public tokenBase(string tokenText)
        {
            m_tokenText = tokenText;
        }
        #endregion
    }
}
