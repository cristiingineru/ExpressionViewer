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
        public IEnumerable<ISymbol> GetVariableDependencies(SyntaxNode expression, Compilation compilation)
        {
            if (expression == null || compilation == null)
            {
                return Enumerable.Empty<ISymbol>();
            }

            var semanticModel = compilation.GetSemanticModel(expression.SyntaxTree);
            var variables = semanticModel.AnalyzeDataFlow(expression).DataFlowsIn;

            return variables;
        }

        public SyntaxNode ReplaceVariablesWithConstants(SyntaxNode espression, IEnumerable<ISymbol> variables)
        {
            return espression;
        }
    }
}
