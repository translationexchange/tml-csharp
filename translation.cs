using System;
using System.Collections.Generic;
using System.Text;

namespace Tr8n
{
    public class translation : Tr8nBase
    {
        #region Member Variables
        private string m_translatedLabel = null;
        private string m_label = null;
        #endregion

        #region Properties
        public string translatedLabel
        {
            get { return m_translatedLabel == null ? "" : m_translatedLabel; }
            set { m_translatedLabel = value; }
        }

        public string label
        {
            get { return m_label == null ? "" : m_label; }
        }
        #endregion

        #region Methods
        public translation(string labelToTranslate)
        {
            m_label = labelToTranslate;
        }
        #endregion

    }
}
