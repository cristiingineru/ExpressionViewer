using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension
{
    public class ExpressionSearcher
    {
        public async Task<SyntaxNode> FindSource(Solution solution)
        {
            if (solution == null)
            {
                return null;
            }

            var compilationTasks = solution.GetProjectDependencyGraph().GetTopologicallySortedProjects()
                .Select(projectId =>
                {
                    var project = solution.GetProject(projectId);
                    var compilation = project.GetCompilationAsync();
                    return compilation;
                });

            foreach(var task in compilationTasks)
            {
                task.Wait();
            }

            var invocationExpressions = compilationTasks
                .Select(task => task.Result)
                .SelectMany(compilation => compilation.SyntaxTrees)
                .Select(syntaxTree => syntaxTree.GetRoot())
                .SelectMany(root => root.DescendantNodesAndSelf())
                .Where(syntaxNode => syntaxNode.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.MethodDeclaration))
                .SelectMany(methodDeclaration => methodDeclaration.DescendantNodesAndSelf())
                .Where(syntaxNode => syntaxNode.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.InvocationExpression));
            
            SyntaxNode foundInvocationExpression = null;
            if (invocationExpressions.Any())
            {
                foundInvocationExpression = invocationExpressions.First();
            }
            return foundInvocationExpression;
        }

        public struct Target
        {
            public Compilation Compilation;
            public SyntaxNode Node;
        }

        public async Task<Target> FindTarget(Solution solution)
        {
            if (solution == null)
            {
                return new Target();
            }

            var classDeclarationSyntaxes = solution.GetProjectDependencyGraph().GetTopologicallySortedProjects()
                .Select(async projectId =>
                {
                    var project = solution.GetProject(projectId);
                    var compilation = (await project.GetCompilationAsync());
                    return compilation;
                })
                .Select(compilationTask => compilationTask.Result)
                .Where(compilation => GetClassDeclarationSyntaxes(compilation).Any())
                .Select(compilation => new Target() { Compilation = compilation, Node = GetClassDeclarationSyntaxes(compilation).First() } );

            var foundNamespaceDeclarationSyntax = new Target();
            if (classDeclarationSyntaxes.Any())
            {
                foundNamespaceDeclarationSyntax = classDeclarationSyntaxes.First();
            }
            return foundNamespaceDeclarationSyntax;
        }

        private static IEnumerable<ClassDeclarationSyntax> GetClassDeclarationSyntaxes(Compilation compilation)
        {
            return compilation.SyntaxTrees
                .Select(syntaxTree => syntaxTree.GetRoot())
                .SelectMany(root => root.DescendantNodesAndSelf())
                .OfType<ClassDeclarationSyntax>();
        }

        public Compilation InsertNodeInCompilation(Compilation compilation, SyntaxNode node, SyntaxNode newNode)
        {
            var syntaxTree = node.SyntaxTree;
            var root = syntaxTree.GetRoot();

            var newRoot = root.InsertNodesBefore(node, new[] { newNode });
            var newSyntaxTree = syntaxTree.WithRootAndOptions(newRoot, syntaxTree.Options);
            var newCompilation = compilation.ReplaceSyntaxTree(syntaxTree, newSyntaxTree);

            return newCompilation;
        }
    }
}
