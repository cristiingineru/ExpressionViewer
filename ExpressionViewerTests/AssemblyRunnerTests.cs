using Extension;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionViewerTests
{

    [Serializable]
    public class Lalala : MarshalByRefObject
    {
        public static void DifferentAppDomain_DomainUnload(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }

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
        public void AssemblyRunner_WithValidArguments_ReturnsString()
        {
            var runner = new AssemblyRunner();

            var result = runner.Run("DummyDll.dll", "DummyClass", "SayHello");

            Assert.IsFalse(string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void AssemblyRunner_WithFullPathDll_ReturnsString()
        {
            var runner = new AssemblyRunner();

            var result = runner.Run(Path.Combine(Directory.GetCurrentDirectory(), "DummyDll.dll"), "DummyClass", "SayHello");

            Assert.IsFalse(string.IsNullOrEmpty(result));
        }
    }
}
