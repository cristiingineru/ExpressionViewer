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
        public async Task<Nullable<int>> FindTarget(Solution solution)
        {
            if (solution == null)
            {
                return null;
            }

            var projectIds = solution.GetProjectDependencyGraph().GetTopologicallySortedProjects();

            var projects = projectIds.Select(projectId => solution.GetProject(projectId));

            var compilation = (await projects.First().GetCompilationAsync());
            //t.Wait();
            //var compilation = t.Result;
            var roots = compilation.SyntaxTrees.Select(syntaxTree => syntaxTree.GetRoot());

            return null;
        }
    }
}
