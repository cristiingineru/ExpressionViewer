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
    public class ViewGeneratorTests
    {
        [TestMethod]
        public async Task ViewGenerator_WithInvalidSolutionPath_DoesntReturnView()
        {
            var generator = new ViewGenerator();

            var view = await generator.GenerateViewAsync(
                solutionPath: InvalidSolutionPath());

            Assert.IsFalse(IsValidView(view));
        }

        [TestMethod]
        public async Task ViewGenerator_WithSolutionWithCompileError_DoesntReturnValidView()
        {
            var generator = new ViewGenerator();

            var view = await generator.GenerateViewAsync(
                solutionPath: SolutionWithCompileErrorPath());

            Assert.IsFalse(IsValidView(view));
        }

        [TestMethod]
        public async Task ViewGenerator_WithSolutionWithCompileError_ReturnsEmitError()
        {
            var generator = new ViewGenerator();

            var view = await generator.GenerateViewAsync(
                solutionPath: SolutionWithCompileErrorPath());

            Assert.IsTrue(view.Contains("error CS1002"));
        }

        [TestMethod]
        public async Task ViewGenerator_WithClassLibrarySolution_ReturnsView()
        {
            var generator = new ViewGenerator();

            var view = await generator.GenerateViewAsync(
                solutionPath: ClassLibrarySolutionPath(),
                activeDocument: DefaultDocument(),
                cursorPosition: ReturnExpressionPosition());

            Assert.IsTrue(IsValidView(view));
        }

        [TestMethod]
        public async Task ViewGenerator_WithConsoleApplicationSolution_ReturnsView()
        {
            var generator = new ViewGenerator();

            var view = await generator.GenerateViewAsync(
                solutionPath: ConsoleApplicationSolutionPath(),
                activeDocument: DefaultDocument(),
                cursorPosition: ReturnExpressionPosition());

            Assert.IsTrue(IsValidView(view));
        }

        [TestMethod]
        public async Task ViewGenerator_OfReturnExpression_ReturnsView()
        {
            var generator = new ViewGenerator();

            var view = await generator.GenerateViewAsync(
                solutionPath: ClassLibrarySolutionPath(),
                activeDocument: DefaultDocument(),
                cursorPosition: ReturnExpressionPosition());

            Assert.IsTrue(IsValidView(view));
        }

        [TestMethod]
        public async Task ViewGenerator_OfVarExpression_ReturnsView()
        {
            var generator = new ViewGenerator();

            var view = await generator.GenerateViewAsync(
                solutionPath: ClassLibrarySolutionPath(),
                activeDocument: DefaultDocument(),
                cursorPosition: VarExpressionPosition());

            Assert.IsTrue(IsValidView(view));
        }

        /// <summary>
        /// This message needs to be present in any valid view returned by the generator.
        /// </summary>
        private const string ItsWorkingWelcomeMessage = "Hello";

        private bool IsValidView(string view)
        {
            view = view ?? string.Empty;
            return view.Contains(ItsWorkingWelcomeMessage);
        }

        private string DefaultDocument()
        {
            return "Program.cs";
        }

        private int ReturnExpressionPosition()
        {
            return 350;
        }

        private int VarExpressionPosition()
        {
            return 500;
        }

        private string InvalidSolutionPath()
        {
            return @"drive:\folder\solution.sln";
        }

        private string SolutionWithCompileErrorPath()
        {
            return @"..\..\..\TestSolutions\SolutionWithCompileError\SolutionWithCompileError.sln";
        }

        private string ClassLibrarySolutionPath()
        {
            return @"..\..\..\TestSolutions\ClassLibrarySolution\ClassLibrarySolution.sln";
        }

        private string ConsoleApplicationSolutionPath()
        {
            return @"..\..\..\TestSolutions\ConsoleApplicationSolution\ConsoleApplicationSolution.sln";
        }
    }
}
