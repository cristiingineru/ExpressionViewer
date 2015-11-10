using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Extension;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Collections.Generic;

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
            var solution = SingleFileSolution(@"
            public void Do() {
                var result = " + expression + @";
            }");
            var result = await searcher.FindSource(solution);

            Assert.AreEqual(expression, result.GetText().ToString());
        }

        [TestMethod]
        public async Task FindTarget_WithNullSolution_ReturnsNoValue()
        {
            var searcher = new ExpressionSearcher();

            var result = await searcher.FindTarget(null);

            Assert.IsNull(result.Node);
            Assert.IsNull(result.Compilation);
        }

        [TestMethod]
        public async Task FindTarget_WithEmptySolution_ReturnsNoValue()
        {
            var searcher = new ExpressionSearcher();

            var solution = EmptySolution();
            var result = await searcher.FindTarget(solution);

            Assert.IsNull(result.Node);
            Assert.IsNull(result.Compilation);
        }

        [TestMethod]
        public async Task FindTarget_WithNamespaceInFileSolution_ReturnsTarget()
        {
            var searcher = new ExpressionSearcher();

            var solution = SingleFileSolution(@"
            namespace SimpleNamespace {
                public class SimpleClass() {
                    public void Do() {
                    }
                }
            }");
            var result = await searcher.FindTarget(solution);

            Assert.IsTrue(result.Node is ClassDeclarationSyntax);
            Assert.IsNotNull(result.Compilation);
        }


        [TestMethod]
        public async Task InsertNodeInCompilation_WithOldAndNewNodes_ReturnsNewUpdatedCompilation()
        {
            var searcher = new ExpressionSearcher();
            var solution = SingleFileSolution(@"
            namespace SimpleNamespace {
                public class SimpleClass() {
                    public void Do() {
                    }
                }
            }");
            var target = await searcher.FindTarget(solution);
            SyntaxNode newNode = SyntaxFactory.ClassDeclaration("NewNode");

            var newCompilation = searcher.InsertNodeInCompilation(target.Compilation, target.Node, newNode);

            Assert.AreNotEqual(target.Compilation, newCompilation);
        }

        [TestMethod]
        public async Task InsertNodeInCompilation_WithOldAndNewNodes_LeavesOldClassInPlace()
        {
            var searcher = new ExpressionSearcher();
            var oldClassName = "SimpleClass";
            var solution = SingleFileSolution(@"
            namespace " + oldClassName + @" {
                public class SimpleClass() {
                    public void Do() {
                    }
                }
            }");
            var target = await searcher.FindTarget(solution);
            SyntaxNode newNode = SyntaxFactory.ClassDeclaration("NewNode");

            var newCompilation = searcher.InsertNodeInCompilation(target.Compilation, target.Node, newNode);

            var classes = GetClassDeclarationSyntaxes(newCompilation);
            Assert.AreEqual(1, classes.Count(classSyntax => classSyntax.Identifier.Text == oldClassName));
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

        private static IEnumerable<ClassDeclarationSyntax> GetClassDeclarationSyntaxes(Compilation compilation)
        {
            return compilation.SyntaxTrees
                .Select(syntaxTree => syntaxTree.GetRoot())
                .SelectMany(root => root.DescendantNodesAndSelf())
                .OfType<ClassDeclarationSyntax>();
        }
    }
}
