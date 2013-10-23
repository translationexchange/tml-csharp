using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tr8n
{
    public class UptimeMonitor
    {
        #region Member Variables
        private DateTime m_nextAttempt = new DateTime(1900, 1, 1);
        private int m_maxFailureTimes = 3;
        private int m_maxFailureWindowInSeconds = 60;
        private int m_reattemptInSeconds = 120;
        private List<DateTime> m_failures = new List<DateTime>();
        private object lockObj = new object();

        #endregion

        #region Properties

        public bool shouldAttempt
        {
            get
            {
                if (DateTime.Now > m_nextAttempt)
                    return true;
                return false;
            }
        }

        public DateTime nextAttempt
        {
            get
            {
                return m_nextAttempt;
            }
        }
        #endregion

        #region Methods
        public UptimeMonitor(int failureTimes=3, int numberOfSeconds=60,int reattemptInSeconds=120)
        {
            m_maxFailureTimes = failureTimes;
            m_maxFailureWindowInSeconds = numberOfSeconds;
            m_reattemptInSeconds = reattemptInSeconds;
        }

        public void BadResponse()
        {
            lock (lockObj)
            {
                // make sure another thread hasn't done this already
                if (m_nextAttempt < DateTime.Now)
                {
                    m_failures.Insert(0, DateTime.Now);
                    if (m_failures.Count > m_maxFailureTimes)
                        m_failures.RemoveRange(m_maxFailureTimes, m_failures.Count - m_maxFailureTimes);
                    // are there enough failure already to qualify shutting down the server access?
                    if (m_failures.Count >= m_maxFailureTimes)
                    {
                        // Did the failures happen in a recent enough timeframe?
                        DateTime window = DateTime.Now.AddSeconds(-1 * m_maxFailureWindowInSeconds);
                        if (m_failures.TrueForAll(item => item > window))
                        {
                            // shut it down for a while
                            m_nextAttempt = DateTime.Now.AddSeconds(m_reattemptInSeconds);
                        }
                    }
                }

            }
        }
        #endregion

    }
}
