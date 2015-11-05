using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension
{
    public interface ISourceMonitor
    {
        event EventHandler SourceChanged;
    }

    public interface IViewDrawer
    {
        void Draw();
    }

    public class Runner
    {
        public ISourceMonitor SourceMonitor { get; private set; }
        public IViewDrawer ViewDrawer { get; private set; }
        public Runner(ISourceMonitor sourceMonitor, IViewDrawer viewDrawer)
        {
            SourceMonitor = sourceMonitor;
            ViewDrawer = viewDrawer;

            SourceMonitor.SourceChanged += (s, e) => ViewDrawer.Draw();
        }
    }
}
