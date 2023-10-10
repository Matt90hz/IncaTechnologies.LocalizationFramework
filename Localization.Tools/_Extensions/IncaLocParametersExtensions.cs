using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Localization.Tools.Extensions;

internal static class IncaLocParametersExtensions
{
    internal static IncaLocParameters ToIncaLocParameters(this AttributeSyntax attribute) => new(attribute.GetContainingNamespaceName(), attribute.GetContaingClassName(), attribute.GetDecoratedPropertyName());
}
