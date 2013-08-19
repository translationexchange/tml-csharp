using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tr8n
{
    public class Tr8nException : Exception
    {
        public Tr8nException()
        {
        }

        public Tr8nException(string message)
            : base(message)
        {
        }

        public Tr8nException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
