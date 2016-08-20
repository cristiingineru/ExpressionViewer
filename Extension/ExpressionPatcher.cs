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
        public IEnumerable<object> GetVariableDependencies(SyntaxNode expression, Compilation compilation)
        {
            if (expression == null || compilation == null)
            {
                return Enumerable.Empty<object>();
            }

            var syntaxTree = expression.SyntaxTree;
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var identifiers = syntaxTree.GetRoot()
                .DescendantNodes()
                .Where(d => d.IsKind(SyntaxKind.IdentifierName));

            var symbols = identifiers
                .Select(identifier => semanticModel.GetSymbolInfo(identifier))
                .Select(info => info.Symbol)
                .Where(symbol => symbol != null);
                //.Where(symbol => symbol is Microsoft.CodeAnalysis.CSharp.Symbols.SourceMemberFieldSymbol).ToList();

            return symbols.Select(symbol => symbol as object);
        }
    }
}
