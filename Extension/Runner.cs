using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;

namespace Extension
{
    public interface IViewGenerator
    {
        Task<string> GenerateViewAsync(string solutionPath, string activeDocument, int cursorPosition);
    }

    public interface ISourceMonitor
    {
        event EventHandler SourceChanged;
    }

    public class SourceMonitorArgs : EventArgs
    {
        public string Solution { get; internal set; }
        public string ActiveDocument { get; internal set; }
        public int CursorPosition { get; internal set; }
        public SourceMonitorArgs(string solution, string activeDocument, int cursorPosition)
        {
            this.Solution = solution;
            this.ActiveDocument = activeDocument;
            this.CursorPosition = cursorPosition;
        }
    }

    public class SourceMonitor : ISourceMonitor, IDisposable
    {
        public event EventHandler SourceChanged;

        public SourceMonitor(IServiceProvider serviceProvider)
        {
            Timer = new Timer(1000);
            Timer.AutoReset = false;
            Timer.Elapsed += (sender, args) =>
            {
                var solutionFullName = GetSolutionFullName(serviceProvider);
                var activeDocument = GetActiveDocument(serviceProvider);
                var cursorPosition = GetCursorPosition(serviceProvider);
                var arguments = new SourceMonitorArgs(solutionFullName, activeDocument, cursorPosition);
                if (SourceChanged != null)
                {
                    SourceChanged(this, arguments);
                }
                Timer.Start();
            };
            Timer.Start();
        }

        private string GetSolutionFullName(IServiceProvider serviceProvider)
        {
            var solutionFullName = String.Empty;
            if (serviceProvider != null)
            {
                var dte = (DTE)serviceProvider.GetService(typeof(DTE));
                if (dte != null)
                {
                    var solution = dte.Solution;
                    if (solution != null)
                    {
                        solutionFullName = solution.FullName;
                    }
                }
            }
            return solutionFullName;
        }

        private string GetActiveDocument(IServiceProvider serviceProvider)
        {
            var activeDocumentFullName = String.Empty;
            if (serviceProvider != null)
            {
                var dte = (DTE)serviceProvider.GetService(typeof(DTE));
                if (dte != null)
                {
                    var document = dte.ActiveDocument;
                    if (document != null)
                    {
                        activeDocumentFullName = document.FullName;
                    }
                }
            }
            return activeDocumentFullName;
        }

        private int GetCursorPosition(IServiceProvider serviceProvider)
        {
            var cursorPosition = -1;
            if (serviceProvider != null)
            {
                var dte = (DTE)serviceProvider.GetService(typeof(DTE));
                if (dte != null)
                {
                    var document = dte.ActiveDocument;
                    if (document != null)
                    {
                        var selection = document.Selection as TextSelection;
                        cursorPosition = selection.ActivePoint.AbsoluteCharOffset;
                    }
                }
            }
            return cursorPosition;
        }

        public void Dispose()
        {
            if (disposed == false)
            {
                Timer.Stop();
                disposed = true;
            }
        }

        private Timer Timer;
        private bool disposed = false;
    }

    public interface IView
    {
        void SetText(string content);
    }

    public interface IViewController
    {
        void Draw(string content);
    }

    public class ExpressionViewController : IViewController
    {
        public IView View { get; private set; }

        public ExpressionViewController(IView view)
        {
            View = view;
        }

        public void Draw(string content)
        {
            View.SetText(content);
        }
    }

    public class Runner : IDisposable
    {
        public ISourceMonitor SourceMonitor { get; private set; }
        public IViewController ViewController { get; private set; }
        public IViewGenerator ViewGenerator { get; private set; }
        public Runner(ISourceMonitor sourceMonitor, IViewController viewController, IViewGenerator viewGenerator)
        {
            SourceMonitor = sourceMonitor;
            ViewController = viewController;
            ViewGenerator = viewGenerator;

            SourceMonitor.SourceChanged += SourceMonitor_SourceChanged;
        }

        public void Dispose()
        {
            if (disposed == false)
            {
                SourceMonitor.SourceChanged -= SourceMonitor_SourceChanged;
                disposed = true;
            }
        }

        private async void SourceMonitor_SourceChanged(object sender, EventArgs e)
        {
            var arguments = e as SourceMonitorArgs;
            var content = await ViewGenerator.GenerateViewAsync(
                arguments.Solution,
                arguments.ActiveDocument,
                arguments.CursorPosition);
            ViewController.Draw(content);
        }

        private bool disposed = false;
    }
}
