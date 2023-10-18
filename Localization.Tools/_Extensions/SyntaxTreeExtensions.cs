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

    internal static IEnumerable<AttributeSyntax> FindAttributeOfType<T>(this SyntaxTree syntaxTree, CSharpCompilation compilation) => syntaxTree.FindAttributeOfType(compilation, typeof(T));

    internal static string DecoratedPropertyName(this AttributeSyntax attributeSyntax) => attributeSyntax.Ancestors().OfType<PropertyDeclarationSyntax>().First().Identifier.Text;

    internal static  string ContaingClassName(this AttributeSyntax attributeSyntax) => attributeSyntax.Ancestors().OfType<ClassDeclarationSyntax>().First().Identifier.Text;

    internal static string ContainingNamespaceName(this AttributeSyntax attributeSyntax) => attributeSyntax.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().First().Name.ToString();

    private static string AttributeName(this Type type) => type.Name.Replace("Attribute", string.Empty);
}
