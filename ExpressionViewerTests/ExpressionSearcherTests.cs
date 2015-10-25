using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Extension;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Threading.Tasks;

namespace ExpressionViewerTests
{
    [TestClass]
    public class ExpressionSearcherTests
    {
        [TestMethod]
        public async Task FindSource_WithNullSolution_ReturnsNoValue()
        {
            var searcher = new ExpressionSearcher();

            var result = await searcher.FindSource(null);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task FindSource_WithEmptySolution_ReturnsNoValue()
        {
            var searcher = new ExpressionSearcher();

            var solution = EmptySolution();
            var result = await searcher.FindSource(solution);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task FindSource_WithExpressionWithinMethod_ReturnsTheExpression()
        {
            var searcher = new ExpressionSearcher();

            var expression = "\"text\".ToString().ToString()";
            var solution = SingleFileSolution(@"public void Do() { var result = " + expression + "; }");
            var result = await searcher.FindSource(solution);

            Assert.AreEqual(expression, result.GetText().ToString());
        }

        [TestMethod]
        public async Task FindTarget_WithNullSolution_ReturnsNoValue()
        {
            var searcher = new ExpressionSearcher();

            var result = await searcher.FindTarget(null);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task FindTarget_WithEmptySolution_ReturnsNoValue()
        {
            var searcher = new ExpressionSearcher();

            var solution = EmptySolution();
            var result = await searcher.FindTarget(solution);

            Assert.IsNull(result);
        }


        private Solution EmptySolution()
        {
            var id = SolutionId.CreateNewId();
            var version = new VersionStamp();
            var solutionInfo = SolutionInfo.Create(id, version);
            var workspace = new AdhocWorkspace();
            var solution = workspace.AddSolution(solutionInfo);

            return solution;
        }

        private Solution SingleFileSolution(string fileContent)
        {
            var id = SolutionId.CreateNewId();
            var version = new VersionStamp();
            var solutionInfo = SolutionInfo.Create(id, version);
            var workspace = new AdhocWorkspace();
            var solution = workspace.AddSolution(solutionInfo);

            var projectId = ProjectId.CreateNewId();
            var projectInfo = NewProjectInfo(projectId);
            solution = solution.AddProject(projectInfo);
            var project = solution.GetProject(projectId);

            var root = CSharpSyntaxTree.ParseText(fileContent).GetRoot();
            var document = project.AddDocument("file.cs", root);

            return document.Project.Solution;
        }

        private ProjectInfo NewProjectInfo(ProjectId projectId)
        {
            var version = VersionStamp.Create();
            var projectInfo = ProjectInfo.Create(projectId, version, "no name", "assembly.dll", "C#");

            return projectInfo;
        }
    }
}
