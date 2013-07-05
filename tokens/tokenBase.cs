﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tr8n.tokens
{

    public class tokenBase
    {
        #region Member Variables
        private string m_tokenText = null;
        private string m_name = null;

        private static char[] m_nameEndingChars = {':','|','}' }; 

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
                            m_name = tokenText.Substring(1, pos);
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
        #endregion

        #region Methods
        public tokenBase(string tokenText)
        {
            m_tokenText = tokenText;
        }
        #endregion
    }
}