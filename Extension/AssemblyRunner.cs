using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Extension
{
    public class AssemblyRunner : IDisposable
    {
        public AppDomain DifferentAppDomain { get; private set; }

        public AssemblyRunner()
        {
            DifferentAppDomain = AppDomain.CreateDomain("newAppDomain");
        }

        public string Run(string assemblyName, string className, string methodName)
        {
            if (string.IsNullOrEmpty(assemblyName))
            {
                throw new ArgumentException("assembly is required");
            }

            if (string.IsNullOrEmpty(className))
            {
                throw new ArgumentException("className is required");
            }

            if (string.IsNullOrEmpty(methodName))
            {
                throw new ArgumentException("methodName is required");
            }

            var type = typeof(Proxy);
            var proxy = (Proxy)DifferentAppDomain.CreateInstanceFromAndUnwrap(
                type.Assembly.Location,
                type.FullName);
            var result = proxy.CallAndReturnString(assemblyName, className, methodName);
            return result;
        }

        public void Dispose()
        {
            if (disposed == false)
            {
                AppDomain.Unload(DifferentAppDomain);
                disposed = true;
            }
        }

        private bool disposed = false;


        private class Proxy : MarshalByRefObject
        {
            public string CallAndReturnString(string assemblyName, string className, string methodName)
            {
                var assembly = Assembly.LoadFrom(assemblyName);
                var thatClass = assembly.GetExportedTypes()
                    .Where(type => type.Name.EndsWith(className))
                    .First();
                var thatMethod = thatClass
                    .GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
                return thatMethod.Invoke(null, null) as string;
            }
        }
    }
}
