using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tr8n.rules
{
    public class numeric : rulesBase
    {
        #region Member Variables
        private string m_part1 = null;
        private string m_part2 = null;
        private string m_numOperator = null;
        private int m_value1 = 0;
        private int m_value2 = 0;
        #endregion

        #region Properties

        public string part1
        {
            get { return m_part1 == null ? "" : m_part1; }
            set { m_part1 = value; }
        }

        public string part2
        {
            get { return m_part2 == null ? "" : m_part2; }
            set { m_part2 = value; }
        }

        public string numOperator
        {
            get { return m_numOperator == null ? "" : m_numOperator; }
            set { m_numOperator = value; }
        }

        public int value1
        {
            get { return m_value1; }
            set { m_value1 = value; }
        }
        
        public int value2
        {
            get { return m_value2; }
            set { m_value2 = value; }
        }

        public override bool multipart
        {
            get { return true; }
        }
        #endregion

        #region Methods

        public override bool evaluate()
        {
            bool result1=evaluateFragment(tokenValue, part1, value1.ToString());
            if (!multipart)
                return result1;
            bool result2 = evaluateFragment(tokenValue, part2, value2.ToString());
            if (numOperator == "or")
                return result1 || result2;
            return result1 && result2;
        }

        public override bool evaluateFragment(string tokenVal, string command, string values)
        {
            switch (command)
            {
                case "is":
                    return values.Contains(tokenVal);
                case "is_not":
                    return !values.Contains(tokenVal);
                case "ends_in":
                    return tokenVal.EndsWith(values);
                case "does_not_end_in":
                    return !tokenVal.EndsWith(values);
                case "starts_with":
                    return tokenVal.StartsWith(values);
                case "does_not_start_with":
                    return !tokenVal.StartsWith(values);
            }
            return false;
        }

        #endregion
    }
}
