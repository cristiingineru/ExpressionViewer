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
        public async Task ExpressionSearcher_WithNullSolution_ReturnsNoValue()
        {
            var searcher = new ExpressionSearcher();

            var result = await searcher.FindTarget(null);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task ExpressionSearcher_WithEmptySolution_ReturnsNoValue()
        {
            var searcher = new ExpressionSearcher();

            var solution = EmptySolution();
            var result = await searcher.FindTarget(solution);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task ExpressionSearcher_WithSolutionWithSingleMethod_ReturnsTheMethod()
        {
            var searcher = new ExpressionSearcher();

            var solution = SolutionWithSingleMethod();
            var result = await searcher.FindTarget(solution);

            Assert.IsNotNull(result);
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

        private Solution SolutionWithSingleMethod()
        {
            var id = SolutionId.CreateNewId();
            var version = new VersionStamp();
            var solutionInfo = SolutionInfo.Create(id, version);
            var workspace = new AdhocWorkspace();
            var solution = workspace.AddSolution(solutionInfo);

            var root = CSharpSyntaxTree.ParseText(@"
                public void Do()
                    {
                        var result = ""text""
                            .ToString()
                            .ToString();
                    }
                }").GetRoot();


            var projectId = ProjectId.CreateNewId();
            var projectInfo = NewProjectInfo(projectId);
            solution = solution.AddProject(projectInfo);
            var project = solution.GetProject(projectId);
            var document = project.AddDocument("file.cs", root);

            return document.Project.Solution;

            //return solution;
        }

        private ProjectInfo NewProjectInfo(ProjectId projectId)
        {
            //var projectId = ProjectId.CreateNewId();
            var version = VersionStamp.Create();
            var projectInfo = ProjectInfo.Create(projectId, version, "no name", "assembly.dll", "C#");

            return projectInfo;
        }
    }
}
