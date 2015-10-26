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
                try
                {
                    Directory.Delete(FullName, true);
                }
                catch (Exception)
                {
                    // Files in use can prevent deleting the parent directory.
                    // It's fine since this operation is meant to save some disk space only.
                }
                disposed = true;
            }
        }

        public TempDirectory()
        {
            FullName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(FullName);
        }

        public string FullName { get; private set; }

        private bool disposed = false;
    }
}
