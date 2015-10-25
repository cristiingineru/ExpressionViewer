using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extension
{
    public class ExpressionBuilder
    {
        public static string WrapperClassName { get { return "WrapperClass"; } }

        public static string WrapperMethodName { get { return "WrapperMethod"; } }


        public async Task<SyntaxNode> BuildWrapper(SyntaxNode innerExpression)
        {
            var newClass =
                SyntaxFactory.ClassDeclaration(WrapperClassName)
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        SyntaxFactory.Token(
                            SyntaxKind.PublicKeyword)))
                .AddMembers(
                    SyntaxFactory.MethodDeclaration(
                        SyntaxFactory.PredefinedType(
                            SyntaxFactory.Token(
                                SyntaxKind.StringKeyword)),
                        SyntaxFactory.Identifier(
                            WrapperMethodName))
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
                        SyntaxFactory.SingletonList<StatementSyntax>(
                            SyntaxFactory.ReturnStatement(

                                innerExpression as ExpressionSyntax)

                            .WithReturnKeyword(
                                SyntaxFactory.Token(
                                    SyntaxKind.ReturnKeyword))
                            .WithSemicolonToken(
                                SyntaxFactory.Token(
                                    SyntaxKind.SemicolonToken))))
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
