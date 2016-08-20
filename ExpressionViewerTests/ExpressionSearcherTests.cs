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

            var result = await searcher.FindSource(
                solution: null,
                activeDocument: SingleFileSolution.ActiveDocument(),
                cursorPosition: DefaultCursorPosition());

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task FindSource_WithEmptySolution_ReturnsNoValue()
        {
            var searcher = new ExpressionSearcher();

            var result = await searcher.FindSource(
                solution: EmptySolution(),
                activeDocument: SingleFileSolution.ActiveDocument(),
                cursorPosition: DefaultCursorPosition());

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task FindSource_WithInvalidActiveDocument_ReturnsNoValue()
        {
            var searcher = new ExpressionSearcher();

            var result = await searcher.FindSource(
                solution: DefaultSolution(),
                activeDocument: InvalidActiveDocument(),
                cursorPosition: DefaultCursorPosition());

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task FindSource_WithWrongFileName_ReturnsNoValue()
        {
            var searcher = new ExpressionSearcher();

            var result = await searcher.FindSource(
                solution: DefaultSolution(),
                activeDocument: WrongActiveDocument(),
                cursorPosition: DefaultCursorPosition());

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task FindSource_WithInvalidCursorPosition_ReturnsNoValue()
        {
            var searcher = new ExpressionSearcher();

            var result = await searcher.FindSource(
                solution: DefaultSolution(),
                activeDocument: SingleFileSolution.ActiveDocument(),
                cursorPosition: InvalidCursorPosition());

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task FindSource_LookingForExpression_ReturnsTheExpression()
        {
            var searcher = new ExpressionSearcher();

            var expression = "\"text\".ToString().ToString()";
            var file = @"
            public void Do() {
                var result = " + expression + @";
            }";
            var activeDocument = SingleFileSolution.ActiveDocument();
            var result = await searcher.FindSource(
                solution: SingleFileSolution.New(file),
                activeDocument: SingleFileSolution.ActiveDocument(),
                cursorPosition: file.IndexOf(expression));

            Assert.AreEqual(expression, result.GetText().ToString());
        }

        [TestMethod]
        public async Task FindSource_LookingForStringConstant_ReturnsTheExpression()
        {
            var searcher = new ExpressionSearcher();

            var constant = "\"text\"";
            var file = @"
            public void Do() {
                var result = " + constant + @";
            }";
            var activeDocument = SingleFileSolution.ActiveDocument();
            var result = await searcher.FindSource(
                solution: SingleFileSolution.New(file),
                activeDocument: SingleFileSolution.ActiveDocument(),
                cursorPosition: file.IndexOf(constant));

            Assert.AreEqual(constant, result.GetText().ToString());
        }

        [TestMethod]
        public async Task FindSource_WithBadCursorPosition_ReturnsNoValue()
        {
            var searcher = new ExpressionSearcher();

            var expression = "\"text\".ToString().ToString()";
            var file = @"
            public void Do() {
                var result = " + expression + @";
            }";
            var activeDocument = SingleFileSolution.ActiveDocument();
            var result = await searcher.FindSource(
                solution: SingleFileSolution.New(file),
                activeDocument: SingleFileSolution.ActiveDocument(),
                cursorPosition: file.IndexOf(expression) - 1);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task FindSource_MethodWithMultipleExpressions_ReturnsTheRequestedExpression()
        {
            var expressions = new[]
            {
                "\"text1\".ToString().ToString()",
                "\"text2\".ToString().ToString()",
                "\"text3\".ToString().ToString()"
            };
            var file = @"
            public string Do()
            {
                var result1 = " + expressions[0] + @";
                var result2 = " + expressions[1] + @";
                return " + expressions[2] + @";
            }";

            foreach (var expression in expressions)
            {
                var searcher = new ExpressionSearcher();

                var result = await searcher.FindSource(
                    solution: SingleFileSolution.New(file),
                    activeDocument: SingleFileSolution.ActiveDocument(),
                    cursorPosition: file.IndexOf(expression));

                Assert.AreEqual(expression, result.GetText().ToString());
            }
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

            var solution = SingleFileSolution.New(@"
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
            var solution = SingleFileSolution.New(@"
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
            var solution = SingleFileSolution.New(@"
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

        [TestMethod]
        public void SameFile_FileNameVsSameFileName_ReturnsTrue()
        {
            var result = FileNameComparer.SameFile(
                fileName1: "file.txt",
                fileName2: "file.txt");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SameFile_FileNameVsDifferentFileName_ReturnsFalse()
        {
            var result = FileNameComparer.SameFile(
                fileName1: "file1.txt",
                fileName2: "file2.txt");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void SameFile_FullFileNameVsSameFileName_ReturnsTrue()
        {
            var result = FileNameComparer.SameFile(
                fileName1: "d:\\folder\\file.txt",
                fileName2: "file.txt");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SameFile_FullFileNameVsDifferentFileName_ReturnsFalse()
        {
            var result = FileNameComparer.SameFile(
                fileName1: "d:\\folder\\file1.txt",
                fileName2: "file2.txt");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void SameFile_FullFileNameVsSameFullFileName_ReturnsTrue()
        {
            var result = FileNameComparer.SameFile(
                fileName1: "d:\\folder\\file.txt",
                fileName2: "d:\\folder\\file.txt");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SameFile_FullFileNameVsDifferentFullFileName_ReturnsFalse()
        {
            var result = FileNameComparer.SameFile(
                fileName1: "d:\\folder\\file1.txt",
                fileName2: "d:\\folder\\file2.txt");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void SameFile_FileNameVsSameFullFileName_ReturnsTrue()
        {
            var result = FileNameComparer.SameFile(
                fileName1: "file.txt",
                fileName2: "d:\\folder\\file.txt");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SameFile_FileNameVsDifferentFullFileName_ReturnsFalse()
        {
            var result = FileNameComparer.SameFile(
                fileName1: "file1.txt",
                fileName2: "d:\\folder\\file2.txt");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void SameFile_FileNameVsSameFileNameDifferentCase_ReturnsTrue()
        {
            var result = FileNameComparer.SameFile(
                fileName1: "file.txt",
                fileName2: "FiLe.TxT");

            Assert.IsTrue(result);
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

        private Solution DefaultSolution()
        {
            var defaultContent = @"
            namespace SimpleNamespace {
                public class SimpleClass() {
                    public void Do() {
                        var x = ""text"".ToString().ToString();
                    }
                }
            }";
            var defaultSolution = SingleFileSolution.New(defaultContent);

            return defaultSolution;
        }

        private static IEnumerable<ClassDeclarationSyntax> GetClassDeclarationSyntaxes(Compilation compilation)
        {
            return compilation.SyntaxTrees
                .Select(syntaxTree => syntaxTree.GetRoot())
                .SelectMany(root => root.DescendantNodesAndSelf())
                .OfType<ClassDeclarationSyntax>();
        }

        private string InvalidActiveDocument()
        {
            return String.Empty;
        }

        private string WrongActiveDocument()
        {
            return "noSuchFileAsThis.cs";
        }

        private int InvalidCursorPosition()
        {
            return -1;
        }
        
        private int DefaultCursorPosition()
        {
            return 0;
        }
    }
}
