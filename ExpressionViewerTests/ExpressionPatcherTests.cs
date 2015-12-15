using Extension;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSG = ExpressionViewerTests.TestSolutionGenerator;

namespace ExpressionViewerTests
{
    [TestClass]
    public class ExpressionPatcherTests
    {
        [TestMethod]
        public async Task ExpressionPatcher_ExpressionWithNoMissingDependency_ReturnsSameExpression()
        {
            var patcher = new ExpressionPatcher();
            
            var expressionText = "\"text\".ToString().ToString()";
            var file = @"
            public void Do() {
                var result = " + expressionText + @";
            }";
            var expression = await FindSource(
                solution: TSG.SingleFileSolution(file),
                activeDocument: TSG.DefaultActiveDocument(),
                cursorPosition: file.IndexOf(expressionText));
            var result = patcher.Patch(expression);

            Assert.AreEqual(expressionText, result.GetText().ToString());
        }


        private async Task<SyntaxNode> FindSource(Solution solution, string activeDocument, int cursorPosition)
        {
            var searcher = new ExpressionSearcher();
            var result = await searcher.FindSource(solution, activeDocument, cursorPosition);
            return result;
        }
    }
}
