﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManualTesting
{
    public class SecondSimpleClass
    {
        public string CiaoMethod()
        {
            return "Ciao!!!"
                .Split("-")
                .Insert(0, "<a>")
                .Append("</a>");
        }

        public string SalutMethod()
        {
            return "Salut!!!"
                .Split("-")
                .Insert(0, "<a>")
                .Append("</a>");
        }
    }
}
