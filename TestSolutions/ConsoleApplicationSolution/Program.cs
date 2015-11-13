using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    public class Program
    {
        public string Greatings()
        {
            return new StringBuilder("Hello!!!")
                .Insert(0, "<a>")
                .Append("</a>")
                .ToString();
        }
		
		public static void Main(string[] args)
        {
        }
    }
}
