using System;
using System.Diagnostics;

namespace MiniTestProject
{
    public class MiniTestClass
    {
        public string Do()
        {
            return "text"
                .ToString()
                .ToString();
        }

        public static string SayHello()
        {
            return "hello"
                .Insert(0, "<a>")
                .ToString();
        }
    }
}
