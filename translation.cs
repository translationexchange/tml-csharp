using System;
using System.Collections.Generic;
using System.Text;

namespace Tr8n
{
    public class translation : Tr8nBase
    {
        #region Member Variables
        private string m_label = null;
        private int m_id = 0;
        private int m_rank = 0;
        private string m_locale = null;
        #endregion

        #region Properties
        public string locale
        {
            get { return m_locale == null ? "" : m_locale; }
        }
        public string label
        {
            get { return m_label == null ? "" : m_label; }
        }

        public int id
        {
            get { return m_id; }
        }

        public int rank
        {
            get { return m_rank; }
        }

        #endregion

        #region Methods
        public translation()
        {
        }

        public translation(int id, string label,string locale="", int rank = 0)
        {
            m_id = id;
            m_rank = rank;
            m_label = label;
            m_locale = locale;
        }

        public bool isValidTranslation(ParamsDictionary paramData)
        {
            return true;
        }

        #endregion

    }
}
