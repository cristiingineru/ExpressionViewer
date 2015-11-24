using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
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
		
		public void Greatings2()
        {
            var greatings = new StringBuilder("!!!Hello!!!")
                .Insert(0, "<a>")
                .Append("</a>")
                .ToString();
        }
    }
}
