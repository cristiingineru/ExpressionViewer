using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension
{
    public class ExpressionPatcher
    {
        public IEnumerable<object> GetOutOfScopeVariables(SyntaxNode expression, Compilation compilation)
        {
            var syntaxTree = expression.SyntaxTree;
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var identifiers = syntaxTree.GetRoot()
                .DescendantNodes()
                .Where(d => d.IsKind(SyntaxKind.IdentifierName));

            var x = identifiers
                .Select(identifier => semanticModel.GetSymbolInfo(identifier));

            return Enumerable.Empty<object>();
        }
    }
}
