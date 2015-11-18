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
            var activeDocument = String.Empty;
            var content = "content";
            var viewGenerator = new Mock<IViewGenerator>(MockBehavior.Loose);
            viewGenerator
                .Setup(generator => generator.GenerateViewAsync(solution, activeDocument))
                .Returns(Task.FromResult(content));
            var runner = new Runner(sourceMonitor.Object, viewController.Object, viewGenerator.Object);

            sourceMonitor.Raise(mock => mock.SourceChanged += null, new SourceMonitorArgs(solution, activeDocument));

            viewController.Verify(mock => mock.Draw(content), Times.Once());
        }

        [TestMethod]
        public void Runner_Object_IsDisposable()
        {
            var sourceMonitor = new Mock<ISourceMonitor>();
            var viewController = new Mock<IViewController>();
            var viewGenerator = new Mock<IViewGenerator>(MockBehavior.Loose);
            var runner = new Runner(sourceMonitor.Object, viewController.Object, viewGenerator.Object);

            Assert.IsTrue(runner is IDisposable);
        }

        [TestMethod]
        public void Runner_CallingDispose_SourceChangedDoesntTriggerDrawView()
        {
            var sourceMonitor = new Mock<ISourceMonitor>(MockBehavior.Loose);
            var viewController = new Mock<IViewController>(MockBehavior.Loose);
            var solution = "solution.sln";
            var activeDocument = String.Empty;
            var content = "content";
            var viewGenerator = new Mock<IViewGenerator>(MockBehavior.Loose);
            viewGenerator
                .Setup(generator => generator.GenerateViewAsync(solution, activeDocument))
                .Returns(Task.FromResult(content));
            var runner = new Runner(sourceMonitor.Object, viewController.Object, viewGenerator.Object);

            runner.Dispose();

            sourceMonitor.Raise(mock => mock.SourceChanged += null, new SourceMonitorArgs(solution, activeDocument));
            viewController.Verify(mock => mock.Draw(content), Times.Never());
        }

        [TestMethod]
        public void Runner_CallingDisposeTwice_DoesntFail()
        {
            var sourceMonitor = new Mock<ISourceMonitor>();
            var viewController = new Mock<IViewController>();
            var viewGenerator = new Mock<IViewGenerator>(MockBehavior.Loose);
            var runner = new Runner(sourceMonitor.Object, viewController.Object, viewGenerator.Object);

            runner.Dispose();
            runner.Dispose();
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
            using (var sourceMonitor = new SourceMonitor(null))
            {
                Assert.IsTrue(sourceMonitor is IDisposable);
            }
        }

        [TestMethod]
        public void SourceMonitor_FromTimeToTime_TriggerSourceChanged()
        {
            using (var sourceMonitor = new SourceMonitor(null))
            {
                var changes = 0;
                sourceMonitor.SourceChanged += (s, e) => changes += 1;

                var arbitraryExpectedChanges = 3;
                while (changes < arbitraryExpectedChanges)
                {
                    Thread.Sleep(10);
                }
            }
        }

        [TestMethod]
        public void SourceMonitor_WhilePreviouseNotificationInProgress_ItWaits()
        {
            using (var sourceMonitor = new SourceMonitor(null))
            {
                var longHandlerEvent = new AutoResetEvent(false);
                var sourceChangedEvent = new AutoResetEvent(false);

                var changes = 0;
                sourceMonitor.SourceChanged += (s, e) =>
                {
                    // this is happening in a different thread
                    changes += 1;
                    sourceChangedEvent.Set();
                    longHandlerEvent.WaitOne();
                };

                sourceChangedEvent.WaitOne();
                var longTimeRequiredToProcessTheEvent = 3000;
                Thread.Sleep(longTimeRequiredToProcessTheEvent);
                longHandlerEvent.Set();

                Assert.AreEqual(1, changes);
            }
        }

        [TestMethod]
        public void SourceMonitor_AfterDisposing_DoesntTriggerSourceChanged()
        {
            using (var sourceMonitor = new SourceMonitor(null))
            {
                sourceMonitor.Dispose();

                var changes = 0;
                sourceMonitor.SourceChanged += (s, e) => changes += 1;
                var arbitraryWaitTime = 1500;
                Thread.Sleep(arbitraryWaitTime);
                Assert.AreEqual(0, changes);
            }
        }
    }
}
