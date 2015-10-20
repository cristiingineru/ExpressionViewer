using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Extension;

namespace ExpressionViewerTests
{
    [TestClass]
    public class ExpressionSearcherTests
    {
        [TestMethod]
        public void ExpressionSearcher_Constructor()
        {
            var searcher = new ExpressionSearcher();
        }

        [TestMethod]
        public void ExpressionSearcher_WithNullSolution_ReturnsNoValue()
        {
            var searcher = new ExpressionSearcher();

            var result = searcher.FindTarget(null);

            Assert.IsFalse(result.HasValue);
        }
    }
}
