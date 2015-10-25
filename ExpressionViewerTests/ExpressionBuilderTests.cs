using Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionViewerTests
{
    [TestClass]
    public class ExpressionBuilderTests
    {
        [TestMethod]
        public async Task BuildWrapper_WithNullInnerExpression_ReturnsWrapper()
        {
            var builder = new ExpressionBuilder();

            var result = await builder.BuildWrapper(null);

            Assert.IsNotNull(result);
        }
    }
}
