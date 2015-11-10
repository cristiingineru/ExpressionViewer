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
            var sourceMonitor = new Mock<ISourceMonitor>(MockBehavior.Loose);
            var viewController = new Mock<IViewController>(MockBehavior.Loose);
            var solution = "solution.sln";
            var content = "content";
            var viewGenerator = new Mock<IViewGenerator>(MockBehavior.Loose);
            viewGenerator
                .Setup(generator => generator.GenerateViewAsync(solution))
                .Returns(Task.FromResult(content));
            var runner = new Runner(sourceMonitor.Object, viewController.Object, viewGenerator.Object);

            sourceMonitor.Raise(mock => mock.SourceChanged += null, new SourceMonitorArgs(solution));

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
            var sourceMonitor = new SourceMonitor(null);

            Assert.IsTrue(sourceMonitor is IDisposable);
        }

        [TestMethod]
        public void SourceMonitor_FromTimeToTime_TriggerSourceChanged()
        {
            var sourceMonitor = new SourceMonitor(null);

            var changes = 0;
            sourceMonitor.SourceChanged += (s, e) => changes += 1;

            var arbitraryExpectedChanges = 3;
            while (changes < arbitraryExpectedChanges)
            {
                Thread.Sleep(10);
            }
        }

        [TestMethod]
        public void SourceMonitor_AfterDisposing_DoesntTriggerSourceChanged()
        {
            var sourceMonitor = new SourceMonitor(null);

            sourceMonitor.Dispose();

            var changes = 0;
            sourceMonitor.SourceChanged += (s, e) => changes += 1;
            var arbitraryWaitTime = 1500;
            Thread.Sleep(arbitraryWaitTime);
            Assert.AreEqual(0, changes);
        }
    }
}
