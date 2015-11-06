using Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var viewDrawer = new Mock<IViewDrawer>(MockBehavior.Loose);
            var runner = new Runner(sourceMonitor.Object, viewDrawer.Object);

            sourceMonitor.Raise(mock => mock.SourceChanged += null, new EventArgs());

            viewDrawer.Verify(mock => mock.Draw(), Times.Once());
        }
    }
}
