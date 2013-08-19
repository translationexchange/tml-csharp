using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tr8n.decorators
{
    class htmlDecorator : baseDecorator
    {
        public override string decorate(translationKey transKey, string language, string label, ParamsDictionary options)
        {
            return label;
        }
    }
}
