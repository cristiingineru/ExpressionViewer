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
        public Action DifferentAppDomainDestructor { get; private set; }

        public AssemblyRunner()
        {
            var result = CreateNewAppDomain("newAppDomain");

            DifferentAppDomain = result.Item1;
            DifferentAppDomainDestructor = result.Item2;
        }

        private Tuple<AppDomain, Action> CreateNewAppDomain(string friendlyName)
        {
            AppDomainSetup domainSetup = new AppDomainSetup();
            domainSetup.ApplicationName = friendlyName;
            domainSetup.ApplicationBase = Environment.CurrentDirectory;
            var assemblyResolveEventHandler = new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            AppDomain.CurrentDomain.AssemblyResolve += assemblyResolveEventHandler;
            Action destructor = () => AppDomain.CurrentDomain.AssemblyResolve -= assemblyResolveEventHandler;
            var newAppDomain = AppDomain.CreateDomain(friendlyName, null, domainSetup);
            return Tuple.Create(newAppDomain, destructor);
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
                DifferentAppDomainDestructor();
                AppDomain.Unload(DifferentAppDomain);
                disposed = true;
            }
        }

        private bool disposed = false;

        private static string LastRequestedAssembly = string.Empty;

        /// <summary>
        /// This event is needed in tests only. Running in production didn't required this workaround.
        /// Details here:
        /// http://stackoverflow.com/questions/1437831/appdomain-createinstancefromandunwrap-unable-to-cast-transparent-proxy
        /// http://weblog.west-wind.com/posts/2009/Jan/19/Assembly-Loading-across-AppDomains
        /// </summary>
        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name == LastRequestedAssembly)
            {
                /// I'm preventing a stack overflow in case the assemly was not found
                return null;
            }
            LastRequestedAssembly = args.Name;
            try
            {
                Assembly assembly = Assembly.Load(args.Name);
                if (assembly != null)
                {
                    return assembly;
                }
            }
            catch
            {
                /* ignore load error */
            }

            string[] Parts = args.Name.Split(',');
            string File = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + Parts[0].Trim() + ".dll";

            return Assembly.LoadFrom(File);
        }


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
