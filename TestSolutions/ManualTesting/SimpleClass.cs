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
            return "Hello!!!"
                .Split("-")
                .Insert(0, "<a>")
                .Append("</a>")
                .ToString();
        }
    }

    public static class StringExtensions
    {
        public static string Append(this string text, string x)
        {
            return text + x;
        }

        public static string Split(this string text, string x)
        {
            return string.Join(x, text.ToString().ToArray());
        }
    }
}
