using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IntegrationEventOverviewer;

public static class SyntaxHelper
{
    internal static string GetNamespaceFrom(SyntaxNode s) =>
        s.SyntaxTree.GetRoot().DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>().First().Name.ToString();
}