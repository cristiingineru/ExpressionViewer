using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManualTesting
{
    public class SimpleClass
    {
        public string SimpleMethod()
        {
            return new StringBuilder("Hello!!!")
                .Insert(0, "<a>")
                .Append("</a>")
                .ToString();
        }
    }
}
