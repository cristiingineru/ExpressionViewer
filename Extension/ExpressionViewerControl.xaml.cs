//------------------------------------------------------------------------------
// <copyright file="ExpressionViewerControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Extension
{
    using Microsoft.CodeAnalysis.MSBuild;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.CSharp;


    /// <summary>
    /// Interaction logic for ExpressionViewerControl.
    /// </summary>
    public partial class ExpressionViewerControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionViewerControl"/> class.
        /// </summary>
        public ExpressionViewerControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            using (var tempDirectory = new TempDirectory())
            {
                var workspace2 = MSBuildWorkspace.Create();
                //var task2 = workspace2.OpenSolutionAsync(@"..\..\..\ManualTesting\ManualTesting.sln");
                var task2 = workspace2.OpenSolutionAsync(@"D:\Projects\ExpressionViewer\ManualTesting\ManualTesting.sln");
                task2.Wait();
                var solution2 = task2.Result;
                var newSolution2 = solution2.GetIsolatedSolution();

                var expressionSearcher = new ExpressionSearcher();
                var expressionBuilder = new ExpressionBuilder();
                var sourceTask = expressionSearcher.FindSource(newSolution2);
                sourceTask.Wait();
                var source = sourceTask.Result;
                var targetTask2 = expressionSearcher.FindTarget(newSolution2);
                targetTask2.Wait();
                var target2 = targetTask2.Result;
                var instrumentationTask = expressionBuilder.BuildWrapper(source);
                instrumentationTask.Wait();
                var instrumentation = instrumentationTask.Result;

                var newCompilation = expressionSearcher.ReplaceNodeInCompilation(target2.Compilation, target2.Node, instrumentation);

                var instrumentedDll = Path.Combine(tempDirectory.FullName, Path.GetRandomFileName() + ".dll");
                var emitResult = newCompilation.Emit(instrumentedDll);
                var assemblyRunner = new AssemblyRunner();
                var result = assemblyRunner.Run(instrumentedDll, "WrapperClass", "WrapperMethod");

                //var root2 = target2.Compilation.SyntaxTrees
            }
        }

        private SyntaxNode FindDestinationParent(SyntaxNode root)
        {
            var target = root
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>().First();

            return target;
        }

        private SyntaxNode FindNodeToBeCopied(SyntaxNode root)
        {
            var target = root
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>().First()
                .ChildNodes()
                .OfType<BlockSyntax>().First();

            return target;
        }

        private SyntaxNode GenerateBranchToBeInserted(SyntaxNode source)
        {
            var newClass =
                SyntaxFactory.ClassDeclaration("MyClass")
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        SyntaxFactory.Token(
                            SyntaxKind.PublicKeyword)))
                .AddMembers(
                    SyntaxFactory.MethodDeclaration(
                        SyntaxFactory.PredefinedType(
                            SyntaxFactory.Token(
                                SyntaxKind.VoidKeyword)),
                        SyntaxFactory.Identifier(
                            @"MyMethod"))
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        SyntaxFactory.Token(
                            SyntaxKind.PublicKeyword),
                        SyntaxFactory.Token(
                            SyntaxKind.StaticKeyword)))
                .WithParameterList(
                    SyntaxFactory.ParameterList()
                    .WithOpenParenToken(
                        SyntaxFactory.Token(
                            SyntaxKind.OpenParenToken))
                    .WithCloseParenToken(
                        SyntaxFactory.Token(
                            SyntaxKind.CloseParenToken)))
                .WithBody(
                    SyntaxFactory.Block(
                        source as StatementSyntax)
                    .WithOpenBraceToken(
                        SyntaxFactory.Token(
                            SyntaxKind.OpenBraceToken))
                    .WithCloseBraceToken(
                        SyntaxFactory.Token(
                            SyntaxKind.CloseBraceToken))));

            return newClass;
        }
    }
}