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
            return "Hello"
                .Insert(0, " ")
                .Insert(0, ":")
                .Insert(0, "Just wanted to say");
        }
    }
}
