using System;
using System.Collections.Generic;
using System.Text;

namespace Tr8n
{
    public class language : Tr8nBase
    {
        #region Constants
        public const string labelContextToken="#description";
        #endregion

        #region Member Variables
        #endregion

        #region Properties
        #endregion

        #region Methods
        public string translate(string label, params object[] items)
        {
            string context = (string)GetParam(labelContextToken,items);

            return new translationKey(label,context).translate(items);
        }
        #endregion

    }
}
