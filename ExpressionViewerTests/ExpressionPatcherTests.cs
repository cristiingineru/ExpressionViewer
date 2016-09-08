﻿using Extension;
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
        public async Task GetVariableDependencies_WithNullExpression_ReturnsEmptyEnumerable()
        {
            var patcher = new ExpressionPatcher();

            var file = "public void Do() {}";
            var solution = SingleFileSolution.New(file);
            var compilation = await CompilationOfSingleProjectSolution(solution);
            var variables = patcher.GetVariableDependencies(null, compilation);

            Assert.IsFalse(variables.Any());
        }

        [TestMethod]
        public async Task GetVariableDependencies_WithNullCompilation_ReturnsEmptyEnumerable()
        {
            var patcher = new ExpressionPatcher();

            var searcher = new ExpressionSearcher();
            var expression = "\"text\".ToString().ToString()";
            var file = @"
            public void Do() {
                var result = " + expression + @";
            }";
            var solution = SingleFileSolution.New(file);
            var source = await searcher.FindSource(
                solution: solution,
                activeDocument: SingleFileSolution.ActiveDocument(),
                cursorPosition: file.IndexOf(expression));
            var compilation = await CompilationOfSingleProjectSolution(solution);
            var variables = patcher.GetVariableDependencies(source, null);

            Assert.IsFalse(variables.Any());
        }

        [TestMethod]
        public async Task GetVariableDependencies_IndependentExpression_ReturnsEmptyEnumerable()
        {
            var patcher = new ExpressionPatcher();

            var searcher = new ExpressionSearcher();
            var expression = "\"text\".ToString().ToString()";
            var file = @"
            public void Do() {
                var result = " + expression + @";
            }";
            var activeDocument = SingleFileSolution.ActiveDocument();
            var solution = SingleFileSolution.New(file);
            var source = await searcher.FindSource(
                solution: solution,
                activeDocument: SingleFileSolution.ActiveDocument(),
                cursorPosition: file.IndexOf(expression));
            var compilation = await CompilationOfSingleProjectSolution(solution);

            var variables = patcher.GetVariableDependencies(source, compilation);

            Assert.IsFalse(variables.Any());
        }

        [TestMethod]
        public async Task GetVariableDependencies_ExpressionDependingOnArgument_ReturnsThatVariable()
        {
            var patcher = new ExpressionPatcher();

            var searcher = new ExpressionSearcher();
            var expression = "\"text\".ToString().Insert(0, variable).ToString()";
            var file = @"
            public void Do(string variable) {
                var result = " + expression + @";
            }";
            var activeDocument = SingleFileSolution.ActiveDocument();
            var solution = SingleFileSolution.New(file);
            var source = await searcher.FindSource(
                solution: solution,
                activeDocument: SingleFileSolution.ActiveDocument(),
                cursorPosition: file.IndexOf(expression));
            var target = await searcher.FindTarget(solution);
            var compilation = await CompilationOfSingleProjectSolution(solution);

            var variables = patcher.GetVariableDependencies(source, compilation);

            Assert.AreEqual(1, variables.Count());
        }

        [TestMethod]
        public async Task GetVariableDependencies_ExpressionDependingOnLocalVariable_ReturnsThatVariable()
        {
            var patcher = new ExpressionPatcher();

            var searcher = new ExpressionSearcher();
            var expression = "\"text\".ToString().Insert(0, variable).ToString()";
            var file = @"
            public void Do() {
                string variable = ""value"";
                var result = " + expression + @";
            }";
            var activeDocument = SingleFileSolution.ActiveDocument();
            var solution = SingleFileSolution.New(file);
            var source = await searcher.FindSource(
                solution: solution,
                activeDocument: SingleFileSolution.ActiveDocument(),
                cursorPosition: file.IndexOf(expression));
            var target = await searcher.FindTarget(solution);
            var compilation = await CompilationOfSingleProjectSolution(solution);

            var variables = patcher.GetVariableDependencies(source, compilation);

            Assert.AreEqual(1, variables.Count());
        }

        [TestMethod]
        public async Task GetVariableDependencies_ExpressionDependingOnMemberVariable_ReturnsThatVariable()
        {
            var patcher = new ExpressionPatcher();

            var searcher = new ExpressionSearcher();
            var expression = "\"text\".ToString().Insert(0, variable).ToString()";
            var file = @"
            public class SmallClass {
                public string variable = ""value"";
                public void Do() {
                    var result = " + expression + @";
                }
            }";
            var activeDocument = SingleFileSolution.ActiveDocument();
            var solution = SingleFileSolution.New(file);
            var source = await searcher.FindSource(
                solution: solution,
                activeDocument: SingleFileSolution.ActiveDocument(),
                cursorPosition: file.IndexOf(expression));
            var target = await searcher.FindTarget(solution);
            var compilation = await CompilationOfSingleProjectSolution(solution);

            var variables = patcher.GetVariableDependencies(source, compilation);

            Assert.AreEqual(1, variables.Count());
        }

        [TestMethod]
        public async Task GetVariableDependencies_ExpressionDependingOnMultipleVariableTypes_ReturnsThoseVariables()
        {
            var patcher = new ExpressionPatcher();

            var searcher = new ExpressionSearcher();
            var expression = "\"text\".ToString().Insert(0, variable1).Insert(0, variable2).ToString()";
            var file = @"
            public class SmallClass {
                public string variable1 = ""value"";
                public void Do() {
                    string variable2 = ""value"";
                    var result = " + expression + @";
                }
            }";
            var activeDocument = SingleFileSolution.ActiveDocument();
            var solution = SingleFileSolution.New(file);
            var source = await searcher.FindSource(
                solution: solution,
                activeDocument: SingleFileSolution.ActiveDocument(),
                cursorPosition: file.IndexOf(expression));
            var target = await searcher.FindTarget(solution);
            var compilation = await CompilationOfSingleProjectSolution(solution);

            var variables = patcher.GetVariableDependencies(source, compilation);

            Assert.AreEqual(2, variables.Count());
        }

        [TestMethod]
        public async Task GetVariableDependencies_ExpressionDependingOnConstants_ReturnsEmptyEnumerable()
        {
            var patcher = new ExpressionPatcher();

            var searcher = new ExpressionSearcher();
            var expression = "\"text\".ToString().Insert(0, constant1).Insert(0, constant2).ToString()";
            var file = @"
            public class SmallClass {
                public const string constant1 = ""value"";
                public void Do() {
                    string const constant2 = ""value"";
                    var result = " + expression + @";
                }
            }";
            var activeDocument = SingleFileSolution.ActiveDocument();
            var solution = SingleFileSolution.New(file);
            var source = await searcher.FindSource(
                solution: solution,
                activeDocument: SingleFileSolution.ActiveDocument(),
                cursorPosition: file.IndexOf(expression));
            var target = await searcher.FindTarget(solution);
            var compilation = await CompilationOfSingleProjectSolution(solution);

            var variables = patcher.GetVariableDependencies(source, compilation);

            Assert.IsFalse(variables.Any());
        }

        [TestMethod]
        public async Task ReplaceVariablesWithConstants_IndependentExpression_ReturnsSameExpression()
        {
            var patcher = new ExpressionPatcher();

            var searcher = new ExpressionSearcher();
            var expression = "\"text\".ToString().ToString()";
            var file = @"
            public void Do() {
                var result = " + expression + @";
            }";
            var activeDocument = SingleFileSolution.ActiveDocument();
            var solution = SingleFileSolution.New(file);
            var source = await searcher.FindSource(
                solution: solution,
                activeDocument: SingleFileSolution.ActiveDocument(),
                cursorPosition: file.IndexOf(expression));
            var compilation = await CompilationOfSingleProjectSolution(solution);
            var variables = patcher.GetVariableDependencies(source, compilation);

            var newSource = patcher.ReplaceVariablesWithConstants(source, variables);

            Assert.AreEqual(source, newSource);
        }

        [TestMethod]
        public async Task ReplaceVariablesWithConstants_ExpressionDependingOnArgument_ReturnsIndependentExpression()
        {
            var patcher = new ExpressionPatcher();

            var searcher = new ExpressionSearcher();
            var expression = "\"text\".ToString().Insert(0, variable).ToString()";
            var file = @"
            using System;
            namespace SimpleNamespace {
                public class SimpleClass() {
                    public void Main(string variable) {
                        var result = " + expression + @";
                    }
                }
            }";
            var activeDocument = SingleFileSolution.ActiveDocument();
            var solution = SingleFileSolution.New(file);
            var source = await searcher.FindSource(
                solution: solution,
                activeDocument: SingleFileSolution.ActiveDocument(),
                cursorPosition: file.IndexOf(expression));
            var target = await searcher.FindTarget(solution);
            var compilation = await CompilationOfSingleProjectSolution(solution);
            var variables = patcher.GetVariableDependencies(source, compilation);

            var newSource = patcher.ReplaceVariablesWithConstants(source, variables);

            var expressionBuilder = new ExpressionBuilder();
            var instrumentation = await expressionBuilder.BuildWrapper(newSource);
            var newCompilation = searcher.InsertNodeInCompilation(target.Compilation, target.Node, instrumentation);
            var errors = target.Compilation.GetDiagnostics();
            Assert.IsFalse(errors.Any());
        }

        public static async Task<Compilation> CompilationOfSingleProjectSolution(Solution solution)
        {
            var project = await solution.Projects.First().GetCompilationAsync();
            return project;
        }
    }
}
