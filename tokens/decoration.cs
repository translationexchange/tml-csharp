using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tr8n.tokens
{
    public class decorationToken : tokenBase
    {
        #region Member Variables
        #endregion

        #region Properties
        #endregion

        #region Methods
        public decorationToken(string tml)
            : base(tml)
        {
        }

        public override string tokenValue
        {
            get
            {
                if (m_tokenValue == null)
                {
                    if (tokenText.Length < 2)
                        m_tokenValue = "";
                    else
                    {
                        int pos = tokenText.IndexOfAny(m_nameEndingChars, 1);
                        if (pos > 1)
                        {
                            if (tokenText.Length >= pos + 1)
                                m_tokenValue = tokenText.Substring(pos + 1);
                            else
                                m_tokenValue = "";
                        }
                        else
                            m_tokenValue = "";
                        // remove the ending ]
                        if (m_tokenValue.Length > 0)
                            m_tokenValue = m_tokenValue.Substring(0, m_tokenValue.Length - 1);
                    }
                }
                return m_tokenValue;
            }
        }


        #endregion
    }
}
