using Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionViewerTests
{
    [TestClass]
    public class AssemblyRunnerTests
    {
        [TestMethod]
        public void AssemblyRunner_WithNullPath_ReturnsNothing()
        {
            var runner = new AssemblyRunner();

            var result = runner.Run(null);

            Assert.AreEqual(String.Empty, result as string);
        }
    }
}
