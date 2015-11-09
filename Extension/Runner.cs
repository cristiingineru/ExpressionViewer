using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Extension
{
    public interface IViewGenerator
    {
        Task<string> GenerateViewAsync(string solutionPath);
    }

    public interface ISourceMonitor
    {
        event EventHandler SourceChanged;
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

    public class Runner
    {
        public ISourceMonitor SourceMonitor { get; private set; }
        public IViewController ViewController { get; private set; }
        public IViewGenerator ViewGenerator { get; private set; }
        public Runner(ISourceMonitor sourceMonitor, IViewController viewController, IViewGenerator viewGenerator)
        {
            SourceMonitor = sourceMonitor;
            ViewController = viewController;
            ViewGenerator = viewGenerator;

            SourceMonitor.SourceChanged += async (s, e) =>
            {
                var content = await viewGenerator.GenerateViewAsync("solution.sln");
                ViewController.Draw(content);
            };
        }
    }
}
