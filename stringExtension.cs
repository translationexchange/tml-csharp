using System;
using System.Collections.Generic;
using System.Text;


namespace Tr8n
{
    // String extensions
    public static class se
    {
        #region String Extension Methods

        private static string translatePD(this String str, ParamsDictionary pd)
        {
            return new language().translate(str, pd);
        }

        public static string translate(this String str, params object[] items)
        {
            ParamsDictionary pd = new ParamsDictionary(items);
            return translatePD(str, pd);
        }

        public static string translatep(this String str, params object[] items)
        {
            ParamsDictionary pd = new ParamsDictionary(str, items);
            return translatePD(str, pd);
        }

         
        #endregion

        #region Methods
        
        public static string tr(string label, params object[] items)
        {
            return new language().translate(label, items);
        }
        #endregion

    }
}
