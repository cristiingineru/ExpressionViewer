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

            var namespaceDeclarationSyntaxes = solution.GetProjectDependencyGraph().GetTopologicallySortedProjects()
                .Select(async projectId =>
                {
                    var project = solution.GetProject(projectId);
                    var compilation = (await project.GetCompilationAsync());
                    return compilation;
                })
                .Select(compilationTask => compilationTask.Result)
                .Where(compilation => GetNamespaceDeclarationSyntaxes(compilation).Any())
                .Select(compilation => new Target() { Compilation = compilation, Node = GetNamespaceDeclarationSyntaxes(compilation).First() } );

            var foundNamespaceDeclarationSyntax = new Target();
            if (namespaceDeclarationSyntaxes.Any())
            {
                foundNamespaceDeclarationSyntax = namespaceDeclarationSyntaxes.First();
            }
            return foundNamespaceDeclarationSyntax;
        }

        private static IEnumerable<NamespaceDeclarationSyntax> GetNamespaceDeclarationSyntaxes(Compilation compilation)
        {
            return compilation.SyntaxTrees
                .Select(syntaxTree => syntaxTree.GetRoot())
                .SelectMany(root => root.DescendantNodesAndSelf())
                .OfType<NamespaceDeclarationSyntax>();
        }

        public Compilation ReplaceNodeInCompilation(Compilation compilation, SyntaxNode node, SyntaxNode newNode)
        {
            var syntaxTree = node.SyntaxTree;
            var root = syntaxTree.GetRoot();

            var newRoot = root.ReplaceNode(node, newNode);
            var newSyntaxTree = syntaxTree.WithRootAndOptions(newRoot, syntaxTree.Options);
            var newCompilation = compilation.ReplaceSyntaxTree(syntaxTree, newSyntaxTree);

            return newCompilation;
        }
    }
}
