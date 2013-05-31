using System;
using System.Collections.Generic;
using System.Text;

namespace Tr8n
{
    public class client : Tr8nBase
    {
        #region Member Variables
        private config m_config = null;
        #endregion

        #region Properties
        public config config
        {
            get
            {
                if (m_config == null)
                    m_config = new config();
                return m_config;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Sets the config file you are using
        /// </summary>
        /// <param name="configFile">Full file path and name of the config file</param>
        public void SetConfig(string configFile)
        {
            m_config = new config(configFile);
        }
        #endregion

    }
}
