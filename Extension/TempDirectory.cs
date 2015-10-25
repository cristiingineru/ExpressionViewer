using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension
{
    public class TempDirectory : IDisposable
    {
        public void Dispose()
        {
            if (disposed == false)
            {
                disposed = true;
            }
        }

        public TempDirectory()
        {
            Directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            System.IO.Directory.CreateDirectory(Directory);
        }

        public string Directory { get; private set; }

        private bool disposed = false;
    }
}
