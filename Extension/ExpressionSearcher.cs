using Microsoft.CodeAnalysis;
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

            var invocationExpressions = solution.GetProjectDependencyGraph().GetTopologicallySortedProjects()
                .Select(async projectId =>
                {
                    var project = solution.GetProject(projectId);
                    var compilation = (await project.GetCompilationAsync());
                    return compilation;
                })
                .Select(compilationTask => compilationTask.Result)
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

        public async Task<SyntaxNode> FindTarget(Solution solution)
        {
            return null;
        }
    }
}
