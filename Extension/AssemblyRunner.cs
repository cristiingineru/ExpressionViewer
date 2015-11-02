using System;
using System.Collections.Generic;
using System.IO;
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
            AppDomainSetup domainSetup = new AppDomainSetup();
            domainSetup.ApplicationName = "newAppDomain";
            domainSetup.ApplicationBase = Environment.CurrentDirectory;                 
            DifferentAppDomain = AppDomain.CreateDomain("newAppDomain", null, domainSetup);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        private static string LastRequestedAssembly = String.Empty;

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name == LastRequestedAssembly)
            {
                return null;
            }
            LastRequestedAssembly = args.Name;
            try
            {
                Assembly assembly = System.Reflection.Assembly.Load(args.Name);
                if (assembly != null)
                    return assembly;
            }
            catch
            { /* ignore load error */ }

            // *** Try to load by filename - split out the filename of the full assembly name
            // *** and append the base path of the original assembly (ie. look in the same dir)
            // *** NOTE: this doesn't account for special search paths but then that never
            //           worked before either.
            string[] Parts = args.Name.Split(',');
            string File = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + Parts[0].Trim() + ".dll";

            return System.Reflection.Assembly.LoadFrom(File);
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
            var obj = DifferentAppDomain.CreateInstanceFromAndUnwrap(
                type.Assembly.Location,
                type.FullName);
            var proxy = (Proxy)obj;
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
