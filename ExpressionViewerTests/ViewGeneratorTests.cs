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
        public async Task ViewGenerator_WithSavedSolution_ReturnsView()
        {
            var generator = new ViewGenerator();

            var solutionPath = SavedSolutionPath();
            var view = await generator.GenerateViewAsync(solutionPath);

            Assert.IsTrue(IsValidView(view));
        }

        /// <summary>
        /// This message needs to be present in any view returned by the generator.
        /// </summary>
        private const string ItsWorkingWelcomeMessage = "Hello!!!";

        private bool IsValidView(string view)
        {
            view = view ?? string.Empty;
            return view.Contains(ItsWorkingWelcomeMessage);
        }

        private string SavedSolutionPath()
        {
            return @"..\..\..\ManualTesting\ManualTesting.sln";
        }
    }
}
