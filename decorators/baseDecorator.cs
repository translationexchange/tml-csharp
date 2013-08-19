using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tr8n.decorators
{
    public abstract class baseDecorator
    {
        public abstract string decorate(translationKey transKey,string language,string label,ParamsDictionary options);
    }
}
