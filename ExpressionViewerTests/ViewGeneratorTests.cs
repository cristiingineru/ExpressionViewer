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

            var solutionPath = @"drive:\folder\solution.sln";
            var view = await generator.GenerateViewAsync(solutionPath);

            Assert.IsFalse(IsValidView(view));
        }

        [TestMethod]
        public async Task ViewGenerator_WithValidSolution_ReturnsView()
        {
            var generator = new ViewGenerator();

            var solutionPath = ValidSolutionPath();
            var view = await generator.GenerateViewAsync(solutionPath);

            Assert.IsTrue(IsValidView(view));
        }

        [TestMethod]
        public async Task ViewGenerator_WithSolutionWithCompileError_DoesntReturnView()
        {
            var generator = new ViewGenerator();

            var solutionPath = SolutionWithCompileErrorPath();
            var view = await generator.GenerateViewAsync(solutionPath);

            Assert.IsFalse(IsValidView(view));
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

        private string ValidSolutionPath()
        {
            return @"..\..\..\TestSolutions\ValidSolution\ValidSolution.sln";
        }

        private string SolutionWithCompileErrorPath()
        {
            return @"..\..\..\TestSolutions\SolutionWithCompileError\SolutionWithCompileError.sln";
        }
    }
}
