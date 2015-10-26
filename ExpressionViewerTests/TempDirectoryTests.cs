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
        public void TempDirectory_FullName_ReturnsExistingDirectory()
        {
            var tempDirectory = new TempDirectory();

            var directory = tempDirectory.FullName;

            Assert.IsTrue(Directory.Exists(directory));
        }

        [TestMethod]
        public void TempDirectory_FullNameCalledTwice_ReturnsSameName()
        {
            var tempDirectory = new TempDirectory();

            var directory1 = tempDirectory.FullName;
            var directory2 = tempDirectory.FullName;

            Assert.AreEqual(directory1, directory2);
        }

        [TestMethod]
        public void TempDirectory_CallingDispose_DeletesDirectory()
        {
            var tempDirectory = new TempDirectory();
            var directory = tempDirectory.FullName;

            tempDirectory.Dispose();

            Assert.IsFalse(Directory.Exists(directory));
        }

        [TestMethod]
        public void TempDirectory_CallingDispose_DeletesDirectoryContent()
        {
            var tempDirectory = new TempDirectory();
            var filePath = Path.Combine(tempDirectory.FullName, "dummy.txt");
            File.WriteAllText(filePath, "some random content");

            tempDirectory.Dispose();

            Assert.IsFalse(File.Exists(filePath));
        }

        [TestMethod]
        public void TempDirectory_CallingDisposeWhenFilesLocked_DoesntFail()
        {
            var tempDirectory = new TempDirectory();
            var filePath = Path.Combine(tempDirectory.FullName, "dummy.txt");
            File.WriteAllText(filePath, "some random content");
            var stream = File.OpenRead(filePath);

            tempDirectory.Dispose();

            Assert.IsTrue(File.Exists(filePath));

            stream.Close();
            Directory.Delete(tempDirectory.FullName, true);
        }
    }
}
