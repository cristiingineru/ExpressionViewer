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
        public void AssemblyRunner_Constructor_ReturnsIDisposable()
        {
            var runner = new AssemblyRunner();

            Assert.IsTrue(runner is IDisposable);
        }

        [TestMethod]
        [Ignore]
        public void AssemblyRunner_AfterDisposing_TheDifferentAppDomainIsUnloaded()
        {
            var runner = new AssemblyRunner();

            var unloaded = false;
            runner.DifferentAppDomain.DomainUnload += new EventHandler(delegate (object o, EventArgs a)
            {
                unloaded = true;
            });

            runner.Dispose();

            Assert.IsTrue(unloaded);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AssemblyRunner_WithNullClassName_Fails()
        {
            var runner = new AssemblyRunner();

            runner.Run("assembly.dll", null, "methodName");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AssemblyRunner_WithNullMethodName_Fails()
        {
            var runner = new AssemblyRunner();

            runner.Run("assembly.dll", "className", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AssemblyRunner_WithNullAssembly_Fails()
        {
            var runner = new AssemblyRunner();

            runner.Run(null, "className", "methodName");
        }

        [TestMethod]
        public void AssemblyRunner_WithValidArguments_ReturnsStringValue()
        {
            var runner = new AssemblyRunner();

            var result = runner.Run("MiniTestProject.dll", "MiniTestClass", "SayHello");

            Assert.IsFalse(string.IsNullOrEmpty(result));
        }
    }
}
