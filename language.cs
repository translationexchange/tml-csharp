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
            string context = null;

            // look for a context
            for (int t = 0; t < items.Length; t++)
            {
                if (items[t] is string && (string)items[t] == labelContextToken)
                {
                    // found the context token -- now grab the next item as the context if we aren't on the last item
                    if (t < items.Length - 1)
                    {
                        context = (string)items[t + 1];
                    }
                }
            }
            return new translationKey(label,context).translate(items);
        }
        #endregion

    }
}
