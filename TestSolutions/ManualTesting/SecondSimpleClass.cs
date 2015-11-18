using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManualTesting
{
    public class SecondSimpleClass
    {
        public string SecondSimpleMethod()
        {
            return "Hello!!!"
                .Split("-")
                .Insert(0, "<a>")
                .Append("</a>");
        }

        private void UsingRequired()
        {
            var x = Directory.Exists("a.txt");
        }
    }
}
