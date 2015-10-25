using Extension;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionViewerTests
{
    [TestClass]
    public class ExpressionBuilderTests
    {
        [TestMethod]
        public async Task BuildWrapper_WithNullInnerExpression_ReturnsWrapper()
        {
            var result = await BuildWrapper_WithNullInnerExpression();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task BuildWrapper_WithNullInnerExpression_ReturnsClassWrapper()
        {
            var result = await BuildWrapper_WithNullInnerExpression();

            Assert.IsNotNull(result is ClassDeclarationSyntax);
        }

        [TestMethod]
        public async Task BuildWrapper_WithNullInnerExpression_ReturnsStaticMethodWrapper()
        {
            var result = await BuildWrapper_WithNullInnerExpression();

            var wrapperMethodModifiers = GetWrapperMethod(result)
                .Modifiers
                .Select(modifier => modifier.ToString());
            Assert.IsTrue(wrapperMethodModifiers.Contains("static"));
        }

        [TestMethod]
        public async Task BuildWrapper_WithNullInnerExpression_ReturnsMethodReturningString()
        {
            var result = await BuildWrapper_WithNullInnerExpression();

            var wrapperMethodReturnType = GetWrapperMethod(result).ReturnType;
            Assert.AreEqual("string", wrapperMethodReturnType.ToString());
        }

        private async Task<SyntaxNode> BuildWrapper_WithNullInnerExpression()
        {
            var builder = new ExpressionBuilder();

            var result = await builder.BuildWrapper(null);

            return result;
        }

        private MethodDeclarationSyntax GetWrapperMethod(SyntaxNode root)
        {
            return (root as ClassDeclarationSyntax).Members
                .Where(member => member.IsKind(SyntaxKind.MethodDeclaration))
                .OfType<MethodDeclarationSyntax>()
                .First(methodDeclaration => methodDeclaration.Identifier.ToString() == ExpressionBuilder.WrapperMethodName);
        }
    }
}
