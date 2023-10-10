using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Localization.Tools.Extensions;

internal static class SyntaxTreeExtensions
{
    internal static bool ContainsAnIdentifierWithText(this SyntaxNode syntaxNode, string text) => syntaxNode.DescendantNodes().OfType<IdentifierNameSyntax>().Any(id => id.Identifier.Text.Equals(text));

    internal static IEnumerable<AttributeSyntax> FindAttributeByIdentifier(this SyntaxTree syntaxTree, string identifier) => 
        syntaxTree
            .GetRoot()
            .DescendantNodes()
            .OfType<AttributeSyntax>()
            .Where(att => att.ContainsAnIdentifierWithText(identifier));

    internal static IEnumerable<AttributeSyntax> FindAttributeOfType(this SyntaxTree syntaxTree, CSharpCompilation compilation, Type toEvaluate) => 
        syntaxTree
            .FindAttributeByIdentifier(toEvaluate.AttributeName())
            .Where(attr => compilation.GetSemanticModel(syntaxTree).IsAttributeOfType(attr, toEvaluate));

    internal static string GetSyntaxNodeName(this SyntaxNode syntaxNode) => syntaxNode.DescendantNodes().OfType<IdentifierNameSyntax>().First().Identifier.Text;

    internal static string GetAttributeName(this AttributeSyntax attributeSyntax) => attributeSyntax.GetSyntaxNodeName();

    internal static string GetDecoratedPropertyName(this AttributeSyntax attributeSyntax) => attributeSyntax.Ancestors().OfType<PropertyDeclarationSyntax>().First().Identifier.Text;

    internal static  string GetContaingClassName(this AttributeSyntax attributeSyntax) => attributeSyntax.Ancestors().OfType<ClassDeclarationSyntax>().First().Identifier.Text;

    internal static string GetContainingNamespaceName(this AttributeSyntax attributeSyntax) => attributeSyntax.Ancestors().OfType<NamespaceDeclarationSyntax>().First().GetSyntaxNodeName();

    private static string AttributeName(this Type type) => type.Name.Replace("Attribute", string.Empty);
}
