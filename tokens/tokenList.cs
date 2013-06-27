using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tr8n.tokens
{
    public class tokenList
    {
        #region Constants
        public static Regex regexData = new Regex(@"(\{[^_:][\w]*(:[\w]+)?(::[\w]+)?\})", RegexOptions.Compiled);
        public static Regex regexTransform = new Regex(@"(\{[^_:|][\w]*(:[\w]+)?(::[\w]+)?\s*\|\|?[^{^}]+\})", RegexOptions.Compiled);
        #endregion

        #region Member Variables
        private List<tokenBase> m_tokens = null;
        #endregion

        #region Properties
        public List<tokenBase> tokens
        {
            get { return m_tokens; }
        }
        #endregion

        #region Methods
        public tokenList(string tokenType, string tml)
        {
            switch (tokenType.ToLower())
            {
                case "data":
                    m_tokens = GetTokens(typeof(dataToken), regexData, tml);
                    break;
                case "transform":
                    m_tokens = GetTokens(typeof(transformToken), regexTransform, tml);
                    break;
                default:
                    m_tokens = new List<tokenBase>();
                    break;
            }
        }

        private List<tokenBase> GetTokens(Type type, Regex regex, string label)
        {
            List<tokenBase> tokens = new List<tokenBase>();
            MatchCollection matches = regex.Matches(label);
            if (matches.Count > 0)
            {
                foreach (Match m in matches)
                {
                    for (int t = 1; t < m.Groups.Count; t++)
                        if (!string.IsNullOrEmpty(m.Groups[t].Value))
                            tokens.Add((tokenBase)Activator.CreateInstance(type, m.Groups[t].Value));
                }
            }
            return tokens;
        }
        #endregion

    }
}
