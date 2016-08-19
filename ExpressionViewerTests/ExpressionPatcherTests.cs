using Extension;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionViewerTests
{
    [TestClass]
    public class ExpressionPatcherTests
    {
        [TestMethod]
        public void GetOutOfScopeVariables_WithNullExpression_ReturnsEmptyEnumerable()
        {
            var patcher = new ExpressionPatcher();

            var variables = patcher.GetOutOfScopeVariables(null, null);

            Assert.IsFalse(variables.Any());
        }

        [TestMethod]
        public async Task GetOutOfScopeVariables_WithoutOutOfScopeVariables_ReturnsEmptyEnumerable()
        {
            var patcher = new ExpressionPatcher();

            var searcher = new ExpressionSearcher();
            var expression = "\"text\".ToString().ToString()";
            var file = @"
            public void Do() {
                var result = " + expression + @";
            }";
            var activeDocument = ExpressionSearcherTests.DefaultActiveDocument();
            var source = await searcher.FindSource(
                solution: ExpressionSearcherTests.SingleFileSolution(file),
                activeDocument: ExpressionSearcherTests.DefaultActiveDocument(),
                cursorPosition: file.IndexOf(expression));

            var variables = patcher.GetOutOfScopeVariables(source, null);

            Assert.IsFalse(variables.Any());
        }

        [TestMethod]
        public async Task GetOutOfScopeVariables_WithOneOutOfScopeVariable_ReturnsThatVariable()
        {
            var patcher = new ExpressionPatcher();

            var searcher = new ExpressionSearcher();
            var expression = "\"text\".ToString().Insert(0, variable).ToString()";
            var file = @"
            public void Do(string variable) {
                var result = " + expression + @";
            }";
            var activeDocument = ExpressionSearcherTests.DefaultActiveDocument();
            var solution = ExpressionSearcherTests.SingleFileSolution(file);
            var source = await searcher.FindSource(
                solution: solution,
                activeDocument: ExpressionSearcherTests.DefaultActiveDocument(),
                cursorPosition: file.IndexOf(expression));
            var target = await searcher.FindTarget(solution);
            var compilation = await CompilationFromSingleProjectSolution(solution);

            var variables = patcher.GetOutOfScopeVariables(source, compilation);

            Assert.AreEqual(1, variables.Count());
        }

        public static async Task<Compilation> CompilationFromSingleProjectSolution(Solution solution)
        {
            var project = await solution.Projects.First().GetCompilationAsync();
            return project;
        }
    }
}
