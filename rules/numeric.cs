using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tr8n.rules
{
    public class numeric : rulesBase
    {
        #region Member Variables
        #endregion

        #region Properties
        public override bool multipart
        {
            get { return true; }
        }
        #endregion

        #region Methods

        public override bool evaluate()
        {
            //if (!multipart)
            //    return evaluateFragment(tokenValue,
            return false;
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
