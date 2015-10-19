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
            var workspace = MSBuildWorkspace.Create();
            var task = workspace.OpenSolutionAsync(@"C:\Users\Cristi\Documents\Visual Studio 2015\Projects\MiniProject\MiniProject.sln");
            task.Wait();
            var solution = task.Result;
            var projects = solution.Projects;

            var project0 = projects.ElementAt(0);
            var newCompilationOptions = projects.First().CompilationOptions.WithOptimizationLevel(OptimizationLevel.Debug);

            solution = solution.WithProjectCompilationOptions(project0.Id, newCompilationOptions);

            //var ar = new AnalyzerReference();
            //solution.AddAnalyzerReference(project0.Id, ar);
            var newSolution = solution.GetIsolatedSolution();

            var assemblies = new List<Stream>();
            foreach (var projectId in solution.GetProjectDependencyGraph().GetTopologicallySortedProjects())
            {
                var projectName = solution.GetProject(projectId).Name;
                //using (var stream = new MemoryStream())
                using (var stream = new FileStream(projectName + ".dll", FileMode.Create))
                {
                    var t2 = solution.GetProject(projectId).GetCompilationAsync();
                    t2.Wait();
                    var compilationUnit = t2.Result;
                    var syntaxTrees = compilationUnit.SyntaxTrees;
                    var syntaxTree = syntaxTrees.ElementAt(0);
                    var root = syntaxTree.GetRoot();
                    var options = syntaxTree.Options;
                    var newOptions = options;

                    //var target = FindInsertParent(root);
                    //var newRoot = target.InsertNodesAfter(null, new[] { GenerateNode() });

                    var target = FindDestinationParent(root);
                    var source = FindNodeToBeCopied(root);
                    var newTarget = target
                        .InsertNodesBefore(target.ChildNodes().Last(), new[] { GenerateBranchToBeInserted(source) });

                    var newRoot = root.ReplaceNode(target, newTarget);

                    var newSyntaxTree = syntaxTree.WithRootAndOptions(newRoot, newOptions);
                    var newCompilationUnit = compilationUnit.ReplaceSyntaxTree(syntaxTree, newSyntaxTree);
                    var emitResult = newCompilationUnit.Emit(stream);
                    assemblies.Add(stream);
                }
            }

            var dll = Assembly.LoadFile(Path.Combine(Directory.GetCurrentDirectory(), "MiniProject.dll"));
            var myClass = dll.GetExportedTypes()
                .Where(type => type.Name.Contains("MyClass"))
                .First();
            dynamic myObject = System.Activator.CreateInstance(myClass);
            myClass.GetMethod("MyMethod", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);

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