using Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionViewerTests
{
    [TestClass]
    public class TempDirectoryTests
    {
        [TestMethod]
        public void TempDirectory_Constructor_ReturnsIDisposableObject()
        {
            var tempDirectory = new TempDirectory();

            Assert.IsTrue(tempDirectory is IDisposable);
        }

        [TestMethod]
        public void TempDirectory_Directory_ReturnsExistingDirectory()
        {
            var tempDirectory = new TempDirectory();

            var directory = tempDirectory.Directory;

            Assert.IsTrue(Directory.Exists(directory));
        }
    }
}
