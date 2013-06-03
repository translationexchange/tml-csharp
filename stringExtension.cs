using System;
using System.Collections.Generic;
using System.Text;


namespace Tr8n
{
    public static class stringExtension
    {
        #region String Extension Methods
        public static string translate(this String str, params object[] items)
        {
            return new language().translate(str, items);
        }
        #endregion

    }
}
