using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tr8n.rules
{
    public class rulesBase
    {
        #region Member Variables
        private string m_tokenValue = null;
        #endregion

        #region Properties
        public string tokenValue
        {
            get { return m_tokenValue == null ? "" : m_tokenValue; }
        }

        public virtual bool multipart
        {
            get { return true; }
        }
        #endregion

        #region Methods

        public virtual bool evaluate()
        {
            return false;
        }

        public virtual bool evaluateFragment(string tokenVal, string command, string values)
        {
            return false;
        }
        #endregion
    }
}
