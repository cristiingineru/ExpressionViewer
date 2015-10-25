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
        public async Task<SyntaxNode> FindTarget(Solution solution)
        {
            if (solution == null)
            {
                return null;
            }

            var projectIds = solution.GetProjectDependencyGraph().GetTopologicallySortedProjects();

            var projects = projectIds.Select(projectId => solution.GetProject(projectId));

            var compilation = (await projects.First().GetCompilationAsync());
            var roots = compilation.SyntaxTrees.Select(syntaxTree => syntaxTree.GetRoot());
            var invocationExpressions = roots
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
    }
}
