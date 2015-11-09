using Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExpressionViewerTests
{
    [TestClass]
    public class RunnerTests
    {
        [TestMethod]
        public void Runner_SourceChanged_DrawView()
        {
            var solution = "solution.sln";
            var sourceMonitor = new Mock<ISourceMonitor>(MockBehavior.Loose);
            var viewController = new Mock<IViewController>(MockBehavior.Loose);
            var content = "content";
            var viewGenerator = new Mock<IViewGenerator>(MockBehavior.Loose);
            viewGenerator
                .Setup(generator => generator.GenerateViewAsync(solution))
                .Returns(Task.FromResult(content));
            var runner = new Runner(sourceMonitor.Object, viewController.Object, viewGenerator.Object);

            sourceMonitor.Raise(mock => mock.SourceChanged += null, new EventArgs());

            viewController.Verify(mock => mock.Draw(content), Times.Once());
        }

        [TestMethod]
        public void ViewDrawer_Draw_UpdateView()
        {
            var view = new Mock<IView>(MockBehavior.Loose);
            var viewController = new ExpressionViewController(view.Object);

            var content = "something to be displayed";
            viewController.Draw(content);

            view.Verify(mock => mock.SetText(content));
        }

        [TestMethod]
        public void SourceMonitor_Object_IsDisposable()
        {
            var sourceMonitor = new SourceMonitor();

            Assert.IsTrue(sourceMonitor is IDisposable);
        }

        [TestMethod]
        public void SourceMonitor_FromTimeToTime_TriggerSourceChanged()
        {
            var sourceMonitor = new SourceMonitor();

            var changes = 0;
            sourceMonitor.SourceChanged += (s, e) => changes += 1;

            var arbitraryExpectedChanges = 5;
            while (changes < arbitraryExpectedChanges)
            {
                Thread.Sleep(10);
            }
        }
    }
}
